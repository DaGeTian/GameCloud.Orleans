// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.Orleans
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using System.Diagnostics;
    using System.Text;
    using global::Orleans;
    using global::Orleans.Concurrency;

    [Reentrant]
    public class GrainDCacheMaster : Grain, IGrainDCacheMaster
    {
        string MasterName { get; set; }
        int Deep { get; set; }
        int SlaveCount { get; set; }
        Dictionary<string, byte[]> MapCache { get; set; }
        StringBuilder SB { get; set; }
        bool IsInited { get; set; }
        const float UPDATE_DATA_TM = 2f;

        //---------------------------------------------------------------------
        public override Task OnActivateAsync()
        {
            DelayDeactivation(TimeSpan.FromDays(365));

            MapCache = new Dictionary<string, byte[]>();
            SB = new StringBuilder();

            return base.OnActivateAsync();
        }

        //---------------------------------------------------------------------
        public override Task OnDeactivateAsync()
        {
            if (MapCache != null)
            {
                MapCache.Clear();
                MapCache = null;
            }

            if (SB != null)
            {
                SB.Clear();
                SB = null;
            }

            return base.OnDeactivateAsync();
        }

        //---------------------------------------------------------------------
        Task IGrainDCacheMaster.Init()
        {
            if (!IsInited)
            {
                IsInited = true;
            }

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        Task<bool> IGrainDCacheMaster.GetIfInit()
        {
            return Task.FromResult(IsInited);
        }

        //---------------------------------------------------------------------
        Task IGrainDCacheMaster.setup(string master_name, int deep, int slave_count)
        {
            MasterName = master_name;
            Deep = deep;
            SlaveCount = slave_count;
            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        // add数据到MasteSlave中
        Task IGrainDCacheMaster.addToMasteSlave(string key, byte[] value)
        {
            MapCache[key] = value;

            for (int i = 1; i <= SlaveCount; i++)
            {
                SB.Clear();
                SB.Append(MasterName);
                SB.Append("_");
                SB.Append(GrainDCache.SLAVE_MAP_NAME);
                SB.Append("_");
                SB.Append(GrainDCache.SLAVE_ROOT_DEEP);
                SB.Append("_");
                SB.Append(i);
                var grain_slave = this.GrainFactory.GetGrain<IGrainDCacheSlave>(SB.ToString());
                grain_slave.addToSlave(key, value);
            }

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        // remove数据
        Task IGrainDCacheMaster.removeFromMasteSlave(string key)
        {
            if (MapCache.ContainsKey(key))
            {
                MapCache.Remove(key);
            }

            for (int i = 1; i <= SlaveCount; i++)
            {
                SB.Clear();
                SB.Append(MasterName);
                SB.Append("_");
                SB.Append(GrainDCache.SLAVE_MAP_NAME);
                SB.Append("_");
                SB.Append(GrainDCache.SLAVE_ROOT_DEEP);
                SB.Append("_");
                SB.Append(i);
                var grain_slave = this.GrainFactory.GetGrain<IGrainDCacheSlave>(SB.ToString());
                grain_slave.removeFromSlave(key);
            }

            return TaskDone.Done;
        }
    }
}
