// Copyright(c) Cragon. All rights reserved.

namespace GameCloud.IM
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Orleans.Runtime.Configuration;

    public static class CrossDomainTaskMarshaler
    {
        //---------------------------------------------------------------------
        public static Task<T> Marshal<T>(AppDomain appDomain, Func<Task<T>> function)
        {
            var m = new MarshalableCompletionSource<T>();
            var t = typeof(RemoteWorker<T>);
            var w = (RemoteWorker<T>)appDomain.CreateInstanceAndUnwrap(t.Assembly.FullName, t.FullName);
            w.Run(function, m);
            return m.Task;
        }

        //---------------------------------------------------------------------
        public class MarshalableCompletionSource<T> : MarshalByRefObject
        {
            private readonly TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

            public Task<T> Task { get { return this.tcs.Task; } }

            public void SetResult(T result)
            {
                this.tcs.SetResult(result);
            }

            public void SetException(Exception[] exception)
            {
                this.tcs.SetException(exception);
            }

            public void SetCanceled()
            {
                this.tcs.SetCanceled();
            }
        }

        //---------------------------------------------------------------------
        public class RemoteWorker<T> : MarshalByRefObject
        {
            public void Run(Func<Task<T>> function, MarshalableCompletionSource<T> marshaler)
            {
                function().ContinueWith(t =>
                {
                    if (t.IsFaulted) marshaler.SetException(t.Exception.InnerExceptions.ToArray());
                    else if (t.IsCanceled) marshaler.SetCanceled();
                    else marshaler.SetResult(t.Result);
                });
            }
        }
    }
}
