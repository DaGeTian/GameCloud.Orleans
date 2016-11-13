// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Orleans.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Configuration;
    using Autofac.Core;

    public class Gateway
    {
        //---------------------------------------------------------------------
        private GatewayRunner gatewayRunner = new GatewayRunner();

        //---------------------------------------------------------------------
        public static Gateway Instance { get; private set; }
        public IContainer GatewaySessionListenerContainer { get; private set; }
        public string ConsoleTitle { get; set; }
        public string Version { get; set; }

        //---------------------------------------------------------------------
        public Gateway()
        {
            Instance = this;
            ConsoleTitle = "";
            Version = "";
        }

        //---------------------------------------------------------------------
        public Task Start(IPAddress ip_address, int port)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
            this.GatewaySessionListenerContainer = builder.Build();

            var gatewaySessionFactory = new GatewaySessionFactory();

            return this.gatewayRunner.Start(ip_address, port, gatewaySessionFactory);
        }

        //---------------------------------------------------------------------
        public Task Stop()
        {
            return this.gatewayRunner.Stop();
        }
    }
}
