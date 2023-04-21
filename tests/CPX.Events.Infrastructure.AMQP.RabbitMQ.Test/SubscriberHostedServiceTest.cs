using CPX.Events.Infrastructure.AMQP.Abstract;
using CPX.Events.Infrastructure.AMQP.RabbitMQ.Http.Extensions;
using CPX.Events.Infrastructure.AMQP.RabbitMQ.Mocks;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Test;

public sealed class SubscriberHostedServiceTest
{
    [Fact]
    public async Task Should_be_able_to_start_and_stop_service()
    {
        // Arrange
        var routingKey = "routingKey";
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var mockSubscribeService = new Mock<ISubscribeService>();
        mockSubscribeService.Setup(o => o.Subscribe<FooEvent>(routingKey, cancellationToken)).Verifiable();
        mockSubscribeService.Setup(o => o.Dispose()).Verifiable();
        var subscribeService = mockSubscribeService.Object;

        var configuration = new Dictionary<Type, string> {
            { typeof(FooEvent), routingKey }
        };
        // Act
        var subscriberHostedService = new SubscriberHostedService(subscribeService, configuration);
        await subscriberHostedService.StartAsync(cancellationToken);
        await subscriberHostedService.StopAsync(cancellationToken);
        // Assert
        mockSubscribeService.VerifyAll();
    }
}