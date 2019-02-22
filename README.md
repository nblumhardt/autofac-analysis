# `Autofac.Analysis` [![NuGet package](https://img.shields.io/nuget/vpre/autofac.analysis.svg)](https://nuget.org/packages/autofac.analysis) [![Build status](https://ci.appveyor.com/api/projects/status/r0a0n6b17080we1i?svg=true)](https://ci.appveyor.com/project/NicholasBlumhardt/autofac-analysis)

Log and analyze runtime operations on an [Autofac](https://autofac.org) container.

This project can:

 * Detect common usage problems, including patterns that lead to unbounded memory growth,
 * Provide a detailed view of how and where an Autofac container is being used within an application, and
 * Surface component graph information, such as the sharing relationships between components.

Analysis has a significant impact on the host application; it's advisable to perform analysis during development and testing rather than on a live production system.

## Getting started

**1. Install the [`Autofac.Analysis` NuGet package](https://nuget.org/packages/autofac.analysis)**

```
dotnet add package Autofac.Analysis
```

**2. Construct a [Serilog](https://serilog.net) `ILogger` to receive analysis events**

Serilog supports a large range of sinks; here, we send analysis to the console/`STDOUT`, but you
will see more detailed information if you choose a sink that can receive structured data.

```csharp
// dotnet add package Serilog.Sinks.Console
var logger = new LoggerConfiguration()
    .WriteTo.Console()
	.CreateLogger();
```

If your application already uses Serilog, you may choose to send analysis events through the same 
pipeline. There are a lot of them, though! Setting a minimum level override can help to control
the noise:

```csharp
var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Autofac.Analysis", LogEventLevel.Warning)
	// ...
```

**3. Register the module in your Autofac container**

```csharp
builder.RegisterModule(new AnalysisModule(logger));
```

The module has a significant performance impact and should not be registered in normal production
settings. You should consider including the registration conditionally in `DEBUG` builds.

## Analyses

### `A001` - A lifetime scope was disposed after its parent

Since components in the child scope can see components in the parent scope, this may lead to problems including `ObjectDisposedException`s at runtime.

### `A002` - A non-root lifetime scope was active for more than 15 seconds

Long-lived scopes may indicate a failure to properly dispose the scope after use, in which case tracked components may not be disposed, and work to clean up unmanaged resources may be forced onto the finalizer thread.

### `A003` - A non-singleton (`InstancePerDependency`) component was resolved multiple times directly from the container

This usage pattern can lead to memory leaks when tracked/`IDisposable` components are introduced.

### `A004` - A non-singleton tracked/`IDisposable` component was resolved directly from the container

This usage pattern often indicates memory leaks; instances of the component will remain in memory for the life of the container.

