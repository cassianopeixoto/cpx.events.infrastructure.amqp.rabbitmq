namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Http.Extensions;

public sealed class RabbitMQConfiguration
{
    private readonly RabbitMQSubscriberConfiguration subscriberConfiguration;
    
    private readonly RabbitMQPublisherConfiguration publisherConfiguration;

    public RabbitMQConfiguration(RabbitMQSubscriberConfiguration subscriberConfiguration, RabbitMQPublisherConfiguration publisherConfiguration)
    {
        this.subscriberConfiguration = subscriberConfiguration;
        this.publisherConfiguration = publisherConfiguration;
    }

    public void UseConsumers(Action<RabbitMQSubscriberConfiguration> action)
    {
        action.Invoke(subscriberConfiguration);
    }

    public void UseProducers(Action<RabbitMQPublisherConfiguration> action)
    {
        action.Invoke(publisherConfiguration);
    }
}
