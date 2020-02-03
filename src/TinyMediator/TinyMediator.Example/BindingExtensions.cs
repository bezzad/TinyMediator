using Ninject.Syntax;
using System.Linq;

namespace TinyMediator.Example
{
    public static class BindingExtensions
    {
        public static IBindingInNamedWithOrOnSyntax<object> WhenNotificationMatchesType<TNotification>(this IBindingWhenSyntax<object> syntax)
            where TNotification : ISignal
        {
            return syntax.When(request => typeof(TNotification).IsAssignableFrom(request.Service.GenericTypeArguments.Single()));
        }
    }
}
