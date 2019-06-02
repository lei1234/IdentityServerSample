using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace DirectReceiveSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var routerKey = Console.ReadLine();

            var factory = new ConnectionFactory { HostName = "127.0.0.1" };

            var conn = factory.CreateConnection();

            var channel = conn.CreateModel();

            channel.ExchangeDeclare("direct_log", "direct");

            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queueName, "direct_log", routerKey);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"routerKey:{routerKey},message:{message}");
            };

            channel.BasicConsume(queueName, true, consumer);

            Console.ReadLine();
        }
    }
}
