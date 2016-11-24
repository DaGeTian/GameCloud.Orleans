// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.Orleans
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using global::Orleans;
    using global::Orleans.Concurrency;

    [Reentrant]
    public class GrainDCacheMap : Grain, IGrainDCacheMap
    {
        Dictionary<string, byte[]> MapCache { get; set; }
        List<byte[]> ListCache { get; set; }
        List<byte[]> ListCacheRandom { get; set; }
        string DCacheMapKey { get; set; }
        Random Random { get; set; }
        bool IsInited { get; set; }

        //---------------------------------------------------------------------
        public override Task OnActivateAsync()
        {
            DelayDeactivation(TimeSpan.FromDays(365));

            MapCache = new Dictionary<string, byte[]>();
            ListCache = new List<byte[]>();
            ListCacheRandom = new List<byte[]>();
            DCacheMapKey = this.GetPrimaryKeyString();
            Random = new Random((int)DateTime.UtcNow.Ticks);

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

            if (Random != null)
            {
                Random = null;
            }

            return base.OnDeactivateAsync();
        }

        //---------------------------------------------------------------------
        Task IGrainDCacheMap.Init()
        {
            if (!IsInited)
            {
                IsInited = true;
            }

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        Task<bool> IGrainDCacheMap.GetIfInit()
        {
            return Task.FromResult(IsInited);
        }

        //---------------------------------------------------------------------
        Task IGrainDCacheMap.setup()
        {
            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        // 从Map中获取数据
        Task<byte[]> IGrainDCacheMap.getFromMap(string key)
        {
            byte[] values = null;

            MapCache.TryGetValue(key, out values);

            return Task.FromResult(values);
        }

        //---------------------------------------------------------------------
        // 从Map中随机获取数据
        Task<List<byte[]>> IGrainDCacheMap.getFromMapRandom(int count)
        {
            ListCache.Clear();
            ListCacheRandom.Clear();
            ListCache.AddRange(MapCache.Values);

            if (ListCache.Count > count)
            {
                for (int i = 0; i < count; i++)
                {
                    ListCacheRandom.Add(ListCache[Random.Next(0, ListCache.Count)]);
                }
            }
            else
            {
                ListCacheRandom.AddRange(ListCache);
            }

            ListCache.Clear();

            return Task.FromResult(ListCacheRandom);
        }

        //---------------------------------------------------------------------
        // 从Map中获取数据长度
        Task<int> IGrainDCacheMap.getMapCount()
        {
            return Task.FromResult(MapCache.Count);
        }

        //---------------------------------------------------------------------
        // add数据到Map中
        Task<string> IGrainDCacheMap.addToMap(string key, byte[] value)
        {
            MapCache[key] = value;

            return Task.FromResult(DCacheMapKey);
        }

        //---------------------------------------------------------------------
        // remove数据
        Task IGrainDCacheMap.removeFromMap(string key)
        {
            MapCache.Remove(key);

            return TaskDone.Done;
        }
    }
}
