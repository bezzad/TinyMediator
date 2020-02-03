using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TinyMediator.Example
{
    public class ConstrainedPingedHandler<TNotification> : ISignalHandler<TNotification>
        where TNotification : Pinged
    {
        private readonly TextWriter _writer;

        public ConstrainedPingedHandler(TextWriter writer)
        {
            _writer = writer;
        }

        public Task Handle(TNotification notification, CancellationToken cancellationToken)
        {
            return _writer.WriteLineAsync("Got pinged constrained async.");
        }
    }
}
