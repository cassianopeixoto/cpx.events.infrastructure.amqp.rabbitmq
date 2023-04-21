using Microsoft.Extensions.DependencyInjection;

namespace CPX.Events.Infrastructure.AMQP.RabbitMQ.Http.Extensions;

public abstract class Configuration
{
    protected readonly IServiceCollection services;

    protected Configuration(IServiceCollection services)
    {
        this.services = services;
    }
}