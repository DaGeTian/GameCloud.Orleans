// Copyright(c) Cragon. All rights reserved.

namespace TexasPoker
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Orleans;
    using Orleans.Concurrency;
    using System;
    using System.Diagnostics;
    using System.Text;

    public class GrainDCacheSlave : Grain, IGrainDCacheSlave
    {
        int CurDeep { get; set; }
        int Count { get; set; }
        string MasterName { get; set; }
        bool HaveChild { get; set; }
        Dictionary<string, byte[]> MapCache { get; set; }
        StringBuilder SB { get; set; }

        //---------------------------------------------------------------------
        public override Task OnActivateAsync()
        {
            DelayDeactivation(TimeSpan.FromDays(365));

            MapCache = new Dictionary<string, byte[]>();
            SB = new StringBuilder();
            HaveChild = false;

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
        Task IGrainDCacheSlave.setup(string master_name, int cur_deep, int max_deep, int count)
        {
            MasterName = master_name;
            CurDeep = cur_deep;
            Count = count;
            int left_deep = max_deep - CurDeep;

            if (left_deep > 0)
            {
                HaveChild = true;

                cur_deep = CurDeep + 1;
                for (int i = 1; i <= count; i++)
                {
                    var grain_slave = this.GrainFactory.GetGrain<IGrainDCacheSlave>(GrainDCache.SLAVE_MAP_NAME + cur_deep + "_" + i);
                    grain_slave.setup(MasterName, cur_deep, max_deep, count);
                }
            }

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        // 从MasteSlave中获取数据
        Task<byte[]> IGrainDCacheSlave.getFromSlave(string key)
        {
            byte[] values = null;

            MapCache.TryGetValue(key, out values);

            return Task.FromResult(values);
        }

        //---------------------------------------------------------------------
        // add数据到MasteSlave中
        Task IGrainDCacheSlave.addToSlave(string key, byte[] value)
        {
            if (!HaveChild)
            {
                MapCache[key] = value;
            }
            else
            {
                for (int i = 1; i <= Count; i++)
                {
                    SB.Clear();
                    SB.Append(MasterName);
                    SB.Append("_");
                    SB.Append(GrainDCache.SLAVE_MAP_NAME);
                    SB.Append("_");
                    SB.Append(CurDeep + 1);
                    SB.Append("_");
                    SB.Append(i);
                    var grain_slave = this.GrainFactory.GetGrain<IGrainDCacheSlave>(SB.ToString());
                    grain_slave.addToSlave(key, value);
                }
            }

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        // remove数据
        Task IGrainDCacheSlave.removeFromSlave(string key)
        {
            if (!HaveChild)
            {
                MapCache.Remove(key);
            }
            else
            {
                for (int i = 1; i <= Count; i++)
                {
                    SB.Clear();
                    SB.Append(MasterName);
                    SB.Append("_");
                    SB.Append(GrainDCache.SLAVE_MAP_NAME);
                    SB.Append("_");
                    SB.Append(CurDeep + 1);
                    SB.Append("_");
                    SB.Append(i);
                    var grain_slave = this.GrainFactory.GetGrain<IGrainDCacheSlave>(SB.ToString());
                    grain_slave.removeFromSlave(key);
                }
            }

            return TaskDone.Done;
        }
    }
}
