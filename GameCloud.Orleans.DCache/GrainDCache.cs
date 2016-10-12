// Copyright(c) Cragon. All rights reserved.

namespace TexasPoker
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Orleans;
    using Orleans.Concurrency;
    using System;
    using System.Text;
    using System.Configuration;

    [Reentrant]
    [StatelessWorker]
    public class GrainDCache : Grain, IGrainDCache
    {
        Random Random { get; set; }
        StringBuilder SB { get; set; }
        public int DcacheMapCount { get; set; }
        public int DcacheMasterSlaveMaxDeep { get; set; }
        public int DcacheMasterSlaveCount { get; set; }
        public const string DCACHEMAP_COUNTER_TITLE = "Counter";
        public const string SLAVE_MAP_NAME = "Slave_";
        public const int SLAVE_ROOT_DEEP = 2;

        //---------------------------------------------------------------------
        public override Task OnActivateAsync()
        {
            DelayDeactivation(TimeSpan.FromHours(24));

            Random = new Random((int)DateTime.UtcNow.Ticks);
            SB = new StringBuilder();
            DcacheMapCount = 100;
            DcacheMasterSlaveMaxDeep = 2;
            DcacheMasterSlaveCount = 100;

            ExeConfigurationFileMap file = new ExeConfigurationFileMap();
            file.ExeConfigFilename = "GameCloud.Orleans.DCache.config";
            Configuration cfg = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            try
            {
                DcacheMapCount = int.Parse(cfg.AppSettings.Settings["DCACHEMAPCOUNT"].Value);
                DcacheMasterSlaveMaxDeep = int.Parse(cfg.AppSettings.Settings["DCACHEMASTERSLAVEMAXDEEP"].Value);
                DcacheMasterSlaveCount = int.Parse(cfg.AppSettings.Settings["DCACHEMASTERSLAVECOUNT"].Value);
            }
            catch (Exception e)
            {

            }

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
        Task IGrainDCache.initMap(string map_name)
        {
            SB.Clear();
            SB.Append(map_name);
            SB.Append("_");
            SB.Append(DCACHEMAP_COUNTER_TITLE);
            int count = DcacheMapCount;
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

            return TaskDone.Done;
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
            int map_index = Random.Next(1, DcacheMapCount + 1);
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
                int map_index = Random.Next(1, DcacheMapCount + 1);
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
        Task IGrainDCache.initMasteSlave(string maste_name)
        {
            var grain_master = this.GrainFactory.GetGrain<IGrainDCacheMaster>(maste_name);
            grain_master.setup(maste_name, DcacheMasterSlaveMaxDeep, DcacheMasterSlaveCount);

            for (int i = 1; i <= DcacheMasterSlaveCount; i++)
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
                grain_slave.setup(maste_name, SLAVE_ROOT_DEEP, DcacheMasterSlaveMaxDeep, DcacheMasterSlaveCount);
            }

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        // 从MasteSlave中获取数据
        async Task<byte[]> IGrainDCache.getFromMasteSlave(string maste_name, string key)
        {
            int slave_count = (int)Math.Pow(DcacheMasterSlaveCount, DcacheMasterSlaveMaxDeep - 1);
            int slave_index = Random.Next(1, slave_count + 1);
            SB.Clear();
            SB.Append(maste_name);
            SB.Append("_");
            SB.Append(SLAVE_MAP_NAME);
            SB.Append("_");
            SB.Append(DcacheMasterSlaveMaxDeep);
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
