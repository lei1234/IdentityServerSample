using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace FanoutReceive
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "127.0.0.1" };

            var conn = factory.CreateConnection();

            var channel = conn.CreateModel();

            channel.ExchangeDeclare("logs", "fanout");

            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queueName, "logs", "", null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;

                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine("server:" + message);
            };

            channel.BasicConsume(queueName, true, consumer);

            Console.Read();
        }
    }
}
