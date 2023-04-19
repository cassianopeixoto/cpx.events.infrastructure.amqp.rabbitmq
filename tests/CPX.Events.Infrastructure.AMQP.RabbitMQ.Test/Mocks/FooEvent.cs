using CPX.Events.Abstract;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Mocks;

public sealed class FooEvent : Event
{
    public FooEvent(DateTimeOffset createdAt) : base(createdAt)
    {
    }
}