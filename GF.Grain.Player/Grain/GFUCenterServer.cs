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

    public class GFUCenterServer : IGFUCenterServer
    {
        //---------------------------------------------------------------------
        // 客户端请求
        Task<MethodData> IGFUCenterServer.Request(MethodData method_data)
        {
            MethodData methodData = new MethodData();
            return Task.FromResult(methodData);
        }
    }
}
