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

    public class GFPlayer : IGFPlayer
    {
        //---------------------------------------------------------------------
        Task IGFPlayer.SubPlayer(IGFPlayerObserver sub)
        {
            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        Task IGFPlayer.UnsubPlayer(IGFPlayerObserver sub)
        {
            return TaskDone.Done;
        }

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

        //---------------------------------------------------------------------
        // Client->Cell的请求
        Task<MethodData> IGFPlayer.Request(MethodData method_data)
        {
            MethodData methodData = new MethodData();
            return Task.FromResult(methodData);
        }

        //---------------------------------------------------------------------
        // Cell->Client的通知
        Task IGFPlayer.Notify(MethodData method_data)
        {
            return TaskDone.Done;
        }
    }
}
