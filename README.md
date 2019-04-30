# .Net Core DI Scope is not associated with Web Request

I had a chance to work with someone briefly who is a manager and as well as writes code. He was sort of "I know everything and everyone else does not or has to learn from me". So I had no the option of working independently following patterns and practices, my knowledge and experience. Either I had to provide documentation from external source as to why I did things in a certain way or got into argument. The first incident happened when I was given task on an existing a .Net Core hosted service ([HostedServices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2)) project. The entire application was using the default dependency injection from the .net core. However, in the particular project I was working on, objects were created manually. The code looks like as shown below

```csharp
 
public class HelloContext
{
   private ISomething _smh ; 
     void Foo(string key)
    {
       switch(key)
        {
            case "A":
            _smh = new SomethingA();
            case "B"
            _smh = new SomethingB();
            default:
            throw new Exception();
        }
        _smh.ShakingMyHead();
    }
}
```
As you can see, this piece of code violates the open close principle because of just an ego; If I have to include `SomethingC` implementation of `ISomething`, I have to modify this code. I asked the manager/developer, why they did it that way, and why they didn't use the ([IServiceScopeFactory](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicescopefactory?view=aspnetcore-2.2)) to create those object dynamically. He told me that DI lifetime Scope in .net core is tied to Per Web Request and the particular hosted service has nothing to do with web request. So I was told to leave it as is and just modify it for new implementation. 
However, the Scope lifetime is not tied to a Web Request at all, you can use it in any unit of work which could be accessing database, create azure blobs, or even writing out to a console. Scope in .net core is a DI lifetime coordinated by ([IServiceScopeFactory](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicescopefactory?view=aspnetcore-2.2)) which implements `IDisposable` and creates objects that are registered in the DI `ConfigureServices` as Scope (`AddScoped()`). You can use  `IServiceScopeFactory` in a `using` block to create a scope and get create a scoped object which will be disposed after the block and will be recreated when you get back to the block of code again. 

## Implementing Hosted Service with Strategy Pattern and Scoped DI

The purpose of this article is so to show you that you can use scoped DI objects in the generic host (`IHost`) without any web application/web host and how to implement a hosted background service with strategy pattern in .net core

### .Net core background services
In .net core, background services can be implemented using hosted services. A hosted service in .net core is just a class that implements [IHostedService](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostedservice?view=aspnetcore-2.2). Background services might do a number of tasks which could be scheduled tasks (timer based), queue based tasks etc.  A hosted service can be hosted using Web Host ([IWebHostBuilder](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.iwebhostbuilder?view=aspnetcore-2.2)) or Generic host ([IHostBuilder](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostbuilder?view=aspnetcore-2.2)) (.net core version 2.1 and above). In this article, we will be using the generic host without any web app associated with. If you are not familiar about hosted services, I would recommend reading [Background tasks with hosted services](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2) before continuing reading this article but you can still skip it.

Implementations of IHostedService are registered them at the `ConfigureService()` method into the DI container. All those hosted services will be started and stopped along with the application. Therefore, a little extra caution should be taken when using DI in hosted service. For example, if you use a constructor injection and use a scoped service, it will automatically become singleton - will live the entire lifetime of the application.

The benefits of Scoped lifetime is to make an object short lived, create it when you use it and dispose it after. Scoped services make sense for services that hold resources, for example, a database connection context, a web socket, etc that you don't want to be singleton.
If you closely, look at the above code snippet, you can easily match it to fit the Strategy Pattern. The .net core hosted service can be efficiently implemented using a strategy pattern. In the following section, we will develop a hosted service using .net core with strategy pattern, using the default DI service provided by the framework. The complete code can be found at ([version-1](https://github.com/danielhunex/hostedservice-dotnetcore/tree/master/Version-1)) and ([version-2](https://github.com/danielhunex/hostedservice-dotnetcore/tree/master/Version-2)) on github.

>In computer programming, the strategy pattern (also known as the policy pattern) is a behavioral software design pattern that enables selecting an algorithm at runtime. Instead of implementing a single algorithm directly, code receives run-time instructions as to which in a family of algorithms to use. ([Wikipedia](https://en.wikipedia.org/wiki/Strategy_pattern))

As you can understand from the above strategy pattern definition, we can perfectly fit the above code snippet into this pattern, we just have to dynamically get the right algorithm ( the right implemention of ISomething). 

In this article, we will build a simple console application with a hosted service that uses the advantage of scope DI and strategy pattern as shown in the class diagram. We will some manual 'queuing' system to trigger the execution. However, in real application, this could triggered by RabbitMQ or whatever kind of queue you want.
![Hosted Service with Strategy pattern](https://github.com/danielhunex/hostedservice-dotnetcore/blob/master/strategy-pattern.PNG "strategy pattern hosted service")

### Prerequisite
In order to follow up this project, you need vs code, .net core 2.2 or above.

Let's do it

1. Create a folder structure as  ***hostedservice->src***
2. From your favourite terminal (I m using Windows command Promopt), navigate to the the folder structure you just created
    `dotnet new console --name HostedService`
This will create a .net core console application. In vs code it looks like the following

![Code structure](https://github.com/danielhunex/hostedservice-dotnetcore/blob/master/code-structure.PNG)
  
3. Add the following class to the ***src***
```csharp
using System.Threading.Tasks;

namespace HostedService
{
    public interface IStrategy
    {
        Task ExecuteAsync();
    }
}
```
And the foolwing concrete implementations
```csharp
using System;
using System.Threading.Tasks;
namespace HostedService
{
    public class StrategyA : IStrategy
    {
        public StrategyA()
        {
            Console.WriteLine("...StrategyA Created...");
        }
        public async Task ExecuteAsync()
        {
            await Task.Run(() => Console.WriteLine("StrategyA: Executing"));
        }
    }
}

using System;
using System.Threading.Tasks;
namespace HostedService
{
    public class StrategyB : IStrategy
    {
        public StrategyB()
        {
            Console.WriteLine("...StrategyB Created...");
        }
        public async Task ExecuteAsync()
        {
            await Task.Run(() => Console.WriteLine("StrategyB: Executing"));
        }
    }
}

using System;
using System.Threading.Tasks;

namespace HostedService
{
    public class StrategyC : IStrategy
    {
        public StrategyC()
        {
            Console.WriteLine("...StrategyC Created...");
        }
        public async Task ExecuteAsync()
        {
            await Task.Run(() => Console.WriteLine("StrategyC: Executing"));
        }
    }
}

```
4. Now lets create a hosted service- `HostedServiceContext`. Here we are not directly inheriting from IHostedService but from BackgroundService which implmenents the interface IHostedService, and we will implement the ExecuteAsync method

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HostedService
{
    public class HostedServiceContext : BackgroundService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        public HostedServiceContext(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
        }
    }
}
```
This class is right is pretty bare but see that we are using the `IServiceScopeFactory` interface and the concrete implementation will be injected.

5. Now let's get into the program.cs file and host our background service, register classes for DI. We will use `HostBuilder` to add interfaces/classes for DI and to create a host as shown the below
```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HostedService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddOptions();
                services.AddHostedService<HostedServiceContext>();
                services.AddScoped<StrategyA>();
                services.AddScoped<StrategyB>();
                services.AddScoped<StrategyC>();
            });
            await builder.RunConsoleAsync();
        }
    }
}
```
 ### So far
 We created an interface `IStrategy` and added three concrete implementations `StrategyA`, `StrategyB`,`StrategyC`. As you can see each of the concrete classes has a console output in its constructor. Its purpose is just to show the each execution of a scope will create a new instance of that concrete class and we will see something like '...StrategyC Created...' everytime StrategyC dynamically is selected and created. The `Program` is all set registering objects and hosting the service.
 We also created `HostedServiceContext` which we will add more to it. What we are going to do in this classes is
  1. Use the `IServiceScopeFactory` to create a scope and create the right `IStrategy` based on a queue input
  2. Call the ExecuteAsync method of the Strategy selected.

First lets implement a pseudo-queue service that we will use to drive the execution of the background service

