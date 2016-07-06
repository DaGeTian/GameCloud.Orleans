// Copyright (c) Cragon. All rights reserved.

namespace GF.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Gateway
    {
        //---------------------------------------------------------------------
        public static Gateway Instance { get; private set; }

        private GatewaySessionFactory gatewaySessionFactory = new GatewaySessionFactory();
        private GatewayRunner gatewayRunner = new GatewayRunner();

        //---------------------------------------------------------------------
        public Gateway()
        {
            Instance = this;
        }

        //---------------------------------------------------------------------
        public Task Start(IPAddress ip_address, int port, string orleansClientConfigFile)
        {
            return this.gatewayRunner.Start(ip_address, port, orleansClientConfigFile, gatewaySessionFactory);
        }

        //---------------------------------------------------------------------
        public Task Stop()
        {
            return this.gatewayRunner.Stop();
        }
    }
}
