// Copyright (c) Cragon. All rights reserved.

namespace GF.Unity.Orleans
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using GF.Unity.Common;

    public class ClientOrleans<TDef> : Component<TDef> where TDef : DefOrleans, new()
    {
        //---------------------------------------------------------------------

        //---------------------------------------------------------------------
        public override void init()
        {
            EbLog.Note("ClientOrleans.init()");

            EntityMgr.getDefaultEventPublisher().addHandler(Entity);

            // AutoPatcher示例
            //EntityMgr.createEntity<EtSampleAutoPatcher>(null, Entity);
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
    }
}
