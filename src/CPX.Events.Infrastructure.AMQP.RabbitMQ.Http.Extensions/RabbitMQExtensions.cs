using Microsoft.Extensions.DependencyInjection;
using CPX.Events.Infrastructure.AMQP.RabbitMQ.Abstract;
using RabbitMQ.Client;
using CPX.Events.Infrastructure.AMQP.Abstract;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Http.Extensions;

public static class RabbitMQExtensions
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, EventBusConfiguration configuration, Action<RabbitMQConfiguration> configure)
    {
        services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>((p) =>
        {
            var connectionFactory = new ConnectionFactory { Uri = new Uri(configuration.ConnectionString) };
            return new RabbitMQConnection(connectionFactory);
        });
        services.AddTransient<ISubscribeService, RabbitMQSubscriberService>((p) =>
        {
            var connection = p.GetService<IRabbitMQConnection>();
            return new RabbitMQSubscriberService(configuration.ServiceBusName, configuration.ServiceName, connection, p);
        });
        services.AddScoped<IPublishService, RabbitMQPublishService>((p) =>
        {
            var connection = p.GetService<IRabbitMQConnection>();
            return new RabbitMQPublishService(configuration.ServiceBusName, configuration.ServiceName, connection);
        });

        var rabbitMQSubscriberConfiguration = new RabbitMQSubscriberConfiguration(services);
        var rabbitMQPublisherConfiguration = new RabbitMQPublisherConfiguration(services);
        var rabbitMQConfiguration = new RabbitMQConfiguration(rabbitMQSubscriberConfiguration, rabbitMQPublisherConfiguration);
        configure.Invoke(rabbitMQConfiguration);

        rabbitMQSubscriberConfiguration.AddHostedService();

        return services;
    }
}
