using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace DirectSendSample
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
                    channel.ExchangeDeclare("direct_log", "direct");

                    var index = 0;

                    var routeKey = "";

                    while (true)
                    {
                        routeKey = index % 2 == 0 ? "info" : "error";

                        index++;

                        var message = "direct_logs" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                        channel.BasicPublish("direct_log", routeKey, null, Encoding.UTF8.GetBytes(message));

                        Console.WriteLine($"routekey:{routeKey},message:{message}");

                        Thread.Sleep(1000);
                    }
                }
            }

            Console.WriteLine("over");

            Console.Read();
        }
    }
}
