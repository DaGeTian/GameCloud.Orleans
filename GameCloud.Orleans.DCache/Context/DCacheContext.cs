// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.Orleans.DCache
{
    //-------------------------------------------------------------------------
    public class DCacheContext : IDCacheContext
    {
        public int DCacheMapCount { get; set; }
        public int DCacheMasterSlaveMaxDeep { get; set; }
        public int DCacheMasterSlaveCount { get; set; }

        //-------------------------------------------------------------------------
        public DCacheContext()
        {
        }

        //-------------------------------------------------------------------------
        public void init(int dcache_map_count, int dcache_masterslave_deep, int dcache_masterslave_count)
        {
            DCacheMapCount = dcache_map_count;
            DCacheMasterSlaveMaxDeep = dcache_masterslave_deep;
            DCacheMasterSlaveCount = dcache_masterslave_count;
        }
    }
}
