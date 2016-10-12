// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.Orleans.DCache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text;    
    using global::Orleans;

    public interface IGrainDCacheSlave : IGrainWithStringKey
    {
        //---------------------------------------------------------------------
        Task setup(string master_name, int cur_deep, int max_deep, int count);

        //---------------------------------------------------------------------
        // 从MasteSlave中获取数据
        Task<byte[]> getFromSlave(string key);

        //---------------------------------------------------------------------
        // add数据到MasteSlave中
        Task addToSlave(string key, byte[] value);

        //---------------------------------------------------------------------
        // remove数据
        Task removeFromSlave(string key);        
    }
}
