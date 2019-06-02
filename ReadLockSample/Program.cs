using RedLockNet.SERedis.Configuration;
using System.Collections.Generic;
using System;
using System.Net;
using RedLockNet.SERedis;
using System.Threading.Tasks;
using System.Threading;

namespace ReadLockSample
{
    class Program
    {
        public static string resource = "redis-lock-test";
        public static TimeSpan expiry = TimeSpan.FromSeconds(30);
        public static TimeSpan wait = TimeSpan.FromSeconds(15);
        public static TimeSpan retry = TimeSpan.FromSeconds(1);

        static void Main(string[] args)
        {
            var endPoint = new List<RedLockEndPoint>
            {
                new DnsEndPoint("192.168.233.128", 6379),
            };

            var redLockFactory = RedLockFactory.Create(endPoint);

            Task.Factory.StartNew(() =>
            {
                using (var redLock = redLockFactory.CreateLock(resource, expiry))
                {
                    if (redLock.IsAcquired)
                    {
                        
                        Console.WriteLine("task 获取到锁");
                        Thread.Sleep(10000);
                    }
                    else
                    {
                        Console.WriteLine("task 锁被占用");
                    }
                }
            });

            Thread.Sleep(1000);

            using(var redLock= redLockFactory.CreateLock(resource,expiry,wait,retry))
            {
                
                if (redLock.IsAcquired)
                {
                    Console.WriteLine("main 获取到锁");
                }
                else
                {
                    Console.WriteLine("main 锁被占用");
                }
            }

            Console.Read();
        }
    }
}
