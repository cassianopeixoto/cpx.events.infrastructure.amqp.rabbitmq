using CPX.Events.Infrastructure.AMQP.Abstract;
using CPX.Events.Infrastructure.AMQP.RabbitMQ.Mocks;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Test;

public sealed class RabbitMQEventEmitterServiceTest
{
    [Fact]
    public void Should_not_be_able_to_instantiate_with_null_publish_service()
    {
        // Arrange
        var routingKey = "routingKey";
        IPublishService? publishService = null;
        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            new RabbitMQEventEmitterService<FooEvent>(routingKey, publishService);
        });
    }

    [Fact]
    public void Should_be_able_to_emit_event()
    {
        // Arrange
        var createdAt = DateTimeOffset.UtcNow;
        var @event = new FooEvent(createdAt);
        var routingKey = "routingKey";
        var mockPublishService = new Mock<IPublishService>();
        mockPublishService.Setup(o => o.Publish(routingKey, @event)).Verifiable();
        var publishService = mockPublishService.Object;
        // Act
        var rabbitMQEventEmitterService = new RabbitMQEventEmitterService<FooEvent>(routingKey, publishService);
        rabbitMQEventEmitterService.Emit(@event);
        // Assert
        Assert.IsAssignableFrom<IEventEmitterService<FooEvent>>(rabbitMQEventEmitterService);
        mockPublishService.VerifyAll();
    }
}