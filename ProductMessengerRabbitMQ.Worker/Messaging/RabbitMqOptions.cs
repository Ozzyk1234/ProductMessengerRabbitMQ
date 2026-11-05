namespace ProductMessengerRabbitMQ.Worker.Messaging
{
    public class RabbitMqOptions
    {
        public string Host { get; set; } = "localhost";
        public string User { get; set; } = "user";
        public string Password { get; set; } = "password";
    }
}
