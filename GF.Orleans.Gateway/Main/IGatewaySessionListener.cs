// Copyright (c) Cragon. All rights reserved.

namespace GF.Orleans.Gateway
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Orleans;

    public interface IGatewaySessionListener
    {
        Task OnSessionCreate();

        Task OnSessionDestroy();

        Task Unity2Orleans(ushort method_id, byte[] data);

        Task Orleans2Unity(ushort method_id, byte[] data);
    }
}
