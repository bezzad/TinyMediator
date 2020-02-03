using System;
using System.Threading.Tasks;
using Shouldly;
using StructureMap;
using Xunit;

namespace TinyMediator.Test
{
    public class ExceptionTests
    {
        private readonly IMediator _mediator;

        public class Pinged : ISignal
        {
        }

        public class AsyncPinged : ISignal
        {
        }

        public ExceptionTests()
        {
            var container = new Container(cfg =>
            {
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
                cfg.For<IMediator>().Use<Mediator>();
            });
            _mediator = container.GetInstance<IMediator>();
        }

        [Fact]
        public async Task Should_not_throw_for_publish()
        {
            Exception ex = null;
            try
            {
                await _mediator.Publish(new Pinged());
            }
            catch (Exception e)
            {
                ex = e;
            }
            ex.ShouldBeNull();
        }

        [Fact]
        public async Task Should_not_throw_for_async_publish()
        {
            Exception ex = null;
            try
            {
                await _mediator.Publish(new AsyncPinged());
            }
            catch (Exception e)
            {
                ex = e;
            }
            ex.ShouldBeNull();
        }
    }
}
