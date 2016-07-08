// Copyright (c) Cragon. All rights reserved.

namespace GF.Orleans.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Orleans;

    public class GatewaySessionListenerDefault : IGatewaySessionListener
    {
        //---------------------------------------------------------------------
        public Task OnSessionCreate()
        {
            Console.WriteLine("GatewaySessionListenerDefault.OnSessionCreate()");

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        public Task OnSessionDestroy()
        {
            Console.WriteLine("GatewaySessionListenerDefault.OnSessionDestroy()");

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        public Task Unity2Orleans(ushort method_id, byte[] data)
        {
            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        public Task Orleans2Unity(ushort method_id, byte[] data)
        {
            return TaskDone.Done;
        }
    }
}
