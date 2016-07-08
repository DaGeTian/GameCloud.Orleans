// Copyright (c) Cragon. All rights reserved.

namespace GF.Orleans.YamsWorkerRole
{
    using Microsoft.WindowsAzure.ServiceRuntime;

    public static class DeploymentIdUtils
    {
        //---------------------------------------------------------------------
        public static string CloudServiceDeploymentId
        {
            get
            {
                if (!RoleEnvironment.IsAvailable || RoleEnvironment.IsEmulated)
                {
                    //return Constants.TestDeploymentId;
                    return "";
                }

                return RoleEnvironment.DeploymentId;
            }
        }
    }
}
