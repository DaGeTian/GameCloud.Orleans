// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Entity
{
    using System;
    using System.Collections.Generic;

    public abstract class EntityDef
    {
        //---------------------------------------------------------------------
        Dictionary<string, object> mMapComponentData = new Dictionary<string, object>();

        //---------------------------------------------------------------------
        public Dictionary<string, object> MapComponentData { get { return mMapComponentData; } }

        //---------------------------------------------------------------------
        public void setupComponentData<TData>() where TData : IComponentData, new()
        {
            TData data = new TData();
            string name = typeof(TData).Name;
            mMapComponentData[name] = data;
        }
    }
}
