using CPX.Events.Infrastructure.AMQP.Abstract;
using CPX.Events.Abstract;
using CPX.Events.Infrastructure.AMQP.RabbitMQ.Abstract;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ;

public sealed class RabbitMQSubscriberService : RabbitMQService, ISubscribeService
{
    private readonly IServiceProvider serviceProvider;

    private readonly int maxSeconds;

    public RabbitMQSubscriberService(string serviceBusName, string serviceName, IRabbitMQConnection? connection, IServiceProvider? serviceProvider, int maxSeconds) : base(serviceBusName, serviceName, connection)
    {
        if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));

        this.serviceProvider = serviceProvider;
        this.maxSeconds = maxSeconds;
    }

    public void Subscribe<TEvent>(string routingKey) where TEvent : Event
    {
        Configure(routingKey).Consume<TEvent>(routingKey);
    }

    private RabbitMQSubscriberService Configure(string routingKey)
    {
        var exchange = GetExchangeName();
        var exchangeDlq = GetExchangeName(true);
        var queue = GetQueueName(routingKey);
        var queueDlq = GetQueueName(routingKey, true);
        var routingKeyDlq = GetRoutingKeyDlq(routingKey);

        ExchangeDeclare(serviceBusName)
        .ExchangeDeclare(exchangeDlq)
        .QueueDeclare(queueDlq)
        .QueueBind(queueDlq, exchangeDlq, routingKeyDlq)
        .ExchangeDeclare(exchange)
        .ExchangeBind(exchange, serviceBusName, routingKey)
        .QueueDeclare(queue, new Dictionary<string, object> {
            {"x-dead-letter-exchange", exchangeDlq},
            {"x-dead-letter-routing-key", routingKeyDlq},
        })
        .QueueBind(queue, exchange, routingKey);

        return this;
    }

    private void Consume<TEvent>(string routingKey) where TEvent : Event
    {
        var consumer = new EventingBasicConsumer(channel);
        var consumerTag = Guid.NewGuid().ToString();
        var queue = GetQueueName(routingKey);
        consumer.Received += async (sender, e) =>
        {
            try
            {
                var serialized = Encoding.UTF8.GetString(e.Body.ToArray());
                var @event = JsonEventConvert.Deserialize<TEvent>(serialized);
                var eventHandler = serviceProvider.GetService(typeof(IEventHandler<TEvent>)) as IEventHandler<TEvent>;

                if (@event is not null && eventHandler is not null)
                {
                    var source = new CancellationTokenSource();
                    source.CancelAfter(maxSeconds * 1000);
                    var cancellationToken = source.Token;
                    await eventHandler.HandleAsync(@event, cancellationToken);
                }

                channel.BasicAck(e.DeliveryTag, false);
            }
            catch
            {
                channel.BasicNack(e.DeliveryTag, false, false);
            }
        };

        channel.BasicQos(0, 1, false);
        channel.BasicConsume(queue, false, consumerTag, false, false, null, consumer);
    }

    private RabbitMQSubscriberService ExchangeDeclare(string exchangeName)
    {
        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
        return this;
    }

    private RabbitMQSubscriberService ExchangeBind(string exchangeDestinationName, string exchangeSourceName, string routingKey)
    {
        channel.ExchangeBind(exchangeDestinationName, exchangeSourceName, routingKey, null);
        return this;
    }

    private RabbitMQSubscriberService QueueDeclare(string queueName, IDictionary<string, object>? arguments = null)
    {
        channel.QueueDeclare(queueName, true, false, false, arguments);
        return this;
    }

    private RabbitMQSubscriberService QueueBind(string queueName, string exchangeName, string routingKey)
    {
        channel.QueueBind(queueName, exchangeName, routingKey, null);
        return this;
    }

    private string GetExchangeName(bool isDeadLetterExchange = false)
    {
        var exchangeName = $"{serviceBusName}_{serviceName}.exchange";

        if (isDeadLetterExchange)
            exchangeName = $"{exchangeName}.dlq";

        return exchangeName;
    }

    private string GetQueueName(string routingKey, bool isDeadLetterQueue = false)
    {
        var queueName = $"{GetExchangeName(isDeadLetterQueue)}_{routingKey}.queue";

        if (isDeadLetterQueue)
            queueName = $"{queueName}.dlq";

        return queueName;
    }

    private string GetRoutingKeyDlq(string routingKey)
    {
        return $"{serviceBusName}_{routingKey}.dlq";
    }
}
