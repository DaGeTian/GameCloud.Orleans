// Copyright (c) Cragon. All rights reserved.

namespace GF.Unity.Orleans
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using GF.Unity.Common;
    using GF.UCenter.SDK.Unity;
    using GF.GrainCommon.Player;

    public class ClientOrleans<TDef> : Component<TDef> where TDef : DefOrleans, new()
    {
        //---------------------------------------------------------------------
        public ClientUCenterSDK<DefUCenterSDK> CoUCenter { get; private set; }

        //---------------------------------------------------------------------
        public override void init()
        {
            EbLog.Note("ClientOrleans.init()");

            EntityMgr.getDefaultEventPublisher().addHandler(Entity);

            DefaultRpcSession.defRpcMethod<GFResponse>(
                (ushort)GFMethodType.S2CGFResponse, S2CGFResponse);
            DefaultRpcSession.defRpcMethod<GFNotify>(
                (ushort)GFMethodType.S2CGFNotify, S2CGFNotify);

            EntityMgr.regComponent<ClientUCenterSDK<DefUCenterSDK>>();
            EntityMgr.regEntityDef<EtUCenterSDK>();

            var et_ucenter = EntityMgr.createEntity<EtUCenterSDK>(null, Entity);
            CoUCenter = et_ucenter.getComponent<ClientUCenterSDK<DefUCenterSDK>>();
        }

        //---------------------------------------------------------------------
        public override void release()
        {
            EbLog.Note("ClientOrleans.release()");
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
        public void RequestEnterWorld(GFEnterWorldRequest enterworld_request)
        {
            GFRequest gf_request;
            gf_request.id = GFRequestId.EnterWorld;
            gf_request.data = EbTool.protobufSerialize(enterworld_request);
            DefaultRpcSession.rpc((ushort)GFMethodType.C2SGFRequest, gf_request);
        }

        //---------------------------------------------------------------------
        void S2CGFResponse(GFResponse gf_response)
        {
        }

        //---------------------------------------------------------------------
        void S2CGFNotify(GFNotify gf_notify)
        {
        }
    }
}
