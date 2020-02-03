using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Planning.Bindings.Resolvers;
using TinyMediator.Example.Models;

namespace TinyMediator.Example
{
    internal class Program
    {
        private static Task Main(string[] args)
        {
            var writer = new WrappingWriter(Console.Out);
            var mediator = BuildMediator(writer);

            

            return Runner.Run(mediator, writer, "Ninject");
        }

        private static IMediator BuildMediator(WrappingWriter writer)
        {
            var kernel = new StandardKernel();
            kernel.Bind<TextWriter>().ToConstant(writer);

            kernel.Components.Add<IBindingResolver, ContravariantBindingResolver>();
            //kernel.Bind(scan => scan.FromAssemblyContaining<IMediator>().SelectAllClasses().BindDefaultInterface());
            kernel.Bind<IMediator>().To<Mediator>();
            kernel.Bind(scan => scan.FromAssemblyContaining<Ping>().SelectAllClasses().InheritedFrom(typeof(ISignalHandler<>)).BindAllInterfaces());
            kernel.Bind(typeof(ISignalHandler<>)).To(typeof(ConstrainedPingedHandler<>)).WhenSignalMatchesType<Pinged>();
            kernel.Bind<ServiceFactory>().ToMethod(ctx => t => ctx.Kernel.TryGet(t));
            
            var mediator = kernel.Get<IMediator>();

            return mediator;
        }
    }
}
