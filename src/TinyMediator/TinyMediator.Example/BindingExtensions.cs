using Ninject.Syntax;
using System.Linq;

namespace TinyMediator.Example
{
    public static class BindingExtensions
    {
        public static IBindingInNamedWithOrOnSyntax<object> WhenSignalMatchesType<TSignal>(this IBindingWhenSyntax<object> syntax)
            where TSignal : ISignal
        {
            return syntax.When(request => typeof(TSignal).IsAssignableFrom(request.Service.GenericTypeArguments.Single()));
        }
    }
}
