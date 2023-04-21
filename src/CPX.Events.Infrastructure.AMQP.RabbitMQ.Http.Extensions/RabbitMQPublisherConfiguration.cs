using CPX.Events.Abstract;
using CPX.Events.Infrastructure.AMQP.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Http.Extensions;

public sealed class RabbitMQPublisherConfiguration : Configuration
{
    public RabbitMQPublisherConfiguration(IServiceCollection services) : base(services)
    {
    }

    public RabbitMQPublisherConfiguration Add<TEvent>(string routingKey) where TEvent : Event
    {
        services.AddScoped<IEventEmitterService<TEvent>, RabbitMQEventEmitterService<TEvent>>((p) =>
        {
            var publisher = p.GetService<IPublishService>();
            return new RabbitMQEventEmitterService<TEvent>(routingKey, publisher);
        });

        return this;
    }
}