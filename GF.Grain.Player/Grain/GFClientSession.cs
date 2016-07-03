// Copyright (c) Cragon. All rights reserved.

namespace GF.Grain.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Orleans;
    using GF.Unity.Common;
    using GF.GrainInterface.Player;

    public class GFClientSession : IGFClientSession
    {
        //---------------------------------------------------------------------
        Task IGFClientSession.SubClient(IGFClientObserver sub)
        {
            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        Task IGFClientSession.UnsubClient(IGFClientObserver sub)
        {
            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        // 客户端请求
        Task IGFClientSession.Request(ushort method_id, byte[] data)
        {
            return TaskDone.Done;
        }
    }
}
