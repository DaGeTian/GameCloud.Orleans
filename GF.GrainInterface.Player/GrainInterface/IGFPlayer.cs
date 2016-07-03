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

    public interface IGFPlayer : IGrainWithGuidKey
    {
        //---------------------------------------------------------------------
        // 进入游戏世界
        Task<EntityData> EnterWorld(string clientSessionGuid);

        //---------------------------------------------------------------------
        // 离开游戏世界
        Task LeaveWorld();
    }
}
