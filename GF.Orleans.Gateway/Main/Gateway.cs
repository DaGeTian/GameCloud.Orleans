// Copyright (c) Cragon. All rights reserved.

namespace GF.Orleans.Gateway
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

        //---------------------------------------------------------------------
        public Gateway()
        {
            Instance = this;
        }

        //---------------------------------------------------------------------
        public Task Start(IPAddress ip_address, int port, string orleansClientConfigFile)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
            this.GatewaySessionListenerContainer = builder.Build();

            var gatewaySessionFactory = new GatewaySessionFactory();

            return this.gatewayRunner.Start(ip_address, port, orleansClientConfigFile, gatewaySessionFactory);
        }

        //---------------------------------------------------------------------
        public Task Stop()
        {
            return this.gatewayRunner.Stop();
        }
    }
}
