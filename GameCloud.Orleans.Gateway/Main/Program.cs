// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Orleans.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Orleans;
    using global::Orleans.Runtime.Configuration;

    class Program
    {
        //---------------------------------------------------------------------
        static async Task RunGatewayAsync()
        {
            EbLog.NoteCallback = Console.WriteLine;
            EbLog.WarningCallback = Console.WriteLine;
            EbLog.ErrorCallback = Console.WriteLine;

            string orleansClientConfigFile = ConfigurationManager.AppSettings["OrleansClientConfiguration"];
            string ip = ConfigurationManager.AppSettings["ListenIP"];
            string port = ConfigurationManager.AppSettings["ListenPort"];
            IPAddress host = IPAddress.Parse(ip);

            Gateway gateway = new Gateway();
            await gateway.Start(host, int.Parse(port));

            // initialize the grain client, with some retry logic
            var initialized = false;
            while (!initialized)
            {
                try
                {
                    GrainClient.Initialize(orleansClientConfigFile);
                    initialized = GrainClient.IsInitialized;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }

            Console.WriteLine("Gateway Start");
            Console.WriteLine("Press Enter To Exit...");
            Console.ReadKey();

            GrainClient.Uninitialize();

            await gateway.Stop();

            Console.WriteLine("Gateway Stop");
        }

        //---------------------------------------------------------------------
        static void Main() => RunGatewayAsync().Wait();
    }
}
