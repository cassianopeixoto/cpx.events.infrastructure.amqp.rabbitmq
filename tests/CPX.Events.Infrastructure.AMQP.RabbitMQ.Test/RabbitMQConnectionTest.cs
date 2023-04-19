namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Test;

public sealed class RabbitMQConnectionTest
{
    [Fact]
    public void Should_not_be_able_to_instantiate_with_null_connection_factory()
    {
        // Arrange
        IConnectionFactory? connectionFactory = null;
        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            new RabbitMQConnection(connectionFactory);
        });
    }

    [Fact]
    public void Should_be_able_to_create_model()
    {
        // Arrange
        var mockChannel = new Mock<IModel>();
        var channel = mockChannel.Object;

        var mockConnection = new Mock<IConnection>();
        mockConnection.Setup(o => o.CreateModel()).Returns(channel).Verifiable();
        mockConnection.Setup(o => o.Dispose()).Verifiable();
        var connection = mockConnection.Object;

        var mockConnectionFactory = new Mock<IConnectionFactory>();
        mockConnectionFactory.Setup(o => o.CreateConnection()).Returns(connection).Verifiable();
        var connectionFactory = mockConnectionFactory.Object;
        // Act
        var rabbitMQConnection = new RabbitMQConnection(connectionFactory);
        var result = rabbitMQConnection.GetChannel();
        rabbitMQConnection.Dispose();
        // Assert
        Assert.IsAssignableFrom<IRabbitMQConnection>(rabbitMQConnection);
        Assert.Equal(channel, result);
        mockConnection.VerifyAll();
        mockConnectionFactory.VerifyAll();
    }
}