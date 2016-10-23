// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.CfgCenter.SDK
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Consul;

    public class CfgCenterSDK
    {
        //---------------------------------------------------------------------
        string ConsulUri { get; set; }

        //---------------------------------------------------------------------
        public CfgCenterSDK(string uri)
        {
            ConsulUri = uri;
        }

        //---------------------------------------------------------------------
        public async Task<Dictionary<string, string>> GetAppSettings(string dir)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            Action<ConsulClientConfiguration> action_cfg = (cfg) =>
            {
                cfg.Address = new Uri(ConsulUri);
                //cfg.Datacenter = "dc1";
                //cfg.Token = "yep";
            };

            using (var client = new ConsulClient(action_cfg))
            {
                //config.DisableServerCertificateValidation = false;
                //await client.KV.Put(new KVPair("kv/reuseconfig") { Flags = 2000 });

                var r = await client.KV.List(dir);

                if (r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    foreach (var i in r.Response)
                    {
                        if (i.Value == null) continue;

                        string k = Path.GetFileName(i.Key);
                        string v = Encoding.UTF8.GetString(i.Value);
                        settings[k] = v;
                    }
                }
                else
                {
                    Console.WriteLine(r.StatusCode);
                }
            }

            return settings;
        }
    }
}
