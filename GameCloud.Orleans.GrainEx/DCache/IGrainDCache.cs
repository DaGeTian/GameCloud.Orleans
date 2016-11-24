// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.Orleans
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text;    
    using global::Orleans;

    public interface IGrainDCache : IGrainWithIntegerKey
    {
        #region DCacheMap

        //---------------------------------------------------------------------
        // 初始化Map
        Task initMap(string map_name);

        //---------------------------------------------------------------------
        // 从Map中获取数据
        Task<byte[]> getFromMap(string map_name, string ticket, string key);

        //---------------------------------------------------------------------
        // 从Map中随机获取数据
        Task<List<byte[]>> getFromMapRandom(string map_name, int count);

        //---------------------------------------------------------------------
        // add数据到Map中
        Task<string> addToMap(string map_name, string ticket, string key, byte[] value);

        //---------------------------------------------------------------------
        // remove数据
        Task removeFromMap(string map_name, string ticket, string key);

        //---------------------------------------------------------------------
        // Count 所有MapCache
        Task<int> countMap(string map_name);

        #endregion

        #region DCacheMasteSlave

        //---------------------------------------------------------------------
        // 初始化MasteSlave
        Task initMasteSlave(string maste_name);

        //---------------------------------------------------------------------
        // 从MasteSlave中获取数据
        Task<byte[]> getFromMasteSlave(string maste_name, string key);

        //---------------------------------------------------------------------
        // add数据到MasteSlave中
        Task addToMaster(string maste_name, string key, byte[] value);

        //---------------------------------------------------------------------
        // remove数据
        Task removeFromMaster(string maste_name, string key);

        #endregion
    }
}
