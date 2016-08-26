// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Server.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Entity
    {
        //---------------------------------------------------------------------
        Entity mParent;
        Dictionary<string, Dictionary<string, Entity>> mMapChild;// key1=entity_type, key2=entity_guid

        //---------------------------------------------------------------------
        //public EntityMgr EntityMgr { get; private set; }
        public string Type { get; private set; }
        public string Guid { get; private set; }
        //public List<IComponent> ListComponent { get; private set; }
        public Entity Parent { get { return mParent; } }
        public bool SignDestroy { internal set; get; }

        //---------------------------------------------------------------------
        internal Entity()
        {
            //EntityMgr = entity_mgr;
            //ListComponent = new List<IComponent>();
        }

        //---------------------------------------------------------------------
        public byte[] ProtobufSerialize()
        {
            return new byte[0];
        }

        //---------------------------------------------------------------------
        public Task SaveAsync<EntityLoader>()
        {
            return Task.FromResult(0);
        }

        //---------------------------------------------------------------------
        public Task SaveAndCloseAsync<EntityLoader>()
        {
            return Task.FromResult(0);
        }
    }
}
