// Copyright (c) Cragon. All rights reserved.

namespace GF.Orleans.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Configuration;
    using Autofac.Core;

    public class Gateway
    {
        //---------------------------------------------------------------------
        private GatewayRunner gatewayRunner = new GatewayRunner();

        //---------------------------------------------------------------------
        public static Gateway Instance { get; private set; }
        public IContainer GatewaySessionListenerContainer { get; private set; }

        //---------------------------------------------------------------------
        public Gateway()
        {
            Instance = this;
        }

        //---------------------------------------------------------------------
        public Task Start(IPAddress ip_address, int port, string orleansClientConfigFile)
        {
            //var curDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //var listFile = Directory.GetFiles(curDir, "*.dll").ToList();
            //listFile.RemoveAll(s => { return s.Contains("Autofac.Configuration.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("Autofac.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("DotNetty.Buffers.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("DotNetty.Codecs.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("DotNetty.Codecs.Mqtt.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("DotNetty.Common.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("DotNetty.Handlers.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("DotNetty.Transport.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("GF.Unity.Common.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("GF.Unity.Sqlite.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("ICSharpCode.SharpZipLib.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("Microsoft.CodeAnalysis.CSharp.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("Microsoft.CodeAnalysis.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("Newtonsoft.Json.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("Orleans.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("OrleansCodeGenerator.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("OrleansProviders.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("Pngcs.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("System.Collections.Immutable.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("System.Reflection.Metadata.dll"); });
            //listFile.RemoveAll(s => { return s.Contains("UnityEngine.dll"); });

            //var builder = new ContainerBuilder();
            //foreach (var i in listFile)
            //{
            //    var assemble = Assembly.LoadFile(i);

            //    builder.RegisterAssemblyTypes(assemble)
            //        .As<IGatewaySessionListener>();
            //    this.GatewaySessionListenerContainer = builder.Build();
            //    var aa = this.GatewaySessionListenerContainer.Resolve<IGatewaySessionListener>();
            //}

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
            this.GatewaySessionListenerContainer = builder.Build();

            var gatewaySessionFactory = new GatewaySessionFactory();

            return this.gatewayRunner.Start(ip_address, port, orleansClientConfigFile, gatewaySessionFactory);
        }

        //---------------------------------------------------------------------
        public Task Stop()
        {
            return this.gatewayRunner.Stop();
        }
    }
}
