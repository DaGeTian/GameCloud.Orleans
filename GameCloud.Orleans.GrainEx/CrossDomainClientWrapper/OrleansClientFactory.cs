// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.IM
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Orleans;
    using global::Orleans.Concurrency;
    using global::Orleans.Providers;
    using global::Orleans.Runtime;
    using global::Orleans.Runtime.Configuration;

    // The following is a base class to use for creating client wrappers.
    // We use ClientWrappers to load an Orleans client in its own app domain. 
    // This allows us to create multiple clients that are connected to different silos.
    public class OrleansClientWrapper : MarshalByRefObject
    {
        //---------------------------------------------------------------------
        public string Name { get; private set; }
        public AppDomain AppDomain { get; private set; }

        //---------------------------------------------------------------------
        public OrleansClientWrapper(string name, string orleansConfigFile)
        {
            this.Name = name;
            this.AppDomain = AppDomain.CurrentDomain;

            Console.WriteLine("Initializing OrleansClientWrapperBase in AppDomain {0}", this.AppDomain.FriendlyName);

            GrainClient.Initialize(orleansConfigFile);
        }
    }

    /// <summary>
    /// A utility class for tests that include multiple clusters.
    /// It calls static methods on TestingSiloHost for starting and stopping silos.
    /// </summary>
    public class OrleansClientFactory
    {
        //---------------------------------------------------------------------
        public ParallelOptions paralleloptions = new ParallelOptions() { MaxDegreeOfParallelism = 4 };
        private readonly List<AppDomain> activeClients = new List<AppDomain>();

        //---------------------------------------------------------------------
        // Create a client, loaded in a new app domain.
        public OrleansClientWrapper NewClient(string clientName, string orleansConfigFile)
        {
            var clientArgs = new object[] { clientName, orleansConfigFile };

            var setup = _getAppDomainSetupInfo();

            var currentAppDomain = AppDomain.CurrentDomain;
            var clientDomain = AppDomain.CreateDomain(clientName, currentAppDomain.Evidence, setup);

            OrleansClientWrapper client = (OrleansClientWrapper)clientDomain.CreateInstanceFromAndUnwrap(
                    Assembly.GetExecutingAssembly().Location, typeof(OrleansClientWrapper).FullName, false,
                    BindingFlags.Default, null, clientArgs, CultureInfo.CurrentCulture,
                    new object[] { });

            lock (activeClients)
            {
                activeClients.Add(clientDomain);
            }

            return client;
        }

        //---------------------------------------------------------------------
        public void StopAllClients()
        {
            List<AppDomain> ac;

            lock (activeClients)
            {
                ac = activeClients.ToList();
                activeClients.Clear();
            }

            Parallel.For(0, ac.Count, paralleloptions, (i) =>
            {
                try
                {
                    AppDomain.Unload(ac[i]);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception Caught While Unloading AppDomain for client {0}: {1}", i, e);
                }
            });
        }

        //---------------------------------------------------------------------
        AppDomainSetup _getAppDomainSetupInfo()
        {
            var currentAppDomain = AppDomain.CurrentDomain;

            var setup = new AppDomainSetup
            {
                ApplicationBase = Environment.CurrentDirectory,
                ApplicationName = currentAppDomain.SetupInformation.ApplicationName,
                ApplicationTrust = currentAppDomain.ApplicationTrust,
                ConfigurationFile = currentAppDomain.SetupInformation.ConfigurationFile,
                ShadowCopyFiles = currentAppDomain.SetupInformation.ShadowCopyFiles,
                ShadowCopyDirectories = currentAppDomain.SetupInformation.ShadowCopyDirectories,
                CachePath = currentAppDomain.SetupInformation.CachePath,
                LoaderOptimization = LoaderOptimization.MultiDomainHost
            };

            return setup;
        }
    }
}
