using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace FanoutSend
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "127.0.0.1" };
            using(var conn = factory.CreateConnection())
            {
                using(var channel = conn.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                    while (true)
                    {
                        var message = "logs" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                        channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, Encoding.UTF8.GetBytes(message));

                        Console.WriteLine("send:" + message);

                        Thread.Sleep(1000);
                    }
                }
            }

            Console.WriteLine("over");

            Console.Read();
        }
    }
}
