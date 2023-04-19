using RabbitMQ.Client;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Abstract;

public interface IRabbitMQConnection : IDisposable
{
    IModel GetChannel();
}
