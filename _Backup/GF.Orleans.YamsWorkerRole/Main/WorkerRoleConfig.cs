// Copyright (c) Cragon. All rights reserved.

namespace GF.Orleans.YamsWorkerRole
{
    using System;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class WorkerRoleConfig
    {
        //---------------------------------------------------------------------
        public string StorageDataConnectionString { get; }
        public string CurrentRoleInstanceLocalStoreDirectory { get; }
        public int UpdateFrequencyInSeconds { get; }
        public int ApplicationRestartCount { get; }

        //---------------------------------------------------------------------
        public WorkerRoleConfig()
        {
            UpdateFrequencyInSeconds = Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("UpdateFrequencyInSeconds"));
            ApplicationRestartCount = Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("ApplicationRestartCount"));
            StorageDataConnectionString = RoleEnvironment.GetConfigurationSettingValue("StorageDataConnectionString");
            CurrentRoleInstanceLocalStoreDirectory = RoleEnvironment.GetLocalResource("LocalStoreDirectory").RootPath;
        }
    }
}
