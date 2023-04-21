using CPX.Events.Infrastructure.AMQP.Abstract;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Mocks;

public sealed class FooEventHandler : IEventHandler<FooEvent>
{
    public Task HandleAsync(FooEvent @event, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}