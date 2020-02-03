using System.Threading;
using System.Threading.Tasks;

namespace TinyMediator
{
    /// <summary>
    /// Defines a mediator to encapsulate request/response and publishing interaction patterns
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Asynchronously send a signal to multiple handlers
        /// </summary>
        /// <param name="signal">Signal object</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A task that represents the publish operation.</returns>
        Task Publish(object signal, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously send a signal to multiple handlers
        /// </summary>
        /// <param name="signal">Signal object</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A task that represents the publish operation.</returns>
        Task Publish<TSignal>(TSignal signal, CancellationToken cancellationToken = default)
            where TSignal : ISignal;
    }
}
