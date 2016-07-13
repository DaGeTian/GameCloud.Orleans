// Copyright (c) Cragon. All rights reserved.

namespace GF.Orleans.Gateway
{
    using DotNetty.Codecs;
    using DotNetty.Common.Internal.Logging;
    using DotNetty.Handlers.Tls;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        //---------------------------------------------------------------------
        static async Task RunGatewayAsync()
        {
            EbLog.NoteCallback = Console.WriteLine;
            EbLog.WarningCallback = Console.WriteLine;
            EbLog.ErrorCallback = Console.WriteLine;

            string orleansClientConfiguration = ConfigurationManager.AppSettings["OrleansClientConfiguration"];
            string ip = ConfigurationManager.AppSettings["ListenIP"];
            string port = ConfigurationManager.AppSettings["ListenPort"];
            IPAddress host = IPAddress.Parse(ip);

            Gateway gateway = new Gateway();
            await gateway.Start(host, int.Parse(port), orleansClientConfiguration);

            Console.WriteLine("Gateway Start ManagedThreadId=" + Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("按回车键退出。。。");
            Console.ReadLine();

            await gateway.Stop();

            Console.WriteLine("Gateway Stop");
        }

        //---------------------------------------------------------------------
        static void Main() => RunGatewayAsync().Wait();
    }
}
