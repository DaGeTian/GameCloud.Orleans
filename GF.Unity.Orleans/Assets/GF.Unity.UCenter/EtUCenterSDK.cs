

namespace GF.UCenter.SDK.Unity
{
    using System;
    using System.Collections.Generic;
    using GF.Unity.Common;

    public class EtUCenterSDK : EntityDef
    {
        //-------------------------------------------------------------------------
        public override void declareAllComponent(byte node_type)
        {
            declareComponent<DefUCenterSDK>();
        }
    }
}
