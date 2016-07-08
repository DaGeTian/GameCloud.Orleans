// Copyright (c) Cragon. All rights reserved.

namespace GF.Orleans.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;
    using GF.Unity.Common;
    using global::Orleans;

    public class GatewaySession : RpcSession
    {
        //---------------------------------------------------------------------
        private IChannelHandlerContext context;
        private IGatewaySessionListener listener = Gateway.Instance.GatewaySessionListenerContainer.Resolve<IGatewaySessionListener>();
        //private IGFClientObserver clientObserver;
        //private IGFClientObserver clientObserverWeak;
        //private Guid clientGuid;

        //---------------------------------------------------------------------
        public GatewaySession(EntityMgr entity_mgr)
        {
        }

        //---------------------------------------------------------------------
        public async void ChannelActive(IChannelHandlerContext context)
        {
            this.context = context;

            Console.WriteLine("GatewaySession.ChannelActive() Name=" + context.Name);

            var task = await Task.Factory.StartNew<Task>(async () =>
            {
                await this.listener.OnSessionCreate();

                //    this.clientObserver = new GatewayClientObserver(this);
                //    this.clientObserverWeak = await GrainClient.GrainFactory.CreateObjectReference<IGFClientObserver>(this.clientObserver);

                //    this.clientGuid = Guid.NewGuid();
                //    var grain_clientsession = GrainClient.GrainFactory.GetGrain<IGFClientSession>(this.clientGuid);
                //    await grain_clientsession.SubClient(this.clientObserverWeak);
            });
        }

        //---------------------------------------------------------------------
        public async void ChannelInactive(IChannelHandlerContext context)
        {
            var task = await Task.Factory.StartNew<Task>(async () =>
            {
                await this.listener.OnSessionDestroy();

                //    var grain_clientsession = GrainClient.GrainFactory.GetGrain<IGFClientSession>(this.clientGuid);
                //    await grain_clientsession.UnsubClient(this.clientObserverWeak);

                //    this.context = null;
                //    this.clientObserver = null;
                //    this.clientObserverWeak = null;

                //    Console.WriteLine("GatewaySession.ChannelInactive() Name=" + context.Name);
            });
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

            Console.WriteLine("GatewaySession.onRecvData() MethodId=" + method_id);

            Task.Factory.StartNew(() =>
            {
                // 收到Client请求数据，转发给Orleans Server
                this.listener.Unity2Orleans(method_id, data);
                //var grain_clientsession = GrainClient.GrainFactory.GetGrain<IGFClientSession>(this.clientGuid);
                //grain_clientsession.Request(method_id, data);
            });
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
