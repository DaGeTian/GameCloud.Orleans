// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.Orleans
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text;    
    using global::Orleans;

    public interface IGrainDCacheMapCounter : IGrainWithStringKey
    {
        //---------------------------------------------------------------------
        Task setup(string map_name, int map_count);
        
        //---------------------------------------------------------------------
        // Count Cache
        Task<int> countMap();

        //---------------------------------------------------------------------
        // 获取非空DCacheMap
        Task<List<string>> getNotEmptyMap();
    }
}
