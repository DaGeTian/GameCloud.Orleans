// Copyright (c) Cragon. All rights reserved.

namespace GF.Unity.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using GF.Unity.Common;
    using GF.Unity.Orleans;
    using GF.GrainCommon.Player;

    public class ClientSampleApp<TDef> : Component<TDef> where TDef : DefSampleApp, new()
    {
        //---------------------------------------------------------------------
        public ClientOrleans<DefOrleans> CoOrleans { get; private set; }

        //---------------------------------------------------------------------
        public override void init()
        {
            EbLog.Note("ClientSampleApp.init()");

            EntityMgr.getDefaultEventPublisher().addHandler(Entity);

            var et_orleans = EntityMgr.createEntity<EtOrleans>(null, Entity);
            CoOrleans = et_orleans.getComponent<ClientOrleans<DefOrleans>>();

            DefaultRpcSession.OnSocketConnected = OnSocketConnected;
            DefaultRpcSession.OnSocketClosed = OnSocketClosed;
            DefaultRpcSession.OnSocketError = OnSocketError;
            DefaultRpcSession.connect("192.168.0.10", 5882);
        }

        //---------------------------------------------------------------------
        public override void release()
        {
            DefaultRpcSession.close();

            EbLog.Note("ClientSampleApp.release()");
        }

        //---------------------------------------------------------------------
        public override void update(float elapsed_tm)
        {
        }

        //---------------------------------------------------------------------
        public override void handleEvent(object sender, EntityEvent e)
        {
        }

        //---------------------------------------------------------------------
        void OnSocketError(object rec, SocketErrorEventArgs args)
        {
        }

        //---------------------------------------------------------------------
        void OnSocketConnected(object client, EventArgs args)
        {
            GFEnterWorldRequest enterworld_request = new GFEnterWorldRequest();
            enterworld_request.acc_id = "";
            enterworld_request.acc_name = "test";
            enterworld_request.et_player_guid = "";
            enterworld_request.nick_name = "";
            enterworld_request.token = "";
            CoOrleans.RequestEnterWorld(enterworld_request);
        }

        //---------------------------------------------------------------------
        void OnSocketClosed(object client, EventArgs args)
        {
        }
    }
}
