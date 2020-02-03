using System.IO;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace TinyMediator.Test
{
    public class SignalHandlerTests
    {
        public class Ping : ISignal
        {
            public string Message { get; set; }
        }

        public class PongChildHandler : SignalHandler<Ping>
        {
            private readonly TextWriter _writer;

            public PongChildHandler(TextWriter writer)
            {
                _writer = writer;
            }

            protected override void Handle(Ping notification)
            {
                _writer.WriteLine(notification.Message + " Pong");
            }
        }

        [Fact]
        public async Task Should_call_abstract_handle_method()
        {
            var builder = new StringBuilder();
            var writer = new StringWriter(builder);

            ISignalHandler<Ping> handler = new PongChildHandler(writer);

            await handler.Handle(
                new Ping() { Message = "Ping" },
                default
            );

            var result = builder.ToString();
            result.ShouldContain("Ping Pong");
        }
    }
}
