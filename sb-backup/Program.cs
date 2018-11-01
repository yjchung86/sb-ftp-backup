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

            var ftp = new FtpSslClient(host, userName, password);
            ftp.Upload("/opt/app-root/src/oc-config.txt", "oc-config.txt");

            while (true)
            {
                Thread.Sleep(1000000);
            }
        }
    }
}
