using System;
using System.IO;
using System.Net;
using System.Threading;

namespace sb_backup
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Environment.GetEnvironmentVariable("HOST");
            var userName = Environment.GetEnvironmentVariable("USERNAME");
            var password = Environment.GetEnvironmentVariable("PASSWORD");

            Console.WriteLine($"{host}:{userName}:{password}");

            while (true)
            {
                DoSomething();    
                Thread.Sleep(100000);
            }
        }

        static void DoSomething()
        {

            DirectoryInfo dirInfo = new DirectoryInfo("/opt/app-root/src");

            foreach(var file in dirInfo.GetFiles())
            {
                Console.WriteLine(file.Name);
            }

            Console.WriteLine(DateTime.Now);
        }
    }
}
