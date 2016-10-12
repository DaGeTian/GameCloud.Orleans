// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.Orleans.DCache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text;    
    using global::Orleans;

    public interface IGrainDCacheMaster : IGrainWithStringKey
    {
        //---------------------------------------------------------------------
        Task setup(string master_name, int deep, int slave_count);

        //---------------------------------------------------------------------
        // add数据到MasteSlave中
        Task addToMasteSlave(string key, byte[] value);

        //---------------------------------------------------------------------
        // remove数据
        Task removeFromMasteSlave(string key);        
    }
}
