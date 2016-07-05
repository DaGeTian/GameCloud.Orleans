// Copyright (c) Cragon. All rights reserved.

namespace GF.Unity.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using GF.Unity.Common;
    using GF.Unity.Orleans;

    public class ClientSampleApp<TDef> : Component<TDef> where TDef : DefSampleApp, new()
    {
        //---------------------------------------------------------------------
        public ClientOrleans<DefOrleans> CoOrleans { get; private set; }

        //---------------------------------------------------------------------
        public override void init()
        {
            EbLog.Note("ClientSampleApp.init()");

            EntityMgr.getDefaultEventPublisher().addHandler(Entity);

            EntityMgr.createEntity<EtOrleans>(null, Entity);
        }

        //---------------------------------------------------------------------
        public override void release()
        {
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
    }
}
