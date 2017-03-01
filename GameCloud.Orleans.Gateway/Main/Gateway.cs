// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Orleans.Gateway
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Groups;
    using Autofac;
    using Autofac.Configuration;
    using Autofac.Core;
    using GameCloud.Unity.Common;

    public class Gateway
    {
        //---------------------------------------------------------------------
        private GatewayRunner gatewayRunner = new GatewayRunner();
        System.Timers.Timer timer = null;

        //---------------------------------------------------------------------
        public static Gateway Instance { get; private set; }
        public IChannelGroup ChannelGroup { get; set; }
        public IContainer GatewaySessionListenerContainer { get; private set; }
        public string ConsoleTitle { get; set; }
        public string Version { get; set; }

        //---------------------------------------------------------------------
        public Gateway(string title, string version)
        {
            Instance = this;
            ConsoleTitle = title;
            Version = version;
        }

        //---------------------------------------------------------------------
        public Task Start(IPAddress ip_address, int port)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
            this.GatewaySessionListenerContainer = builder.Build();

            var gatewaySessionFactory = new GatewaySessionFactory();

            this.timer = new System.Timers.Timer();
            this.timer.Interval = 3000;
            this.timer.Elapsed += (obj, evt) =>
            {
                int count = 0;
                IChannelGroup g = this.ChannelGroup;
                if (g != null)
                {
                    count = g.Count;
                }

                string title = Gateway.Instance.ConsoleTitle;
                string version = Gateway.Instance.Version;
                Console.Title = string.Format("{0} {1} ConnectionCount={2}", title, version, count);
            };
            this.timer.Start();

            return this.gatewayRunner.Start(ip_address, port, gatewaySessionFactory);
        }

        //---------------------------------------------------------------------
        public Task Stop()
        {
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Close();
                this.timer = null;
            }

            return this.gatewayRunner.Stop();
        }

        //---------------------------------------------------------------------
        public void addSession(IChannelHandlerContext c)
        {
            IChannelGroup g = ChannelGroup;
            if (g == null)
            {
                lock (this)
                {
                    if (ChannelGroup == null)
                    {
                        g = ChannelGroup = new DefaultChannelGroup(c.Executor);
                    }
                }
            }

            ChannelGroup.Add(c.Channel);
        }

        //---------------------------------------------------------------------
        public void removeSession(IChannelHandlerContext c)
        {
            ChannelGroup.Remove(c.Channel);
        }
    }
}
