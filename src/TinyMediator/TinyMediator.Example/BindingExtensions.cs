using Ninject.Syntax;
using System.Linq;
using Ninject;

namespace TinyMediator.Example
{
    public static class BindingExtensions
    {
        public static IBindingInNamedWithOrOnSyntax<object> WhenSignalMatchesType<TSignal>(this IBindingWhenSyntax<object> syntax)
            where TSignal : ISignal
        {
            return syntax.When(request => typeof(TSignal).IsAssignableFrom(request.Service.GenericTypeArguments.Single()));
        }

        public static void BindMediatorHandlers(this IKernel kernel, object obj)
        {
            // Check obj type have any signal handlers
            var interfaces = obj.GetType().GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(ISignalHandler<>) &&
                            typeof(ISignal).IsAssignableFrom(i.GetGenericArguments().Single())).ToList();

            foreach (var @interface in interfaces)
            {
                // Bind to DI and Call WhenSignalMatchesType like:
                // ApplicationInjector.Current.Container
                //        .Bind(typeof(ISignalHandler<>)).ToConstant(obj)
                //        .WhenSignalMatchesType<ThemeChangedSignal>();

                var whenSignalMatchesTypeMethod = typeof(BindingExtensions)
                    .GetMethod(nameof(WhenSignalMatchesType))
                    ?.MakeGenericMethod(@interface.GetGenericArguments().Single());

                if (whenSignalMatchesTypeMethod != null)
                    whenSignalMatchesTypeMethod.Invoke(null, new object[]
                    {
                        kernel.Bind(typeof(ISignalHandler<>)).ToConstant(obj)
                    });
            }
        }
    }
}
