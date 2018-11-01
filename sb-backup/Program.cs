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

            while (true)
            {
                ftp.Upload("/opt/app-root/src/oc-config.txt", "oc-config.txt");  
                Thread.Sleep(100000);
            }
        }
    }
}
