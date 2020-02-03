using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TinyMediator.Example.Models
{
    public class MessageHandler<TSignal> : ISignalHandler<TSignal>
        where TSignal : Message
    {
        private readonly TextWriter _writer;

        public MessageHandler(TextWriter writer)
        {
            _writer = writer;
        }

        public Task Handle(TSignal signal, CancellationToken cancellationToken)
        {
            return _writer.WriteLineAsync($"Got message async: {signal.Text}");
        }
    }
}
