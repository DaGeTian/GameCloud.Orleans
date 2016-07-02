// Copyright (c) Cragon. All rights reserved.

namespace GF.GrainInterface.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Orleans;
    using GF.Unity.Common;

    public interface IGFUCenterServer : IGrainWithIntegerKey
    {
        //---------------------------------------------------------------------
        // 客户端请求
        Task<MethodData> Request(MethodData method_data);
    }
}
