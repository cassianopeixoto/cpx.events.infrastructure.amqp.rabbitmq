using CPX.Events.Infrastructure.AMQP.Abstract;
using CPX.Events.Infrastructure.AMQP.RabbitMQ.Mocks;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Test;

public sealed class RabbitMQSubscriberServiceTest
{
    [Fact]
    public void Should_not_be_able_to_instantiate_with_null_rabbit_mq_connection()
    {
        // Arrange
        var serviceBusName = "serviceBusName";
        var serviceName = "serviceName";
        IRabbitMQConnection? rabbitMQConnection = null;
        var mockServiceProvider = new Mock<IServiceProvider>();
        var serviceProvider = mockServiceProvider.Object;
        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            new RabbitMQSubscriberService(serviceBusName, serviceName, rabbitMQConnection, serviceProvider);
        });
    }

    [Fact]
    public void Should_not_be_able_to_instantiate_with_null_service_provider()
    {
        // Arrange
        var serviceBusName = "serviceBusName";
        var serviceName = "serviceName";
        var mockRabbitMQConnection = new Mock<IRabbitMQConnection>();
        IRabbitMQConnection? rabbitMQConnection = mockRabbitMQConnection.Object;
        IServiceProvider? serviceProvider = null;
        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            new RabbitMQSubscriberService(serviceBusName, serviceName, rabbitMQConnection, serviceProvider);
        });
    }

    [Fact]
    public void Should_be_able_to_subscribe_event()
    {
        // Arrange
        var serviceBusName = "serviceBusName";
        var serviceName = "serviceName";
        var routingKey = "routingKey";
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var exchangeName = GetExchangeName(serviceBusName, serviceName);
        var exchangeDlqName = GetExchangeName(serviceBusName, serviceName, true);
        var queueName = GetQueueName(serviceBusName, serviceName, routingKey);
        var queueDlqName = GetQueueName(serviceBusName, serviceName, routingKey, true);
        var routingKeyDlqName = GetRoutingKeyDlq(serviceBusName, routingKey);

        var mockChannel = new Mock<IModel>();
        ExchangeDeclare(mockChannel, serviceBusName)
        .ExchangeDeclare(mockChannel, exchangeDlqName)
        .QueueDeclare(mockChannel, queueDlqName)
        .QueueBind(mockChannel, queueDlqName, exchangeDlqName, routingKeyDlqName)
        .ExchangeDeclare(mockChannel, exchangeName)
        .ExchangeBind(mockChannel, exchangeName, serviceBusName, routingKey)
        .QueueDeclare(mockChannel, queueName, new Dictionary<string, object> {
            {"x-dead-letter-exchange", exchangeDlqName},
            {"x-dead-letter-routing-key", routingKeyDlqName},
        })
        .QueueBind(mockChannel, queueName, exchangeName, routingKey)
        .BasicQos(mockChannel)
        .BasicConsume(mockChannel, queueName)
        .Close(mockChannel);

        var channel = mockChannel.Object;

        var mockConnection = new Mock<IRabbitMQConnection>();
        mockConnection.Setup(o => o.GetChannel()).Returns(channel).Verifiable();
        var connection = mockConnection.Object;

        var mockServiceProvider = new Mock<IServiceProvider>();
        var serviceProvider = mockServiceProvider.Object;
        // Act
        var subscriberService = new RabbitMQSubscriberService(serviceBusName, serviceName, connection, serviceProvider);
        subscriberService.Subscribe<FooEvent>(routingKey, cancellationToken);
        subscriberService.Dispose();
        // Assert
        Assert.IsAssignableFrom<ISubscribeService>(subscriberService);
        Assert.IsAssignableFrom<RabbitMQService>(subscriberService);
        mockChannel.VerifyAll();
        mockConnection.VerifyAll();
    }

    private RabbitMQSubscriberServiceTest ExchangeDeclare(Mock<IModel> mockChannel, string exchangeName)
    {
        mockChannel.Setup(o => o.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null)).Verifiable();
        return this;
    }

    private RabbitMQSubscriberServiceTest ExchangeBind(Mock<IModel> mockChannel, string exchangeDestinationName, string exchangeSourceName, string routingKey)
    {
        mockChannel.Setup(o => o.ExchangeBind(exchangeDestinationName, exchangeSourceName, routingKey, null));
        return this;
    }

    private RabbitMQSubscriberServiceTest QueueDeclare(Mock<IModel> mockChannel, string queueName, IDictionary<string, object>? arguments = null)
    {
        mockChannel.Setup(o => o.QueueDeclare(queueName, true, false, false, arguments));
        return this;
    }

    private RabbitMQSubscriberServiceTest QueueBind(Mock<IModel> mockChannel, string queueName, string exchangeName, string routingKey)
    {
        mockChannel.Setup(o => o.QueueBind(queueName, exchangeName, routingKey, null));
        return this;
    }

    private RabbitMQSubscriberServiceTest BasicQos(Mock<IModel> mockChannel)
    {
        mockChannel.Setup(o => o.BasicQos(0, 1, false)).Verifiable();
        return this;
    }

    private RabbitMQSubscriberServiceTest BasicConsume(Mock<IModel> mockChannel, string queueName)
    {
        mockChannel.Setup(o => o.BasicConsume(queueName, false, It.IsAny<string>(), false, false, null, It.IsAny<IBasicConsumer>())).Verifiable();
        return this;
    }

    private RabbitMQSubscriberServiceTest Close(Mock<IModel> mockChannel)
    {
        mockChannel.Setup(o => o.Close()).Verifiable();
        return this;
    }

    private static string GetExchangeName(string serviceBusName, string serviceName, bool isDeadLetterExchange = false)
    {
        var exchangeName = $"{serviceBusName}_{serviceName}.exchange";

        if (isDeadLetterExchange)
            exchangeName = $"{exchangeName}.dlq";

        return exchangeName;
    }

    private string GetQueueName(string serviceBusName, string serviceName, string routingKey, bool isDeadLetterQueue = false)
    {
        var queueName = $"{GetExchangeName(serviceBusName, serviceName, isDeadLetterQueue)}_{routingKey}.queue";

        if (isDeadLetterQueue)
            queueName = $"{queueName}.dlq";

        return queueName;
    }

    private string GetRoutingKeyDlq(string serviceBusName, string routingKey)
    {
        return $"{serviceBusName}_{routingKey}.dlq";
    }
}