// Copyright (c) Cragon. All rights reserved.

namespace GF.Grain.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Orleans;
    using Orleans.Concurrency;
    using Orleans.Runtime;
    using Orleans.Streams;
    using GF.Unity.Common;
    using GF.GrainInterface.Player;

    public class GFClientSession : Grain, IGFClientSession
    {
        //---------------------------------------------------------------------
        public ObserverSubscriptionManager<IGFClientObserver> Subscribers { get; private set; }
        public Logger Logger { get { return GetLogger(); } }

        //---------------------------------------------------------------------
        public override Task OnActivateAsync()
        {
            Subscribers = new ObserverSubscriptionManager<IGFClientObserver>();

            Logger.Info("OnActivateAsync() GrainId={0}", this.GetPrimaryKey().ToString());

            return base.OnActivateAsync();
        }

        //---------------------------------------------------------------------
        public override Task OnDeactivateAsync()
        {
            Subscribers.Clear();

            Logger.Info("OnDeactivateAsync() GrainId={0}", this.GetPrimaryKey().ToString());

            return base.OnDeactivateAsync();
        }

        //---------------------------------------------------------------------
        Task IGFClientSession.SubClient(IGFClientObserver sub)
        {
            Logger.Info("SubClient() GrainId={0}", this.GetPrimaryKey().ToString());

            Subscribers.Subscribe(sub);

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        Task IGFClientSession.UnsubClient(IGFClientObserver sub)
        {
            bool is_sub = Subscribers.IsSubscribed(sub);
            if (is_sub)
            {
                Subscribers.Unsubscribe(sub);

                DeactivateOnIdle();
            }

            Logger.Info("UnsubClient() GrainId={0}", this.GetPrimaryKey().ToString());

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        // 客户端请求
        Task IGFClientSession.Request(ushort method_id, byte[] data)
        {
            Logger.Info("Request() MethodId={0}", method_id);

            if (method_id < 100)
            {
            }
            else
            {
            }

            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        Task IGFClientSession.GetPlayerList()
        {
            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        Task IGFClientSession.NewPlayer(Dictionary<string, string> map_newplayer_data)
        {
            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        Task IGFClientSession.DeletePlayer(string et_player_guid)
        {
            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        Task IGFClientSession.EnterWorld()
        {
            return TaskDone.Done;
        }

        //---------------------------------------------------------------------
        Task IGFClientSession.LeaveWorld()
        {
            return TaskDone.Done;
        }
    }
}
