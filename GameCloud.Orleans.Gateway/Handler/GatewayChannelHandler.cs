// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Orleans.Gateway
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Groups;
    using GameCloud.Unity.Common;

    public class GatewayChannelHandler : ChannelHandlerAdapter
    {
        //---------------------------------------------------------------------
        private GatewaySessionFactory factory;
        private GatewaySession session;

        //---------------------------------------------------------------------
        public GatewayChannelHandler(GatewaySessionFactory factory)
        {
            this.factory = factory;
        }

        //---------------------------------------------------------------------
        public override void ChannelActive(IChannelHandlerContext context)
        {
            this.session = (GatewaySession)this.factory.createRpcSession(null);
            this.session.ChannelActive(context);

            IChannelGroup g = Gateway.Instance.ChannelGroup;
            if (g == null)
            {
                lock (this)
                {
                    if (Gateway.Instance.ChannelGroup == null)
                    {
                        g = Gateway.Instance.ChannelGroup = new DefaultChannelGroup(context.Executor);
                    }
                }
            }

            Gateway.Instance.ChannelGroup.Add(context.Channel);
        }

        //---------------------------------------------------------------------
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            IChannelGroup g = Gateway.Instance.ChannelGroup;
            if (g != null)
            {
                g.Remove(context.Channel);
            }

            if (this.session != null)
            {
                this.session.ChannelInactive(context);
                this.session = null;
            }
        }

        //---------------------------------------------------------------------
        public override void ChannelRegistered(IChannelHandlerContext context)
        {
        }

        //---------------------------------------------------------------------
        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
        }

        //---------------------------------------------------------------------
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var msg = message as IByteBuffer;
            msg.WithOrder(ByteOrder.BigEndian);

            if (this.session != null)
            {
                session.onRecvData(msg.ToArray());
            }
        }

        //---------------------------------------------------------------------
        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        //---------------------------------------------------------------------
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            IChannelGroup g = Gateway.Instance.ChannelGroup;
            if (g != null)
            {
                g.Remove(context.Channel);
            }

            if (exception is System.ObjectDisposedException)
            {
                // do nothting
            }
            else
            {
                Console.WriteLine("Exception: \n" + exception);
            }

            context.CloseAsync();
        }
    }
}
