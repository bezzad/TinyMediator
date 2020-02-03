**TinyMediator** is a low-ambition library trying to solve a simple problem — decoupling the in-process sending of messages from handling messages. Cross-platform, supporting `netstandard2.0`.

TinyMediator has no dependencies. You will need to configure a single factory delegate, used to instantiate all handlers, pipeline behaviors, and pre/post-processors.

You will need to configure two dependencies: first, the mediator itself. The other dependency is the factory delegate, `ServiceFactory`. The Mediator class is defined as:

```csharp
public class Mediator : IMediator
{
    public Mediator(ServiceFactory serviceFactory)
}
```

The factory delegates are named delegates around a couple of generic factory methods:

```csharp
public delegate object ServiceFactory(Type serviceType);
```

Declare whatever flavor of handler you need - sync, async or cancellable async. From the `IMediator` side, the interface is async-only, designed for modern hosts.

Finally, you'll need to register your handlers in your container of choice.



### StructureMap

Setup Mediator itself:
`cfg.For<IMediator>().Use<Mediator>()`. 

Setup the factory delegate:
`cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);`. This factory delegate is how TinyMediator builds instances of the request and notification handlers.

```csharp
new Container(cfg => cfg.Scan(scanner => {
    scanner.TheCallingAssembly();
    scanner.AddAllTypesOf(typeof(ISignalHandler<>));
});
```

The full example looks like:
```csharp
var container = new Container(cfg =>
{
    cfg.Scan(scanner =>
    {
        scanner.AssemblyContainingType<Ping>(); // Our assembly with requests & handlers
        scanner.ConnectImplementationsToTypesClosing(typeof(ISignalHandler<>));
    });
    cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
    cfg.For<IMediator>().Use<Mediator>();
});
```

### Autofac

The full example looks like:
```csharp
// Uncomment to enable polymorphic dispatching of requests, but note that
// this will conflict with generic pipeline behaviors
// builder.RegisterSource(new ContravariantRegistrationSource());

// Mediator itself
builder
    .RegisterType<Mediator>()
    .As<IMediator>()
    .InstancePerLifetimeScope();

// request & notification handlers
builder.Register<ServiceFactory>(context =>
{
    var c = context.Resolve<IComponentContext>();
    return t => c.Resolve(t);
});

// finally register our custom code (individually, or via assembly scanning)
// - requests & handlers as transient, i.e. InstancePerDependency()
// - pre/post-processors as scoped/per-request, i.e. InstancePerLifetimeScope()
// - behaviors as transient, i.e. InstancePerDependency()
builder.RegisterAssemblyTypes(typeof(MyType).GetTypeInfo().Assembly).AsImplementedInterfaces(); // via assembly scan
//builder.RegisterType<MyHandler>().AsImplementedInterfaces().InstancePerDependency();          // or individually
```

### Other

For more examples, check out the [samples](https://github.com/jbogard/TinyMediator/tree/master/samples) for working examples using:
* Autofac
* Castle Windsor
* DryIoc
* Lamar
* LightInject
* Ninject
* Simple Injector
* StructureMap
* Unity

These examples highlight all the features of TinyMediator including sync/async, request/response, pub/sub and more.

## Basics

TinyMediator has one kinds of messages it dispatches:
* Signal messages, dispatched to multiple handlers

### Signals

For signals, first create your notification message:

```csharp
public class Ping : ISignal { }
```

Next, create zero or more handlers for your notification:

```csharp
public class Pong1 : ISignalHandler<Ping>
{
    public Task Handle(Ping notification, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Pong 1");
        return Task.CompletedTask;
    }
}

public class Pong2 : ISignalHandler<Ping>
{
    public Task Handle(Ping notification, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Pong 2");
        return Task.CompletedTask;
    }
}
```

Finally, publish your message via the mediator:

```csharp
await mediator.Publish(new Ping());
```

#### Publish strategies

The default implementation of Publish loops through the notification handlers and awaits each one. This ensures each handler is run after one another.

Depending on your use-case for publishing signals, you might need a different strategy for handling the signals. Maybe you want to publish all signals in parallel, or wrap each notification handler with your own exception handling logic.

## Polymorphic dispatch

Handler interfaces are contravariant:

```csharp
public interface ISignalHandler<in TSignal>
{
    Task Handle(TSignal notification, CancellationToken cancellationToken);
}
```

Containers that support generic variance will dispatch accordingly. For example, you can have an `ISignalHandler<ISignal>` to handle all signals.

## Async
Publish is asynchronous from the `IMediator` side, with corresponding synchronous and asynchronous-based interfaces/base classes for signal handlers.

Your handlers can use the async/await keywords as long as the work is awaitable:

```csharp
public class PingHandler : ISignalHandler<Ping>
{
    public async Task Handle(Ping signal, CancellationToken cancellationToken)
    {
        await DoSomeThing(); 
    }
}
```
You will also need to register these handlers with the IoC container of your choice, similar to the synchronous handlers shown above.
