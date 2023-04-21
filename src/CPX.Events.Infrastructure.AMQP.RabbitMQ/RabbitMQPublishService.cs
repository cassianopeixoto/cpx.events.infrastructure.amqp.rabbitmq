using System.Text;
using CPX.Events.Infrastructure.AMQP.Abstract;
using CPX.Events.Abstract;
using CPX.Events.Infrastructure.AMQP.RabbitMQ.Abstract;
using RabbitMQ.Client;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ;

public sealed class RabbitMQPublishService : RabbitMQService, IPublishService
{
    public RabbitMQPublishService(string serviceBusName, string serviceName, IRabbitMQConnection? connection) : base(serviceBusName, serviceName, connection)
    {
    }

    public void Publish(string routingKey, Event @event)
    {
        var basicProperties = GetBasicProperties(@event);
        var body = GetBody(@event);
        channel.BasicPublish(serviceBusName, routingKey, true, basicProperties, body);
    }

    private IBasicProperties GetBasicProperties(Event @event)
    {
        var basicProperties = channel.CreateBasicProperties();
        basicProperties.Headers = new Dictionary<string, object> {
            {"origin-service-name", serviceName }
        };
        basicProperties.Persistent = true;
        return basicProperties;
    }

    private static ReadOnlyMemory<byte> GetBody(Event @event)
    {
        var serializedEvent = JsonEventConvert.Serialize(@event);
        return Encoding.UTF8.GetBytes(serializedEvent);
    }
}
