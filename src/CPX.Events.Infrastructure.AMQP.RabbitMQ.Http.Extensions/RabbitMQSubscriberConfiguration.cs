using CPX.Events.Abstract;
using CPX.Events.Infrastructure.AMQP.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Http.Extensions;

public sealed class RabbitMQSubscriberConfiguration : Configuration
{
    private readonly IDictionary<Type, string> configuration;

    public RabbitMQSubscriberConfiguration(IServiceCollection services) : base(services)
    {
        configuration = new Dictionary<Type, string>();
    }

    public RabbitMQSubscriberConfiguration Add<TEvent, TEventHandler>(string routingKey) where TEvent : Event where TEventHandler : class, IEventHandler<TEvent>
    {
        services.AddTransient<IEventHandler<TEvent>, TEventHandler>();
        configuration.Add(typeof(TEvent), routingKey);
        return this;
    }

    public void AddHostedService()
    {
        services.AddHostedService((p) =>
        {
            var subscriber = p.GetService<ISubscribeService>();
            return new SubscriberHostedService(subscriber, configuration);
        });
    }
}
