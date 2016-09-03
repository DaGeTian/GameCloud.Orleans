// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Entity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class Entity
    {
        //---------------------------------------------------------------------
        Entity mParent;
        Dictionary<string, Dictionary<string, Entity>> mMapChild;// key1=entity_type, key2=entity_guid
        Dictionary<string, IComponent> mMapComponent = new Dictionary<string, IComponent>();

        //---------------------------------------------------------------------
        public EntityMgr EntityMgr { get; private set; }
        public string Type { get; private set; }
        public string Guid { get; private set; }
        public List<IComponent> ListComponent { get; private set; }
        public Entity Parent { get { return mParent; } }
        public bool SignDestroy { internal set; get; }
        
        //---------------------------------------------------------------------
        public Entity(EntityMgr entity_mgr)
        {
            EntityMgr = entity_mgr;
            ListComponent = new List<IComponent>();
        }
        
        //---------------------------------------------------------------------
        public TData getComponentData<TData>() where TData : IComponentData, new()
        {
            string type_name = typeof(TData).Name;
            IComponent co = null;
            if (mMapComponent.TryGetValue(type_name, out co))
            {
                return (TData)((Component<TData>)co).Data;
            }
            else
            {
                return default(TData);
            }
        }

        //---------------------------------------------------------------------
        public T getComponent<T>() where T : IComponent
        {
            string type_name = "";// EntityMgr.getComponentName<T>();

            IComponent co = null;
            if (mMapComponent.TryGetValue(type_name, out co))
            {
                return (T)co;
            }
            else
            {
                return default(T);
            }
        }

        //---------------------------------------------------------------------
        public IComponent getComponent(string type)
        {
            IComponent co = null;
            mMapComponent.TryGetValue(type, out co);
            return co;
        }

        ////---------------------------------------------------------------------
        //public void setUserData<T>(T data)
        //{
        //    if (mMapCacheData == null)
        //    {
        //        mMapCacheData = new Dictionary<string, object>();
        //    }

        //    mMapCacheData[data.GetType().Name] = data;
        //}

        ////---------------------------------------------------------------------
        //public T getUserData<T>()
        //{
        //    string key = typeof(T).Name;
        //    if (mMapCacheData == null || !mMapCacheData.ContainsKey(key)) return default(T);
        //    else return (T)mMapCacheData[key];
        //}

        ////---------------------------------------------------------------------
        //public void setCacheData(string key, object v)
        //{
        //    if (mMapCacheData == null)
        //    {
        //        mMapCacheData = new Dictionary<string, object>();
        //    }

        //    mMapCacheData[key] = v;
        //}

        ////---------------------------------------------------------------------
        //public object getCacheData(string key)
        //{
        //    if (mMapCacheData == null || !mMapCacheData.ContainsKey(key)) return null;
        //    else return mMapCacheData[key];
        //}

        ////---------------------------------------------------------------------
        //public bool hasCacheData(string key)
        //{
        //    return mMapCacheData.ContainsKey(key);
        //}

        //---------------------------------------------------------------------
        public void setParent(Entity parent)
        {
            mParent = parent;
            mParent._addChild(this);
        }

        //---------------------------------------------------------------------
        public void removeChild(Entity child)
        {
            if (mMapChild == null) return;

            Dictionary<string, Entity> m = null;
            if (mMapChild.TryGetValue(child.Type, out m))
            {
                m.Remove(child.Guid);
            }
        }

        //---------------------------------------------------------------------
        public Dictionary<string, Entity> getChildrenByType(string et_type)
        {
            if (mMapChild == null) return null;

            Dictionary<string, Entity> m = null;
            mMapChild.TryGetValue(et_type, out m);
            return m;
        }

        //---------------------------------------------------------------------
        public Entity getChild(string et_type, string et_guid)
        {
            if (mMapChild == null) return null;

            Entity et = null;
            Dictionary<string, Entity> m = null;
            if (mMapChild.TryGetValue(et_type, out m))
            {
                m.TryGetValue(et_guid, out et);
            }
            return et;
        }

        //---------------------------------------------------------------------
        public Dictionary<string, Dictionary<string, Entity>> getChildren()
        {
            return mMapChild;
        }

        //---------------------------------------------------------------------
        public void update(float elapsed_tm)
        {
            _update(elapsed_tm);
        }

        //---------------------------------------------------------------------
        public void close()
        {
            _destroy();
        }

        //---------------------------------------------------------------------
        //public void _reset(EntityData entity_data)
        //{
        //    SignDestroy = false;
        //    Type = entity_data.entity_type;
        //    Guid = entity_data.entity_guid;
        //    //mMapCacheData = entity_data.cache_data;

        //    //mPublisher = new EntityEventPublisher(EntityMgr);
        //    //mPublisher.addHandler(this);

        //    EntityDef entity_def = EntityMgr.getEntityDef(entity_data.entity_type);
        //    if (entity_def == null) return;

        //    foreach (var i in entity_def.ListComponentDef)
        //    {
        //        IComponentFactory component_factory = EntityMgr.getComponentFactory(i);
        //        if (component_factory == null)
        //        {
        //            //EbLog.Error("Entity.addComponent() failed! can't find component_factory, component=" + i);
        //            continue;
        //        }

        //        Dictionary<string, string> def_propset = null;
        //        if (entity_data.map_component != null)
        //        {
        //            entity_data.map_component.TryGetValue(i, out def_propset);
        //        }

        //        var component = component_factory.createComponent(this, def_propset);
        //        mMapComponent[i] = component;
        //        ListComponent.Add(component);
        //        component.awake();
        //    }
        //}

        //---------------------------------------------------------------------
        public void _initAllComponent()
        {
            foreach (var i in ListComponent)
            {
                i.init();
            }
        }

        //---------------------------------------------------------------------
        public void _releaseAllComponent()
        {
            ListComponent.Reverse();
            foreach (var i in ListComponent)
            {
                i.release();
                i.Entity = null;
                i.EntityMgr = null;
            }
            ListComponent.Reverse();
        }

        //---------------------------------------------------------------------
        // 直接销毁该Entity
        internal void _destroy()
        {
            if (SignDestroy) return;
            SignDestroy = true;

            // 先销毁所有子Entity
            if (mMapChild != null)
            {
                Dictionary<string, Dictionary<string, Entity>> map_children =
                    new Dictionary<string, Dictionary<string, Entity>>(mMapChild);
                foreach (var i in map_children)
                {
                    List<string> list_entity = new List<string>(i.Value.Keys);
                    foreach (var j in list_entity)
                    {
                        EntityMgr.destroyEntity(j);
                    }
                }
                map_children.Clear();
            }

            // 销毁Entity上挂接的所有组件
            ListComponent.Reverse();
            foreach (var i in ListComponent)
            {
                //if (!EbTool.isNull(i))
                {
                    i.release();
                    i.Entity = null;
                    i.EntityMgr = null;
                }
            }
            ListComponent.Clear();
            mMapComponent.Clear();

            //if (mPublisher != null)
            //{
            //    mPublisher.removeHandler(this);
            //}

            //if (mMapCacheData != null)
            //{
            //    mMapCacheData.Clear();
            //}

            if (mMapChild != null)
            {
                mMapChild.Clear();
                mMapChild = null;
            }

            // 从父Entity中移除
            if (Parent != null)
            {
                Parent.removeChild(this);
            }

            Type = "";
            Guid = "";
            mParent = null;
        }

        //---------------------------------------------------------------------
        internal void _update(float elapsed_tm)
        {
            if (SignDestroy) return;

            // 循环更新Update容器中所有元素
            foreach (var i in ListComponent)
            {
                i.update(elapsed_tm);

                if (SignDestroy) break;
            }
        }

        //---------------------------------------------------------------------
        internal void _onChildInit(Entity child)
        {
            if (SignDestroy) return;

            foreach (var i in ListComponent)
            {
                i.onChildInit(child);

                if (SignDestroy) break;
            }
        }

        //---------------------------------------------------------------------
        internal void _handleEvent(object sender, EntityEvent e)
        {
            if (SignDestroy) return;

            foreach (var i in ListComponent)
            {
                i.handleEvent(sender, e);

                if (SignDestroy) break;
            }
        }

        //---------------------------------------------------------------------
        internal void _addChild(Entity child)
        {
            if (mMapChild == null)
            {
                mMapChild = new Dictionary<string, Dictionary<string, Entity>>();
            }

            string et_type = child.Type;
            string et_guid = child.Guid;
            if (mMapChild.ContainsKey(et_type))
            {
                Dictionary<string, Entity> map_child = mMapChild[et_type];
                map_child[et_guid] = child;
            }
            else
            {
                Dictionary<string, Entity> map_child = new Dictionary<string, Entity>();
                map_child[et_guid] = child;
                mMapChild[et_type] = map_child;
            }
        }
    }
}
