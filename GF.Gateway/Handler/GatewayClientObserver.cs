// Copyright (c) Cragon. All rights reserved.

namespace GF.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GF.GrainInterface.Player;

    public class GatewayClientObserver : IGFClientObserver
    {
        //-------------------------------------------------------------------------
        void IGFClientObserver.Notify(ushort method_id, byte[] data)
        {
        }
    }
}
