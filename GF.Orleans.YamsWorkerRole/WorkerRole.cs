// Copyright (c) Cragon. All rights reserved.

namespace GF.Orleans.YamsWorkerRole
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Etg.Yams;
    using Etg.Yams.Azure.Utils;
    using Etg.Yams.Utils;

    public class WorkerRole : RoleEntryPoint
    {
        //---------------------------------------------------------------------
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private IYamsService yamsService;

        //---------------------------------------------------------------------
        //[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override void Run()
        {
            Trace.TraceInformation("WorkerRoleYams is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        //---------------------------------------------------------------------
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRoleYams has been started");

            return result;
        }

        //---------------------------------------------------------------------
        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRoleYams is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            StopAsync().Wait();

            base.OnStop();

            Trace.TraceInformation("WorkerRoleYams has stopped");
        }

        //---------------------------------------------------------------------
        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            //while (!cancellationToken.IsCancellationRequested)
            //{
            //    Trace.TraceInformation("Working");
            //    await Task.Delay(1000);
            //}

            WorkerRoleConfig config = new WorkerRoleConfig();
            YamsConfig yamsConfig = new YamsConfigBuilder(
                // mandatory configs
                DeploymentIdUtils.CloudServiceDeploymentId,
                RoleEnvironment.CurrentRoleInstance.UpdateDomain.ToString(),
                RoleEnvironment.CurrentRoleInstance.Id,
                config.CurrentRoleInstanceLocalStoreDirectory)
                // optional configs
                .SetCheckForUpdatesPeriodInSeconds(config.UpdateFrequencyInSeconds)
                .SetApplicationRestartCount(config.ApplicationRestartCount)
                .Build();

            this.yamsService = YamsServiceFactory.Create(yamsConfig,
                deploymentRepositoryStorageConnectionString: config.StorageDataConnectionString,
                updateSessionStorageConnectionString: config.StorageDataConnectionString);

            try
            {
                Trace.TraceInformation("Yams is starting");

                await this.yamsService.Start();

                Trace.TraceInformation("Yams has started. Looking for apps with deploymentId:" + yamsConfig.ClusterDeploymentId);

                while (true)
                {
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        //---------------------------------------------------------------------
        public async Task StopAsync()
        {
            if (this.yamsService != null)
            {
                await this.yamsService.Stop();
            }
        }
    }
}
