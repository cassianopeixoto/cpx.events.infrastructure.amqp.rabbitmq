using CPX.Events.Abstract;
using CPX.Events.Infrastructure.AMQP.Abstract;
using Microsoft.Extensions.Hosting;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Http.Extensions;

public sealed class SubscriberHostedService : IHostedService
{
    private readonly ISubscribeService subscribeService;

    private readonly IDictionary<Type, string> configuration;

    public SubscriberHostedService(ISubscribeService? subscribeService, IDictionary<Type, string> configuration)
    {
        if (subscribeService is null) throw new ArgumentNullException(nameof(subscribeService));

        this.subscribeService = subscribeService;
        this.configuration = configuration;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var subscriberType = subscribeService.GetType();
            var methodName = "Subscribe";
            var subscribeMethod = subscriberType.GetMethods().SingleOrDefault(o => o.Name.Equals(methodName));

            if (subscribeMethod is not null)
            {
                foreach (var item in configuration)
                {
                    var eventType = item.Key;

                    if (typeof(Event).IsAssignableFrom(eventType))
                    {
                        var routingKey = item.Value;
                        var genericSubscribe = subscribeMethod.MakeGenericMethod(new[] { eventType });
                        object ct = cancellationToken;
                        genericSubscribe.Invoke(subscribeService, new[] { routingKey, ct });
                    }
                }
            }
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.Run(subscribeService.Dispose);
    }
}
