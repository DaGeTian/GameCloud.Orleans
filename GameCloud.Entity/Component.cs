// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Entity
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    //public abstract class IComponentData
    //{
    //}

    public abstract class IComponent
    {
        //---------------------------------------------------------------------
        public Entity Entity { internal set; get; }
        public EntityMgr EntityMgr { internal set; get; }
        //public EntityEventPublisher Publisher { get { return Entity.Publisher; } }
        //public RpcSession DefaultRpcSession { get { return EntityMgr.Instance.DefaultRpcSession; } }
        public bool EnableUpdate { set; get; }
        public bool EnableSave2Db { set; get; }
        public bool EnableNetSync { get; set; }

        //---------------------------------------------------------------------
        public IComponent()
        {
            EnableUpdate = true;
            EnableSave2Db = true;
            EnableNetSync = true;
        }

        //---------------------------------------------------------------------
        public abstract void awake();

        //---------------------------------------------------------------------
        public abstract void init();

        //---------------------------------------------------------------------
        public abstract void release();

        //---------------------------------------------------------------------
        public abstract void update(float elapsed_tm);

        //---------------------------------------------------------------------
        public abstract void handleEvent(object sender, EntityEvent e);

        //---------------------------------------------------------------------
        public abstract void onChildInit(Entity child);
    }

    public class Component<TData> : IComponent where TData : class
    {
        //---------------------------------------------------------------------
        public TData Data { get; set; }

        //---------------------------------------------------------------------
        public override void awake()
        {
        }

        //---------------------------------------------------------------------
        public override void init()
        {
        }

        //---------------------------------------------------------------------
        public override void release()
        {
        }

        //---------------------------------------------------------------------
        public override void update(float elapsed_tm)
        {
        }

        //---------------------------------------------------------------------
        public override void handleEvent(object sender, EntityEvent e)
        {
        }

        //---------------------------------------------------------------------
        public override void onChildInit(Entity child)
        {
        }
    }
}
