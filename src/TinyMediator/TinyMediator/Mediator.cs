using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TinyMediator
{
    /// <summary>
    /// Default mediator implementation relying on single- and multi instance delegates for resolving handlers.
    /// </summary>
    public class Mediator : IMediator
    {
        private ServiceFactory ServiceFactory { get; }
        private static ConcurrentDictionary<Type, SignalHandlerWrapper> SignalHandlers { get; } = new ConcurrentDictionary<Type, SignalHandlerWrapper>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator"/> class.
        /// </summary>
        /// <param name="serviceFactory">The single instance factory.</param>
        public Mediator(ServiceFactory serviceFactory)
        {
            ServiceFactory = serviceFactory;
        }



        public Task Publish<TSignal>(TSignal signal, CancellationToken cancellationToken = default)
             where TSignal : ISignal
        {
            if (signal == null)
            {
                throw new ArgumentNullException(nameof(signal));
            }

            return PublishSignal(signal, cancellationToken);
        }

        public Task Publish(object signal, CancellationToken cancellationToken = default)
        {
            if (signal == null)
            {
                throw new ArgumentNullException(nameof(signal));
            }
            if (signal is ISignal instance)
            {
                return PublishSignal(instance, cancellationToken);
            }

            throw new ArgumentException($"{nameof(signal)} does not implement ${nameof(ISignal)}");
        }
        
        /// <summary>
        /// Override in a derived class to control how the tasks are awaited. By default the implementation is a foreach and await of each handler
        /// </summary>
        /// <param name="allHandlers">Enumerable of tasks representing invoking each signal handler</param>
        /// <param name="signal">The signal being published</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing invoking all handlers</returns>
        protected virtual async Task PublishCore(IEnumerable<Func<ISignal, CancellationToken, Task>> allHandlers, ISignal signal, CancellationToken cancellationToken)
        {
            foreach (var handler in allHandlers)
            {
                await handler(signal, cancellationToken).ConfigureAwait(false);
            }
        }

        private Task PublishSignal(ISignal signal, CancellationToken cancellationToken = default)
        {
            var signalType = signal.GetType();
            var handler = SignalHandlers.GetOrAdd(signalType,
                t => (SignalHandlerWrapper)Activator.CreateInstance(typeof(SignalHandlerWrapperImpl<>).MakeGenericType(signalType)));

            return handler.Handle(signal, cancellationToken, ServiceFactory, PublishCore);
        }
    }
}
