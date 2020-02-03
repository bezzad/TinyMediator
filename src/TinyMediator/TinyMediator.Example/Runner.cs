using System;
using System.Linq;
using System.Threading.Tasks;
using TinyMediator.Example.Models;

namespace TinyMediator.Example
{
    public static class Runner
    {
        public static async Task Run(IMediator mediator, WrappingWriter writer, string projectName)
        {
            await writer.WriteLineAsync("===============");
            await writer.WriteLineAsync(projectName);
            await writer.WriteLineAsync("===============");
            await writer.WriteLineAsync();

            await writer.WriteLineAsync("Publishing Message...");
            ISignal signal = new Message() {Text = "I'm a signal message body"};
            await mediator.Publish(signal);
            await writer.WriteLineAsync();

            await writer.WriteLineAsync("Publishing Pinged...");
            await mediator.Publish(new Pinged());
            await writer.WriteLineAsync();

            await writer.WriteLineAsync("Publishing Ponged...");
            var failedPong = false;
            try
            {
                await mediator.Publish(new Ponged());
            }
            catch (Exception e)
            {
                failedPong = true;
                await writer.WriteLineAsync(e.ToString());
            }
            await writer.WriteLineAsync();

            await writer.WriteLineAsync("---------------");
            var contents = writer.Contents;
            var order = new[] {
                contents.IndexOf("- Starting Up", StringComparison.OrdinalIgnoreCase),
                contents.IndexOf("-- Handling Request", StringComparison.OrdinalIgnoreCase),
                contents.IndexOf("--- Handled Ping", StringComparison.OrdinalIgnoreCase),
                contents.IndexOf("-- Finished Request", StringComparison.OrdinalIgnoreCase),
                contents.IndexOf("- All Done", StringComparison.OrdinalIgnoreCase),
                contents.IndexOf("- All Done with Ping", StringComparison.OrdinalIgnoreCase),
            };

            var results = new RunResults
            {
                RequestHandlers = contents.Contains("--- Handled Ping:"),
                PipelineBehaviors = contents.Contains("-- Handling Request"),
                RequestPreProcessors = contents.Contains("- Starting Up"),
                RequestPostProcessors = contents.Contains("- All Done"),
                OrderedPipelineBehaviors = order.SequenceEqual(order.OrderBy(i => i)),
                NotificationHandler = contents.Contains("Got pinged async"),
                MultipleNotificationHandlers = contents.Contains("Got pinged async") && contents.Contains("Got pinged also async"),
                ConstrainedGenericNotificationHandler = contents.Contains("Got pinged constrained async") && !failedPong,
                CovariantNotificationHandler = contents.Contains("Got notified")
            };

            await writer.WriteLineAsync($"Request Handler....................................................{(results.RequestHandlers ? "Y" : "N")}");
            await writer.WriteLineAsync($"Void Request Handler...............................................{(results.VoidRequestsHandlers ? "Y" : "N")}");
            await writer.WriteLineAsync($"Pipeline Behavior..................................................{(results.PipelineBehaviors ? "Y" : "N")}");
            await writer.WriteLineAsync($"Pre-Processor......................................................{(results.RequestPreProcessors ? "Y" : "N")}");
            await writer.WriteLineAsync($"Post-Processor.....................................................{(results.RequestPostProcessors ? "Y" : "N")}");
            await writer.WriteLineAsync($"Constrained Post-Processor.........................................{(results.ConstrainedGenericBehaviors ? "Y" : "N")}");
            await writer.WriteLineAsync($"Ordered Behaviors..................................................{(results.OrderedPipelineBehaviors ? "Y" : "N")}");
            await writer.WriteLineAsync($"Notification Handler...............................................{(results.NotificationHandler ? "Y" : "N")}");
            await writer.WriteLineAsync($"Notification Handlers..............................................{(results.MultipleNotificationHandlers ? "Y" : "N")}");
            await writer.WriteLineAsync($"Constrained Notification Handler...................................{(results.ConstrainedGenericNotificationHandler ? "Y" : "N")}");
            await writer.WriteLineAsync($"Covariant Notification Handler.....................................{(results.CovariantNotificationHandler ? "Y" : "N")}");
            await writer.WriteLineAsync($"Handler for inherited request with same exception used.............{(results.HandlerForSameException ? "Y" : "N")}");
            await writer.WriteLineAsync($"Handler for inherited request with base exception used.............{(results.HandlerForBaseException ? "Y" : "N")}");
            await writer.WriteLineAsync($"Handler for request with less specific exception used by priority..{(results.HandlerForLessSpecificException ? "Y" : "N")}");
            await writer.WriteLineAsync($"Preferred handler for inherited request with base exception used...{(results.PreferredHandlerForBaseException ? "Y" : "N")}");
            await writer.WriteLineAsync($"Overridden handler for inherited request with same exception used..{(results.OverriddenHandlerForBaseException ? "Y" : "N")}");

            await writer.WriteLineAsync();
        }

        private static bool IsExceptionHandledBy<TException, THandler>(WrappingWriter writer)
            where TException : Exception
        {
            var messages = writer.Contents.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            return messages[^2].Contains(typeof(THandler).FullName ?? throw new InvalidOperationException())
                && messages[^3].Contains(typeof(TException).FullName ?? throw new InvalidOperationException());
        }
    }
}
