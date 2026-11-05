using System;
using System.Collections.Generic;
using System.Text;

namespace ProductMessengerRabbitMQ.Contracts.Events
{
    public record ProductCreated(Guid Id, string Name, string? Description, decimal Price);
}
