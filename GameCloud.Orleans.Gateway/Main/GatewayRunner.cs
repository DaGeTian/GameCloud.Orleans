// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Orleans.Gateway
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Common.Internal.Logging;
    using DotNetty.Handlers.Logging;
    using DotNetty.Handlers.Tls;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;
    using GameCloud.Unity.Common;

    public class GatewayRunner
    {
        //---------------------------------------------------------------------
        MultithreadEventLoopGroup bossGroup = new MultithreadEventLoopGroup(4);
        MultithreadEventLoopGroup workerGroup = new MultithreadEventLoopGroup(4);
        ServerBootstrap bootstrap = new ServerBootstrap();
        IChannel bootstrapChannel = null;
        System.Timers.Timer timer = null;
        object lockSetSession = new object();
        HashSet<IChannelHandlerContext> setSession = new HashSet<IChannelHandlerContext>();

        //---------------------------------------------------------------------
        public async Task Start(IPAddress ip_address, int port,
            GatewaySessionFactory factory)
        {
            this.bootstrap
                    .Group(this.bossGroup, this.workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 100)
                    .Option(ChannelOption.SoKeepalive, true)
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new LoggingHandler(LogLevel.INFO))
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast(new LengthFieldPrepender(
                            ByteOrder.LittleEndian, 2, 0, false));
                        pipeline.AddLast(new LengthFieldBasedFrameDecoder(
                            ByteOrder.LittleEndian, ushort.MaxValue, 0, 2, 0, 2, true));
                        pipeline.AddLast(new GatewayChannelHandler(factory));
                    }));

            this.bootstrapChannel = await this.bootstrap.BindAsync(ip_address, port);

            this.timer = new System.Timers.Timer();
            this.timer.Interval = 3000;
            this.timer.Elapsed += (obj, evt) =>
            {
                //var t = obj as System.Timers.Timer;

                int count = 0;
                lock (lockSetSession)
                {
                    count = this.setSession.Count;
                }

                string title = Gateway.Instance.ConsoleTitle;
                string version = Gateway.Instance.Version;
                Console.Title = string.Format("{0} {1} ConnectionCount={2}", title, version, count);
            };
            this.timer.Start();
        }

        //---------------------------------------------------------------------
        public async Task Stop()
        {
            try
            {
                if (this.timer != null)
                {
                    this.timer.Stop();
                    this.timer.Close();
                    this.timer = null;
                }

                await this.bootstrapChannel.CloseAsync();
            }
            finally
            {
                Task.WaitAll(this.bossGroup.ShutdownGracefullyAsync(), this.workerGroup.ShutdownGracefullyAsync());
            }
        }

        //---------------------------------------------------------------------
        public void addSession(IChannelHandlerContext c, GatewaySession s)
        {
            lock (lockSetSession)
            {
                this.setSession.Add(c);
            }
        }

        //---------------------------------------------------------------------
        public void removeSession(IChannelHandlerContext c)
        {
            lock (lockSetSession)
            {
                this.setSession.Remove(c);
            }
        }
    }
}
