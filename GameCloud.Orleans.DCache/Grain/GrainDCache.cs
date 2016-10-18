// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.Orleans.DCache
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using System.Text;
    using global::Orleans.Concurrency;
    using global::Orleans;

    [Reentrant]
    [StatelessWorker]
    public class GrainDCache : Grain, IGrainDCache
    {
        Random Random { get; set; }
        StringBuilder SB { get; set; }
        public int DCacheMapCount { get; set; }
        public int DCacheMasterSlaveMaxDeep { get; set; }
        public int DCacheMasterSlaveCount { get; set; }
        public const string DCACHEMAP_COUNTER_TITLE = "Counter";
        public const string SLAVE_MAP_NAME = "Slave_";
        public const int SLAVE_ROOT_DEEP = 2;

        //---------------------------------------------------------------------
        public GrainDCache(IDCacheContext dcache_context)
        {
            DCacheMapCount = dcache_context.DCacheMapCount;
            DCacheMasterSlaveMaxDeep = dcache_context.DCacheMasterSlaveMaxDeep;
            DCacheMasterSlaveCount = dcache_context.DCacheMasterSlaveCount;
        }

        //---------------------------------------------------------------------
        public override Task OnActivateAsync()
        {
            DelayDeactivation(TimeSpan.FromHours(24));

            Random = new Random((int)DateTime.UtcNow.Ticks);
            SB = new StringBuilder();
            DCacheMapCount = DCacheMapCount == 0 ? 100 : DCacheMapCount;
            DCacheMasterSlaveMaxDeep = DCacheMasterSlaveMaxDeep == 0 ? 2 : DCacheMasterSlaveMaxDeep;
            DCacheMasterSlaveCount = DCacheMasterSlaveMaxDeep == 0 ? 100 : DCacheMasterSlaveMaxDeep;

            return base.OnActivateAsync();
        }

        //---------------------------------------------------------------------
        public override Task OnDeactivateAsync()
        {
            if (Random != null)
            {
                Random = null;
            }

            if (SB != null)
            {
                SB.Clear();
                SB = null;
            }

            return base.OnDeactivateAsync();
        }

        #region DCacheMap

        //---------------------------------------------------------------------
        // 初始化Map
       async Task IGrainDCache.initMap(string map_name)
        {
            SB.Clear();
            SB.Append(map_name);
            SB.Append("_");
            SB.Append(DCACHEMAP_COUNTER_TITLE);
            int count = DCacheMapCount;

            int map_index = Random.Next(1, count + 1);
            SB.Clear();
            SB.Append(map_name);
            SB.Append("_");
            SB.Append(map_index);

            var grain_maprandom = this.GrainFactory.GetGrain<IGrainDCacheMap>(SB.ToString());
            var is_inited = await grain_maprandom.GetIfInit();

            if (is_inited)
            {
                return;
            }

            var grain_mapcounter = this.GrainFactory.GetGrain<IGrainDCacheMapCounter>(SB.ToString());
            grain_mapcounter.setup(map_name, count);

            for (int i = 1; i <= count; i++)
            {
                SB.Clear();
                SB.Append(map_name);
                SB.Append("_");
                SB.Append(i);
                var grain_map = this.GrainFactory.GetGrain<IGrainDCacheMap>(SB.ToString());
                grain_map.setup();
            }            
        }

        //---------------------------------------------------------------------
        // 从Map中获取数据
        async Task<byte[]> IGrainDCache.getFromMap(string map_name, string ticket, string key)
        {
            var grain_map = this.GrainFactory.GetGrain<IGrainDCacheMap>(ticket);
            var value = await grain_map.getFromMap(key);

            return value;
        }

        //---------------------------------------------------------------------
        // 从Map中随机获取数据
        async Task<List<byte[]>> IGrainDCache.getFromMapRandom(string map_name, int count)
        {
            int map_index = Random.Next(1, DCacheMapCount + 1);
            SB.Clear();
            SB.Append(map_name);
            SB.Append("_");
            SB.Append(map_index);

            var grain_map = this.GrainFactory.GetGrain<IGrainDCacheMap>(SB.ToString());
            var list_value = await grain_map.getFromMapRandom(count);

            return list_value;
        }

        //---------------------------------------------------------------------
        // add数据到Map中
        async Task<string> IGrainDCache.addToMap(string map_name, string ticket, string key, byte[] value)
        {
            if (string.IsNullOrEmpty(ticket))
            {
                int map_index = Random.Next(1, DCacheMapCount + 1);
                SB.Clear();
                SB.Append(map_name);
                SB.Append("_");
                SB.Append(map_index);
                ticket = SB.ToString();
            }

            var grain_map = this.GrainFactory.GetGrain<IGrainDCacheMap>(ticket);
            var tick = await grain_map.addToMap(key, value);

            return tick;
        }

        //---------------------------------------------------------------------
        // remove数据
        Task IGrainDCache.removeFromMap(string map_name, string ticket, string key)
        {
            var grain_map = this.GrainFactory.GetGrain<IGrainDCacheMap>(ticket);
            grain_map.removeFromMap(key);

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        // Count 所有MapCache
        async Task<int> IGrainDCache.countMap(string map_name)
        {
            SB.Clear();
            SB.Append(map_name);
            SB.Append("_");
            SB.Append(DCACHEMAP_COUNTER_TITLE);
            var grain_mapcounter = this.GrainFactory.GetGrain<IGrainDCacheMapCounter>(SB.ToString());
            int count = await grain_mapcounter.countMap();

            return count;
        }

        #endregion

        #region DCacheMasteSlave

        //---------------------------------------------------------------------
        // 初始化MasteSlave        
        async Task IGrainDCache.initMasteSlave(string maste_name)
        {
            var grain_master = this.GrainFactory.GetGrain<IGrainDCacheMaster>(maste_name);
            bool is_inited = await grain_master.GetIfInit();

            if (is_inited)
            {
                return;
            }

            grain_master.Init();
            grain_master.setup(maste_name, DCacheMasterSlaveMaxDeep, DCacheMasterSlaveCount);

            for (int i = 1; i <= DCacheMasterSlaveCount; i++)
            {
                SB.Clear();
                SB.Append(maste_name);
                SB.Append("_");
                SB.Append(SLAVE_MAP_NAME);
                SB.Append("_");
                SB.Append(SLAVE_ROOT_DEEP);
                SB.Append("_");
                SB.Append(i);
                var grain_slave = this.GrainFactory.GetGrain<IGrainDCacheSlave>(SB.ToString());
                grain_slave.setup(maste_name, SLAVE_ROOT_DEEP, DCacheMasterSlaveMaxDeep, DCacheMasterSlaveCount);
            }
        }

        //---------------------------------------------------------------------
        // 从MasteSlave中获取数据
        async Task<byte[]> IGrainDCache.getFromMasteSlave(string maste_name, string key)
        {
            int slave_count = (int)Math.Pow(DCacheMasterSlaveCount, DCacheMasterSlaveMaxDeep - 1);
            int slave_index = Random.Next(1, slave_count + 1);
            SB.Clear();
            SB.Append(maste_name);
            SB.Append("_");
            SB.Append(SLAVE_MAP_NAME);
            SB.Append("_");
            SB.Append(DCacheMasterSlaveMaxDeep);
            SB.Append("_");
            SB.Append(slave_index);
            var grain_slave = this.GrainFactory.GetGrain<IGrainDCacheSlave>(SB.ToString());
            var result = await grain_slave.getFromSlave(key);

            return result;
        }

        //---------------------------------------------------------------------
        // add数据到MasteSlave中
        Task IGrainDCache.addToMaster(string maste_name, string key, byte[] value)
        {
            var grain_master = this.GrainFactory.GetGrain<IGrainDCacheMaster>(maste_name);
            grain_master.addToMasteSlave(key, value);

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        // remove数据
        Task IGrainDCache.removeFromMaster(string maste_name, string key)
        {
            var grain_master = this.GrainFactory.GetGrain<IGrainDCacheMaster>(maste_name);
            grain_master.removeFromMasteSlave(key);

            return TaskDone.Done;
        }

        #endregion
    }
}
