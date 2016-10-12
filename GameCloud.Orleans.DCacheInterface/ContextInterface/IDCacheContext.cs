// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.Orleans.DCache
{
    //---------------------------------------------------------------------
    public interface IDCacheContext
    {
        int DCacheMapCount { get; set; }
        int DCacheMasterSlaveMaxDeep { get; set; }
        int DCacheMasterSlaveCount { get; set; }
        void init(int dcache_map_count, int dcache_masterslave_deep, int dcache_masterslave_count);
    }
}
