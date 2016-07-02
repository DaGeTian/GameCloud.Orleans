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

    [Serializable]
    public class MethodData
    {
        public short method_id;
        public byte[] param1;
        public byte[] param2;
        public byte[] param3;
        public byte[] param4;
    }

    public interface IGFPlayer : IGrainWithGuidKey
    {
        //---------------------------------------------------------------------
        Task SubPlayer(IGFPlayerObserver sub);

        //---------------------------------------------------------------------
        Task UnsubPlayer(IGFPlayerObserver sub);

        //---------------------------------------------------------------------
        // 进入游戏世界
        Task<EntityData> EnterWorld();

        //---------------------------------------------------------------------
        // 离开游戏世界
        Task LeaveWorld();

        //---------------------------------------------------------------------
        // Client->Cell的请求
        Task<MethodData> Request(MethodData method_data);

        //---------------------------------------------------------------------
        // Cell->Client的通知
        Task Notify(MethodData method_data);
    }
}
