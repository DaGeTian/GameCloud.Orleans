// Copyright (c) Cragon. All rights reserved.

namespace GF.GrainInterface.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Orleans;

    public interface IGFClientObserver : IGrainObserver
    {
        //---------------------------------------------------------------------
        void Notify(ushort method_id, byte[] data);
    }
}
