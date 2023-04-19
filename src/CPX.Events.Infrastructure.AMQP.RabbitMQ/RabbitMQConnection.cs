using CPX.Events.Infrastructure.AMQP.RabbitMQ.Abstract;
using RabbitMQ.Client;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ;

public sealed class RabbitMQConnection : IRabbitMQConnection
{
    private readonly IConnection connection;

    private IModel? channel;

    public RabbitMQConnection(IConnectionFactory? connectionFactory)
    {
        if (connectionFactory is null) throw new ArgumentNullException(nameof(connectionFactory));

        connection = connectionFactory.CreateConnection();
    }

    public IModel GetChannel()
    {
        if (channel is null)
            channel = connection.CreateModel();

        return channel;
    }

    public void Dispose()
    {
        connection.Dispose();
    }
}
