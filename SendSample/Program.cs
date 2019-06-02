using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace SendSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName="127.0.0.1"};

            using(var conn = factory.CreateConnection())
            {
                using(var channel = conn.CreateModel())
                {

                    while (true)
                    {
                        channel.QueueDeclare("hello world", durable: false, exclusive: false, autoDelete: false, arguments: null);

                        var message = "hello world" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                        channel.BasicPublish(exchange: "", routingKey: "hello world", null, Encoding.UTF8.GetBytes(message));

                        Console.WriteLine(message);

                        Thread.Sleep(1000);
                    }

                    

                }
            }
            Console.WriteLine("over");
            Console.Read();
        }
    }
}
