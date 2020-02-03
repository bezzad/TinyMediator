using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TinyMediator
{
    internal abstract class SignalHandlerWrapper
    {
        public abstract Task Handle(ISignal signal, CancellationToken cancellationToken, ServiceFactory serviceFactory,
            Func<IEnumerable<Func<ISignal, CancellationToken, Task>>, ISignal, CancellationToken, Task> publish);
    }

    internal class SignalHandlerWrapperImpl<TSignal> : SignalHandlerWrapper
        where TSignal : ISignal
    {
        public override Task Handle(ISignal signal, CancellationToken cancellationToken, ServiceFactory serviceFactory,
            Func<IEnumerable<Func<ISignal, CancellationToken, Task>>, ISignal, CancellationToken, Task> publish)
        {
            var handlers = serviceFactory
                .GetInstances<ISignalHandler<TSignal>>()
                .Select(x => new Func<ISignal, CancellationToken, Task>((theSignal, theToken) => x.Handle((TSignal)theSignal, theToken)));

            return publish(handlers, signal, cancellationToken);
        }
    }
}
