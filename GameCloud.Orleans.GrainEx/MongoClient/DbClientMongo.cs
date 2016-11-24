// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.Orleans
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Orleans;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using GameCloud.Unity.Common;

    public class DbClientMongo
    {
        //-------------------------------------------------------------------------
        public const string DOC_DEFAULT_ID_NAME = "_id";// 默认唯一标识

        //-------------------------------------------------------------------------
        public static DbClientMongo Instance { get; private set; }
        public IMongoDatabase Database { get; private set; }

        //-------------------------------------------------------------------------
        public DbClientMongo(string databaseName, string connectionString)
        {
            Instance = this;
            MongoClient client = new MongoClient(connectionString);
            Database = client.GetDatabase(databaseName);
        }

        //---------------------------------------------------------------------
        public Task DeleteOneAsync<TDocument>(string collectionName, string key)
        {
            var collection = GetCollection<TDocument>(collectionName);
            if (collection == null)
            {
                return TaskDone.Done;
            }

            var builder = Builders<TDocument>.Filter.Eq(DOC_DEFAULT_ID_NAME, key);

            return collection.DeleteOneAsync(builder);
        }

        //---------------------------------------------------------------------
        public async Task<string> Read<TDocument>(string collectionName, string key)
        {
            var collection = GetCollection<TDocument>(collectionName);
            if (collection == null) return null;

            var builder = Builders<TDocument>.Filter.Eq(DOC_DEFAULT_ID_NAME, key);
            var project = Builders<TDocument>.Projection.Exclude(DOC_DEFAULT_ID_NAME);
            var read_data = await collection.Find(builder).Project(project).FirstOrDefaultAsync();

            if (read_data == null) return null;

            return read_data.ToJson();
        }

        //---------------------------------------------------------------------
        public async Task<Entity> ReadEntity<TEtEntity, TUserData>(string collectionName, string key, TUserData user_data)
            where TEtEntity : EntityDef
        {
            var collection = GetCollection<EntityData>(collectionName);
            if (collection == null) return null;

            var builder = Builders<EntityData>.Filter.Eq(DOC_DEFAULT_ID_NAME, key);
            var project = Builders<EntityData>.Projection.Exclude(DOC_DEFAULT_ID_NAME);
            var read_data = await collection.Find(builder).Project(project).FirstOrDefaultAsync();

            if (read_data == null) return null;

            var entity_data = EbTool.jsonDeserialize<EntityData>(read_data.ToJson());
            Entity et = EntityMgr.Instance.genEntity<TEtEntity, TUserData>(entity_data, user_data);

            return et;
        }

        //---------------------------------------------------------------------      
        public Task InsertOneData<T>(string collectionName, string key, T doc)
        {
            var collection = GetOrCreateCollection<T>(collectionName);

            try
            {
                collection.InsertOneAsync(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------      
        public Task InertOneEntity(Entity et)
        {
            var collection = GetOrCreateCollection<BsonDocument>(et.Type);
            string et_data = EbTool.jsonSerialize(et.genEntityData4SaveDb());
            var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(et_data);
            doc[DOC_DEFAULT_ID_NAME] = et.Guid;
            try
            {
                collection.InsertOneAsync(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------      
        public Task ReplaceOneData<T>(string collectionName, string key, T doc)
        {
            var collection = GetOrCreateCollection<T>(collectionName);

            var builder = Builders<T>.Filter.Eq(DOC_DEFAULT_ID_NAME, key);
            collection.ReplaceOneAsync(builder, doc);

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------      
        public Task ReplaceOneEntity(Entity et)
        {
            var collection = GetOrCreateCollection<BsonDocument>(et.Type);
            string et_data = EbTool.jsonSerialize(et.genEntityData4SaveDb());
            var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(et_data);
            doc[DOC_DEFAULT_ID_NAME] = et.Guid;

            var builder = Builders<BsonDocument>.Filter.Eq(DOC_DEFAULT_ID_NAME, et.Guid);
            try
            {
                collection.ReplaceOneAsync(builder, doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        public void createAndGetCollection<TDocument>(string name)
        {
            try
            {
                Database.CreateCollection(name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Database.GetCollection<TDocument>(name);
        }

        //---------------------------------------------------------------------
        public IMongoCollection<TDocument> getCollection<TDocument>(string name)
        {
            return Database.GetCollection<TDocument>(name);
        }

        //---------------------------------------------------------------------
        public void dropCollection(string name)
        {
            var collection = Database.GetCollection<BsonDocument>(name);
            if (collection != null)
            {
                Database.DropCollection(name);
            }
        }

        //---------------------------------------------------------------------
        public void createIndex(string name, bool is_ascending, params string[] prop_indextext)
        {
            var collection = getCollection<BsonDocument>(name);
            if (collection != null)
            {
                var collecion_indexes = collection.Indexes;
                BsonDocument index = new BsonDocument();
                foreach (var i in prop_indextext)
                {
                    index.Add(i, is_ascending ? 1 : -1);
                }

                collecion_indexes.CreateOne(index);
            }
        }

        //---------------------------------------------------------------------     
        private IMongoCollection<TDocument> GetCollection<TDocument>(string name)
        {
            return Database.GetCollection<TDocument>(name);
        }

        //---------------------------------------------------------------------       
        private IMongoCollection<T> GetOrCreateCollection<T>(string name)
        {
            var collection = Database.GetCollection<T>(name);
            if (collection != null)
            {
                return collection;
            }

            Database.CreateCollection(name);
            collection = Database.GetCollection<T>(name);

            return collection;
        }
    }
}

//---------------------------------------------------------------------
//public async Task<Entity> ReadEntity<TEtEntity>(string collectionName, string key) where TEtEntity : EntityDef
//{
//    var collection = GetCollection<EntityData>(collectionName);
//    if (collection == null) return null;

//    var builder = Builders<EntityData>.Filter.Eq(DOC_DEFAULT_ID_NAME, key);
//    var project = Builders<EntityData>.Projection.Exclude(DOC_DEFAULT_ID_NAME);
//    var read_data = await collection.Find(builder).Project(project).FirstOrDefaultAsync();

//    if (read_data == null) return null;

//    var entity_data = EbTool.jsonDeserialize<EntityData>(read_data.ToJson());
//    Entity et = EntityMgr.Instance.genEntity<TEtEntity>(entity_data);

//    return et;
//}
