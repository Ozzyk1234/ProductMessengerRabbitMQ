using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductMessengerRabbitMQ.API.Data;
using ProductMessengerRabbitMQ.API.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//configuration of sqlite
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

//configuration of rabitmq
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));

//configuration of masstransit
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.UseSqlite();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
        cfg.Host(rabbitMqOptions.Host, "/", h =>
        {
            h.Username(rabbitMqOptions.User);
            h.Password(rabbitMqOptions.Password);
        });
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
