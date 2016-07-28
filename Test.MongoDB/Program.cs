// Copyright (c) Cragon. All rights reserved.

namespace Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;


    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "mongodb://114.55.67.210:27017/";
            string databaseName = "TexasPoker";
            string collectionName = "EtPlayer";

            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase db = client.GetDatabase(databaseName);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //for (int i = 0; i < 1000000; i++)
            //{
            //    db.GetCollection<BsonDocument>(collectionName);
            //}

            int index = 1;
            for (int i = 0; i < 100; i++)
            {
                //db.GetCollection<BsonDocument>(collectionName);
                //db.CreateCollection("TestA");
                //db.CreateCollection("Test" + index++);
                db.GetCollection<BsonDocument>("Test" + index++);
            }

            var tm = sw.ElapsedMilliseconds;
            Console.WriteLine(tm.ToString());

            sw.Stop();

            Console.ReadKey();
        }
    }
}
