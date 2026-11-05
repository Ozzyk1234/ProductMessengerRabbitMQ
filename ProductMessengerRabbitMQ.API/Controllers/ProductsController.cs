using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductMessengerRabbitMQ.API.Data;
using ProductMessengerRabbitMQ.API.Models;
using ProductMessengerRabbitMQ.Contracts.Events;

namespace ProductMessengerRabbitMQ.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(AppDbContext dbContext, IPublishEndpoint publishEndpoint) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductModel product)
        {
            product.Id = Guid.NewGuid();

            dbContext.Products.Add(product);

            await publishEndpoint.Publish(new ProductCreated(
                product.Id,
                product.Name,
                product.Description,
                product.Price
                ));

            await dbContext.SaveChangesAsync();

            return Ok(product);
        }
    }
}
