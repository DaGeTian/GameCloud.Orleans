// Copyright (c) Cragon. All rights reserved.

namespace Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GameCloud.CfgCenter.SDK;

    class Program
    {
        //---------------------------------------------------------------------
        static async Task RunTestAsync()
        {
            CfgCenterSDK cfg_center_sdk = new CfgCenterSDK("http://aaaa:8500/");
            var settings = await cfg_center_sdk.GetAppSettings("Cragon.Fishing.Production/Fishing.Gateway");

            foreach (var i in settings)
            {
                string s = string.Format("key={0}, value={1}", i.Key, i.Value);
                Console.WriteLine(s);
            }

            Console.ReadKey();
        }

        //---------------------------------------------------------------------
        static void Main() => RunTestAsync().Wait();
    }
}
