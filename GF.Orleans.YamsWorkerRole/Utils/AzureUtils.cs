// Copyright (c) Cragon. All rights reserved.

namespace GF.Orleans.YamsWorkerRole
{
    using Microsoft.WindowsAzure.ServiceRuntime;

    public static class AzureUtils
    {
        //---------------------------------------------------------------------
        public static bool IsEmulator()
        {
            return RoleEnvironment.IsAvailable && RoleEnvironment.IsEmulated;
        }
    }
}
