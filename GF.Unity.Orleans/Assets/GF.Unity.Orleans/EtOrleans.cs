// Copyright (c) Cragon. All rights reserved.

namespace GF.Unity.Orleans
{
    using System;
    using System.Collections.Generic;
    using GF.Unity.Common;

    public class EtOrleans : EntityDef
    {
        //---------------------------------------------------------------------
        public override void declareAllComponent(byte node_type)
        {
            declareComponent<DefOrleans>();
        }
    }
}
