using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TinyMediator.Example.Models
{
    public class ConstrainedPingedHandler<TSignal> : ISignalHandler<TSignal>
        where TSignal : Pinged
    {
        private readonly TextWriter _writer;

        public ConstrainedPingedHandler(TextWriter writer)
        {
            _writer = writer;
        }

        public Task Handle(TSignal notification, CancellationToken cancellationToken)
        {
            return _writer.WriteLineAsync("Got pinged constrained async.");
        }
    }
}
