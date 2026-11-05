using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductMessengerRabbitMQ.API.Models;

namespace ProductMessengerRabbitMQ.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ProductModel> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddInboxStateEntity();
        }
    }   
}
