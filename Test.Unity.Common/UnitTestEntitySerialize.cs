// Copyright (c) Cragon. All rights reserved.


namespace Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ProtoBuf;
    using GameCloud.Unity.Common;

    [ProtoContract]
    public class TestData1
    {
        [ProtoMember(1)]
        public string s1;
        [ProtoMember(2)]
        public int a1;
        [ProtoMember(3)]
        public int a2;
        [ProtoMember(4)]
        public string s2;
    }

    public class DefPlayer : ComponentDef
    {
        //---------------------------------------------------------------------
        [PropAttrDistribution((byte)2, true)]
        public Prop<long> PropGiveChipTotal;// 当天赠送的筹码总数

        [PropAttrDistribution((byte)2, true)]
        public Prop<DateTime> PropGiveChipDateTime;// 当天赠送的日期

        [PropAttrDistribution((byte)2, true)]
        public Prop<bool> PropFirstLoginDateTime;// 当天首次登录日期

        //---------------------------------------------------------------------
        public override void defAllProp(Dictionary<string, string> map_param)
        {
            PropGiveChipTotal = defProp<long>(null, "GiveChipTotal", 100);
            PropGiveChipDateTime = defProp<DateTime>(null, "GiveChipDateTime", DateTime.Now);
            PropFirstLoginDateTime = defProp<bool>(map_param, "FirstLoginDateTime", true);
        }
    }

    public class EtPlayer : EntityDef
    {
        //---------------------------------------------------------------------
        public override void declareAllComponent(byte node_type)
        {
            declareComponent<DefPlayer>();
        }
    }

    public class CellPlayer<TDef> : Component<TDef> where TDef : DefPlayer, new()
    {
        //---------------------------------------------------------------------
        public override void init()
        {
        }
    }

    [TestClass]
    public class UnitTestEntitySerialize
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestProtoBufSerializerObject()
        {
            int a1 = 100;
            object a2 = a1;

            byte[] data = null;
            using (MemoryStream s = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<int>(s, a1);
                data = s.ToArray();
            }

            using (MemoryStream s = new MemoryStream(data))
            {
                var a3 = ProtoBuf.Serializer.Deserialize<int>(s);

                int a4 = (int)a3;

                Debug.WriteLine(a1);
                Debug.WriteLine(a2);
                Debug.WriteLine(a3);
                Debug.WriteLine(a4);
            }
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void TestProtoBufSerializerProp()
        {
            TestData1 test_data1 = new TestData1();
            test_data1.s1 = "aaa";
            test_data1.s2 = "asdfasdfasdfasdf";
            test_data1.a1 = 1100;
            test_data1.a2 = 5000;

            MemoryStream s = new MemoryStream();

            var buf = EbTool.protobufSerialize(s, test_data1);
            var test_data2 = EbTool.protobufDeserialize<TestData1>(s, buf);

            int aa = 0;

            //Prop<int> p = new Prop<int>();
            //p.Key = "aaa";
            //p.set(100);

            //Dictionary<string, IProp> m = new Dictionary<string, IProp>();
            //m[p.Key] = p;

            //byte[] data = null;
            //using (MemoryStream s = new MemoryStream())
            //{
            //    ProtoBuf.Serializer.Serialize<Dictionary<string, IProp>>(s, m);
            //    data = s.ToArray();
            //}

            //using (MemoryStream s = new MemoryStream(data))
            //{
            //    var m1 = ProtoBuf.Serializer.Deserialize<Dictionary<string, IProp>>(s);

            //    IProp p1 = m1["aaa"];
            //    Prop<int> p2 = new Prop<int>();
            //    p2.copyValueFrom(p1);

            //    Debug.WriteLine(p1.Key);
            //    Debug.WriteLine(p1.getValue());

            //    Debug.WriteLine(p2.Key);
            //    Debug.WriteLine(p2.getValue());
            //}
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void TestProtoBufSerializerEntity()
        {
            EntityMgr entityMgr = new EntityMgr(2, "Cell");
            entityMgr.regComponent<CellPlayer<DefPlayer>>();
            entityMgr.regEntityDef<EtPlayer>();

            Entity et = entityMgr.createEntity<EtPlayer>(null);
            EntityData etData = et.genEntityData4NetSync(2);

            byte[] data = null;
            using (MemoryStream s = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<EntityData>(s, etData);
                data = s.ToArray();
            }

            using (MemoryStream s = new MemoryStream(data))
            {
                var etData1 = ProtoBuf.Serializer.Deserialize<EntityData>(s);

                Assert.AreEqual(etData.entity_guid, etData1.entity_guid);
                Assert.AreEqual(etData.entity_type, etData1.entity_type);

                foreach (var i in etData.map_component)
                {
                    Debug.WriteLine(i.Key);
                }

                foreach (var i in etData1.map_component)
                {
                    Debug.WriteLine(i.Key);
                }
            }
        }
    }
}
