using CPX.Events.Abstract;
using CPX.Events.Infrastructure.AMQP.Abstract;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ;

public sealed class RabbitMQEventEmitterService : IEventEmitterService
{
    private readonly string routingKey;

    private readonly IPublishService publishService;

    public RabbitMQEventEmitterService(string routingKey, IPublishService? publishService)
    {
        if (publishService is null) throw new ArgumentNullException(nameof(publishService));

        this.routingKey = routingKey;
        this.publishService = publishService;
    }

    public void Emit(Event @event)
    {
        publishService.Publish(routingKey, @event);
    }
}