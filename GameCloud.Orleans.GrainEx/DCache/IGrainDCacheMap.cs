// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.Orleans
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text;    
    using global::Orleans;

    public interface IGrainDCacheMap : IGrainWithStringKey
    {
        //---------------------------------------------------------------------
        Task Init();

        //---------------------------------------------------------------------
        Task<bool> GetIfInit();

        //---------------------------------------------------------------------
        Task setup();

        //---------------------------------------------------------------------
        // 从Map中获取数据
        Task<byte[]> getFromMap(string key);

        //---------------------------------------------------------------------
        // 从Map中随机获取数据
        Task<List<byte[]>> getFromMapRandom(int count);

        //---------------------------------------------------------------------
        // 从Map中获取数据长度
        Task<int> getMapCount();

        //---------------------------------------------------------------------
        // add数据到Map中
        Task<string> addToMap(string key, byte[] value);

        //---------------------------------------------------------------------
        // remove数据
        Task removeFromMap(string key);
    }
}
