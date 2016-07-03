// Copyright (c) Cragon. All rights reserved.

namespace Test.Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Text;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;
    using GF.Unity.Common;

    public class ClientHandler : ChannelHandlerAdapter
    {
        private RpcSessionFactory factory;
        private ConcurrentDictionary<IChannelHandlerContext, ClientSession> mapSession
            = new ConcurrentDictionary<IChannelHandlerContext, ClientSession>();

        public ClientHandler(RpcSessionFactory factory)
        {
            this.factory = factory;

            //readonly IByteBuffer initialMessage;
            //this.initialMessage = Unpooled.Buffer(256);
            //byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world");
            //this.initialMessage.WriteBytes(messageBytes);

            Console.WriteLine("Client send Msg!");
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            //context.WriteAndFlushAsync(this.initialMessage);

            var session = (ClientSession)this.factory.createRpcSession(null);
            mapSession[context] = session;

            session.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            ClientSession session = null;
            mapSession.TryRemove(context, out session);

            if (session != null)
            {
                session.ChannelInactive(context);
            }
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var byteBuffer = message as IByteBuffer;
            if (byteBuffer != null)
            {
                Console.WriteLine("Received from server: " + byteBuffer.ToString(Encoding.UTF8));
            }

            context.WriteAsync(message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            context.CloseAsync();
        }
    }
}
