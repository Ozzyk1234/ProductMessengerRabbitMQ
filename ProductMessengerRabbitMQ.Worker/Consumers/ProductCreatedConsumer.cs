using MassTransit;
using ProductMessengerRabbitMQ.Contracts.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductMessengerRabbitMQ.Worker.Consumers
{
    public class ProductCreatedConsumer(ILogger<ProductCreated> logger) : IConsumer<ProductCreated>
    {
        public Task Consume(ConsumeContext<ProductCreated> context)
        {
            var message = context.Message;

            logger.LogInformation($"Product created: Id: {message.Id}, Name: {message.Name}, Description: {message.Description}, Price: {message.Price}");

            return Task.CompletedTask;
        }
    }
}
