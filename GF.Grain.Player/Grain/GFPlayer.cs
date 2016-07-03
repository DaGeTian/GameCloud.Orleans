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
        public string ClientSessionGuid { get; private set; }

        //---------------------------------------------------------------------
        // 进入游戏世界
        Task<EntityData> IGFPlayer.EnterWorld(string clientSessionGuid)
        {
            ClientSessionGuid = clientSessionGuid;

            EntityData entityData = new EntityData();
            return Task.FromResult(entityData);
        }

        //---------------------------------------------------------------------
        // 离开游戏世界
        Task IGFPlayer.LeaveWorld()
        {
            ClientSessionGuid = string.Empty;

            return TaskDone.Done;
        }
    }
}
