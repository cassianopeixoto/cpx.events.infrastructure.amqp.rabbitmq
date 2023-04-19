using CPX.Events.Infrastructure.AMQP.RabbitMQ.Abstract;
using RabbitMQ.Client;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ;

public abstract class RabbitMQService
{
    protected readonly string serviceBusName;
    protected readonly string serviceName;
    protected readonly IModel channel;

    protected RabbitMQService(string serviceBusName, string serviceName, IRabbitMQConnection? connection)
    {
        if (connection is null) throw new ArgumentNullException(nameof(connection));

        this.serviceBusName = serviceBusName;
        this.serviceName = serviceName;
        this.channel = connection.GetChannel();
    }
}
