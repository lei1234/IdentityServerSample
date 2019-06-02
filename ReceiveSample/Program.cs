using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ReceiveSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "127.0.0.1" };

            var conn = factory.CreateConnection();

            var channel = conn.CreateModel();

            channel.QueueDeclare("hello world", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;

                var message = "server:"+Encoding.UTF8.GetString(body);

                Console.WriteLine(message);
            };

            channel.BasicConsume("hello world", true, consumer);

            Console.WriteLine(" Press [enter] to exit.");

            Console.ReadLine();
        }
    }
}
