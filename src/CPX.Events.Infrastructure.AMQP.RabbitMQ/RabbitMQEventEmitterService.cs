using CPX.Events.Abstract;
using CPX.Events.Infrastructure.AMQP.Abstract;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ;

public sealed class RabbitMQEventEmitterService<TEvent> : IEventEmitterService<TEvent> where TEvent : Event
{
    private readonly string routingKey;

    private readonly IPublishService publishService;

    public RabbitMQEventEmitterService(string routingKey, IPublishService? publishService)
    {
        if (publishService is null) throw new ArgumentNullException(nameof(publishService));

        this.routingKey = routingKey;
        this.publishService = publishService;
    }

    public void Emit(TEvent @event)
    {
        publishService.Publish(routingKey, @event);
    }
}