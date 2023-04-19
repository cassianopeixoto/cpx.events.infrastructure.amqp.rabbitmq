namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Test;

public sealed class IRabbitMQConnectionTest
{
    [Fact]
    public void Should_check_if_GetChannel_method_exists()
    {
        // Arrange
        var typeRabbitMQConnection = typeof(IRabbitMQConnection);
        var methodName = "GetChannel";
        // Act
        var methodInfo = typeRabbitMQConnection.GetMethod(methodName);
        // Assert
        Assert.NotNull(methodInfo);
        if (methodInfo is not null)
        {
            Assert.Equal(typeof(IModel), methodInfo.ReturnType);
        }
    }

    [Fact]
    public void Should_implement_IDisposable()
    {
        // Arrange
        var typeRabbitMQConnection = typeof(IRabbitMQConnection);
        // Act
        var interfaces = typeRabbitMQConnection.GetInterfaces();
        // Assert
        Assert.Single(interfaces);
        var disposableInterface = interfaces.SingleOrDefault();
        if (disposableInterface is not null)
        {
            Assert.Equal(typeof(IDisposable), disposableInterface);
        }
    }
}