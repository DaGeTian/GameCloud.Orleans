// Copyright (c) Cragon. All rights reserved.

namespace GF.Orleans.Host
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        //---------------------------------------------------------------------
        static void Main(string[] args)
        {
            var siloHost = new WindowsServerHost();

            int exitCode;
            try
            {
                if (!siloHost.ParseArguments(args))
                {
                    siloHost.PrintUsage();
                    exitCode = -1;
                }
                else
                {
                    siloHost.Init();
                    exitCode = siloHost.Run();
                }
            }
            catch (Exception ex)
            {
                EbLog.Error(string.Format("halting due to error - {0}", ex.ToString()));
                exitCode = 1;
            }
            finally
            {
                siloHost.Dispose();
            }

            Environment.Exit(exitCode);
        }
    }
}
