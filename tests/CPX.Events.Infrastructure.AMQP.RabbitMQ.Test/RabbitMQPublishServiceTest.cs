using CPX.Events.Infrastructure.AMQP.RabbitMQ.Mocks;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Test;

public sealed class RabbitMQPublishServiceTest
{
    [Fact]
    public void Should_not_be_able_to_instantiate_with_null_rabbit_mq_connection()
    {
        // Arrange
        var serviceBusName = "serviceBusName";
        var serviceName = "serviceName";
        IRabbitMQConnection? rabbitMQConnection = null;
        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            new RabbitMQPublishService(serviceBusName, serviceName, rabbitMQConnection);
        });
    }

    [Fact]
    public void Should_be_able_to_publish_event()
    {
        // Arrange
        var serviceBusName = "serviceBusName";
        var serviceName = "serviceName";
        var routingKey = "routingKey";
        var createdAt = DateTimeOffset.UtcNow;
        var @event = new FooEvent(createdAt);
        var mockBasicProperties = new Mock<IBasicProperties>();
        var basicProperties = mockBasicProperties.Object;
        var mockChannel = new Mock<IModel>();
        mockChannel.Setup(o => o.CreateBasicProperties()).Returns(basicProperties).Verifiable();
        mockChannel.Setup(o => o.BasicPublish(serviceBusName, routingKey, true, basicProperties, It.IsAny<ReadOnlyMemory<byte>>())).Verifiable();
        var channel = mockChannel.Object;
        var mockRabbitMQConnection = new Mock<IRabbitMQConnection>();
        mockRabbitMQConnection.Setup(o => o.GetChannel()).Returns(channel).Verifiable();
        var rabbitMQConnection = mockRabbitMQConnection.Object;
        // Act
        var rabbitMQPublishService = new RabbitMQPublishService(serviceBusName, serviceName, rabbitMQConnection);
        rabbitMQPublishService.Publish(routingKey, @event);
        // Assert
        mockChannel.VerifyAll();
        mockRabbitMQConnection.VerifyAll();
    }
}