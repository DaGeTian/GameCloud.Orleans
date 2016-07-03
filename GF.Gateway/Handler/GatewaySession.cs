// Copyright (c) Cragon. All rights reserved.

namespace GF.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;
    using GF.Unity.Common;

    public class GatewaySession : RpcSession
    {
        private IChannelHandlerContext context;

        public GatewaySession(EntityMgr entity_mgr)
        {
        }

        public void ChannelActive(IChannelHandlerContext context)
        {
            this.context = context;

            Console.WriteLine("GatewaySession.ChannelActive() Name=" + context.Name);
        }

        public void ChannelInactive(IChannelHandlerContext context)
        {
            Console.WriteLine("GatewaySession.ChannelInactive() Name=" + context.Name);
        }

        public override bool isConnect()
        {
            return true;
        }

        public override void connect(string ip, int port)
        {
        }

        public override void send(ushort method_id, byte[] data)
        {
            IByteBuffer msg = PooledByteBufferAllocator.Default.Buffer(256);
            msg.WriteBytes(BitConverter.GetBytes(method_id));
            if (data != null) msg.WriteBytes(data);

            context.WriteAndFlushAsync(msg);
        }

        public override void onRecv(ushort method_id, byte[] data)
        {
            Console.WriteLine("GatewaySession.OnRecvData() MethodId=" + method_id);
        }

        public override void close()
        {
            context.CloseAsync();
        }

        public override void update(float elapsed_tm)
        {
        }

        public void onRecvData(byte[] data)
        {
            ushort method_id = BitConverter.ToUInt16(data, 0);

            byte[] buf = null;
            if (data.Length > sizeof(ushort))
            {
                ushort data_len = (ushort)(data.Length - sizeof(ushort));
                buf = new byte[data_len];
                Array.Copy(data, sizeof(ushort), buf, 0, data_len);
            }
            else buf = new byte[0];

            onRpcMethod(method_id, buf);
        }
    }

    public class GatewaySessionFactory : RpcSessionFactory
    {
        public override RpcSession createRpcSession(EntityMgr entity_mgr)
        {
            return new GatewaySession(entity_mgr);
        }
    }
}
