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

    [Reentrant]
    public class GrainDCacheMapCounter : Grain, IGrainDCacheMapCounter
    {
        string KeyTitle { get; set; }
        int MapCount { get; set; }
        StringBuilder SB { get; set; }
        List<Task> ListCountTask { get; set; }
        int MapCacheDataCount { get; set; }    

        //---------------------------------------------------------------------
        public override Task OnActivateAsync()
        {
            DelayDeactivation(TimeSpan.FromDays(365));

            SB = new StringBuilder();
            ListCountTask = new List<Task>();

            return base.OnActivateAsync();
        }

        //---------------------------------------------------------------------
        public override Task OnDeactivateAsync()
        {
            if (SB != null)
            {
                SB.Clear();
                SB = null;
            }

            if (ListCountTask != null)
            {
                ListCountTask.Clear();
                ListCountTask = null;
            }

            return base.OnDeactivateAsync();
        }

        //---------------------------------------------------------------------
        Task IGrainDCacheMapCounter.setup(string map_name, int map_count)
        {
            KeyTitle = map_name;
            MapCount = map_count;
            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        // Count Cache
        async Task<int> IGrainDCacheMapCounter.countMap()
        {
            ListCountTask.Clear();

            for (int i = 1; i <= MapCount; i++)
            {
                SB.Clear();
                SB.Append(KeyTitle);
                SB.Append("_");
                SB.Append(i);
                var grain_map = this.GrainFactory.GetGrain<IGrainDCacheMap>(SB.ToString());
                ListCountTask.Add(grain_map.getMapCount());
            }

            await Task.WhenAll(ListCountTask);

            MapCacheDataCount = 0;
            foreach (var i in ListCountTask)
            {
                if (i.IsCompleted)
                {
                    MapCacheDataCount += ((Task<int>)i).Result;
                }
            }

            ListCountTask.Clear();

            return MapCacheDataCount;
        }

        //---------------------------------------------------------------------
        // 获取非空DCacheMap
        Task<List<string>> IGrainDCacheMapCounter.getNotEmptyMap()
        {
            return Task.FromResult(new List<string>());
        }
    }
}
