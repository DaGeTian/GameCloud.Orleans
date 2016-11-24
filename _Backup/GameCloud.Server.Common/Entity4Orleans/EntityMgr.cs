// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Server.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class EntityMgr
    {
        //---------------------------------------------------------------------
        public static Entity CreateEntityFromProtobuf<EntityDef>(byte[] data)
        {
            return null;
        }

        //---------------------------------------------------------------------
        public static Task<Entity> CreateEntityAsync<EntityDef, EntityLoader>()
        {
            Entity entity = new Entity();

            return Task.FromResult(entity);
        }
    }
}
