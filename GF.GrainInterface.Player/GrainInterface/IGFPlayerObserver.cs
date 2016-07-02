// Copyright (c) Cragon. All rights reserved.

namespace GF.GrainInterface.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGFPlayerObserver
    {
        //---------------------------------------------------------------------
        void Notify(MethodData method_data);
    }
}
