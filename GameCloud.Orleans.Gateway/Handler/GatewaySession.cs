// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Orleans.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;
    using global::Orleans;
    using GameCloud.Unity.Common;

    public class GatewaySession : RpcSession
    {
        //---------------------------------------------------------------------
        private IChannelHandlerContext context;
        private IGatewaySessionListener listener =
            Gateway.Instance.GatewaySessionListenerContainer.Resolve<IGatewaySessionListener>();

        //---------------------------------------------------------------------
        public GatewaySession(EntityMgr entity_mgr)
        {
        }

        //---------------------------------------------------------------------
        public async void ChannelActive(IChannelHandlerContext context)
        {
            this.context = context;
            listener.GatewaySession = this;

            await this.listener.OnSessionCreate();
        }

        //---------------------------------------------------------------------
        public async void ChannelInactive(IChannelHandlerContext context)
        {
            await this.listener.OnSessionDestroy();
        }

        //---------------------------------------------------------------------
        public override bool isConnect()
        {
            return true;
        }

        //---------------------------------------------------------------------
        public override void connect(string ip, int port)
        {
        }

        //---------------------------------------------------------------------
        public override void send(ushort method_id, byte[] data)
        {
            IByteBuffer msg = PooledByteBufferAllocator.Default.Buffer(256);
            msg.WriteBytes(BitConverter.GetBytes(method_id));
            if (data != null) msg.WriteBytes(data);

            this.context.WriteAndFlushAsync(msg);
        }

        //---------------------------------------------------------------------
        public override void onRecv(ushort method_id, byte[] data)
        {
        }

        //---------------------------------------------------------------------
        public override void close()
        {
            this.context.CloseAsync();
        }

        //---------------------------------------------------------------------
        public override void update(float elapsed_tm)
        {
        }

        //---------------------------------------------------------------------
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

            // 收到Client请求数据，转发给Orleans Server
            this.listener.Unity2Orleans(method_id, buf);
        }

        //---------------------------------------------------------------------
        public void OnOrleansNotify(ushort method_id, byte[] data)
        {
            // 收到Orleans Server的推送数据，转发给Client
            this.send(method_id, data);
        }
    }

    public class GatewaySessionFactory : RpcSessionFactory
    {
        //---------------------------------------------------------------------
        public override RpcSession createRpcSession(EntityMgr entity_mgr)
        {
            return new GatewaySession(entity_mgr);
        }
    }
}
