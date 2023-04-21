using Microsoft.Extensions.DependencyInjection;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Http.Extensions;

public abstract class RabbitMQConfiguration
{
    protected readonly IServiceCollection services;

    protected RabbitMQConfiguration(IServiceCollection services)
    {
        this.services = services;
    }
}
