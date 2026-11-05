using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProductMessengerRabbitMQ.Worker.Consumers;

namespace ProductMessengerRabbitMQ.Worker.Messaging
{
    public static class MassTransitConfigurations
    {
        public static void AddMassTransitConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));

            services.AddMassTransit(x =>
            {
                x.AddConsumer<ProductCreatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                    cfg.Host(rabbitMqOptions.Host, h =>
                    {
                        h.Username(rabbitMqOptions.User);
                        h.Password(rabbitMqOptions.Password);
                    });

                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
