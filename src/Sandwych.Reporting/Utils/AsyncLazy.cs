// https://devblogs.microsoft.com/pfxteam/asynclazyt/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandwych.Reporting.Utils
{
    public sealed class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory, CancellationToken cancel) :
            base(() => Task.Factory.StartNew(valueFactory, cancel))
        { }

        public AsyncLazy(Func<T> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory))
        { }

        public AsyncLazy(Func<Task<T>> taskFactory, CancellationToken cancel) :
            base(() => Task.Factory.StartNew(() => taskFactory(), cancel).Unwrap())
        { }

        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap())
        { }

        public AsyncLazy(Func<CancellationToken, Task<T>> taskFactory, CancellationToken cancel = default) :
            base(() => Task.Factory.StartNew(() => taskFactory(cancel), cancel).Unwrap())
        { }

        public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
    }
}
