// Copyright (c) Cragon. All rights reserved.

namespace GF.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;
    using GF.Unity.Common;

    public class GatewayChannelHandler : ChannelHandlerAdapter
    {
        //---------------------------------------------------------------------
        private GatewaySessionFactory factory;
        private ConcurrentDictionary<IChannelHandlerContext, GatewaySession> mapSession
            = new ConcurrentDictionary<IChannelHandlerContext, GatewaySession>();

        //---------------------------------------------------------------------
        public GatewayChannelHandler(GatewaySessionFactory factory)
        {
            this.factory = factory;
        }

        //---------------------------------------------------------------------
        public override void ChannelActive(IChannelHandlerContext context)
        {
            var session = (GatewaySession)this.factory.createRpcSession(null);
            mapSession[context] = session;

            session.ChannelActive(context);
        }

        //---------------------------------------------------------------------
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            GatewaySession session = null;
            mapSession.TryRemove(context, out session);

            if (session != null)
            {
                session.ChannelInactive(context);
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

            GatewaySession session = null;
            if (mapSession.TryGetValue(context, out session))
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
            Console.WriteLine("Exception: " + exception);

            context.CloseAsync();
        }
    }
}
