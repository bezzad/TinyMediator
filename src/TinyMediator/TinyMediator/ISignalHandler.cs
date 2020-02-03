using System.Threading;
using System.Threading.Tasks;

namespace TinyMediator
{
    /// <summary>
    /// Defines a handler for a signal
    /// </summary>
    /// <typeparam name="TSignal">The type of signal being handled</typeparam>
    public interface ISignalHandler<in TSignal>
        where TSignal : ISignal
    {
        /// <summary>
        /// Handles a signal
        /// </summary>
        /// <param name="signal">The signal</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task Handle(TSignal signal, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Wrapper class for a synchronous signal handler
    /// </summary>
    /// <typeparam name="TSignal">The signal type</typeparam>
    public abstract class SignalHandler<TSignal> : ISignalHandler<TSignal>
        where TSignal : ISignal
    {
        Task ISignalHandler<TSignal>.Handle(TSignal signal, CancellationToken cancellationToken)
        {
            Handle(signal);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override in a derived class for the handler logic
        /// </summary>
        /// <param name="signal">Signal</param>
        protected abstract void Handle(TSignal signal);
    }
}
