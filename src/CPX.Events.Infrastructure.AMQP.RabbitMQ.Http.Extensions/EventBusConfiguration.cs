namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Http.Extensions;

public sealed class EventBusConfiguration
{
    public EventBusConfiguration(string connectionString, string serviceBusName, string serviceName)
    {
        ConnectionString = connectionString;
        ServiceBusName = serviceBusName;
        ServiceName = serviceName;
    }

    public string ConnectionString { get; }

    public string ServiceBusName { get; }
    
    public string ServiceName { get; }
}