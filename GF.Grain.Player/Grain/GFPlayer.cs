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

    public class GFPlayer : Grain, IGFPlayer
    {
        //---------------------------------------------------------------------
        // 进入游戏世界
        Task<EntityData> IGFPlayer.EnterWorld()
        {
            EntityData entityData = new EntityData();
            return Task.FromResult(entityData);
        }

        //---------------------------------------------------------------------
        // 离开游戏世界
        Task IGFPlayer.LeaveWorld()
        {
            return TaskDone.Done;
        }
    }
}
