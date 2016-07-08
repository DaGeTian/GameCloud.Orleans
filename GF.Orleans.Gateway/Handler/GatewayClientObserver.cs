//// Copyright (c) Cragon. All rights reserved.

//namespace GF.Gateway
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using System.Threading.Tasks;
//    using GF.GrainInterface.Player;

//    public class GatewayClientObserver : IGFClientObserver
//    {
//        //---------------------------------------------------------------------
//        private GatewaySession session;

//        //---------------------------------------------------------------------
//        public GatewayClientObserver(GatewaySession session)
//        {
//            this.session = session;
//        }

//        //---------------------------------------------------------------------
//        void IGFClientObserver.Notify(ushort method_id, byte[] data)
//        {
//            this.session.OnOrleansNotify(method_id, data);
//        }
//    }
//}
