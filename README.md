**.Net Core DI Scope is not associated with Web Request**

I had a chance to work with someone briefly who is a manager and as well as writes code. He was sort of "I know everything and everyone else does not or has to learn from me". So I had no the option of working independetly following patterns and practices, my knowledge and experience. Either I had to provide documentation from external source as to why I did thing or got into argument. I was given task on an existing a .Net Core BackgroundService ([HostedServices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2)) project. The entire application was using the default dependency injection from the .net core. However, in the particular background service project I was working on, objects were created manually. I was about to add more to it. The code looks like as shown below

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
However, this code completely violates the open close principle becasue of just an ego; you have come to this code and modify whenever you have a new implementation of ISomething. I asked the manager/developer, why they did it that way, and why they didn't use the ([IServiceScopeFactory](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicescopefactory?view=aspnetcore-2.2)) to create those object dynamically. He told me that DI lifetime Scope in .net core is tied to Per Request and the particular hosted service has nothing to do with web request. I know the Scope is not tied to a Web Request, you can use it in any unit of work which could be accessing database, create azure blobs, or even writing out to a console. Scope in .net core is a DI lifetime provided by ([IServiceScopeFactory](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicescopefactory?view=aspnetcore-2.2)) which implements `IDisposable` and since it is a factory, it creates objects that are registered in the DI ServiceCollection as Scope (`AddScoped()`).  When you use  `IServiceScopeFactory` in a `using` block to create a scope and get an instance of a particular scoped object. The object created by the factory in the `using` block will be disposed after. But I was told that is per request and otherwise I had to show documentation, do argument argument etc (such a few arguments happened with different issues, which inspire me to write article and show what I think is write implemention)

If you closely, look at the above code snippet, you can easily match it to fit the Strategy Pattern. The .net core hosted service can be efficiently implemented using a strategy pattern. In the following section, we will develop a hosted service using .net core with strategy pattern, using the default DI service provided by the framework. The complete code can be found at ([version-1](https://github.com/danielhunex/hostedservice-dotnetcore/tree/master/Version-1)) and ([version-2](https://github.com/danielhunex/hostedservice-dotnetcore/tree/master/Version-2)) on github.

>In computer programming, the strategy pattern (also known as the policy pattern) is a behavioral software design pattern that enables selecting an algorithm at runtime. Instead of implementing a single algorithm directly, code receives run-time instructions as to which in a family of algorithms to use. ([Wikipedia](https://en.wikipedia.org/wiki/Strategy_pattern))

As you can understand from the above strategy pattern definition, we can perfectly fit the above code snippet into this pattern, we just have to dynamically get the right algorithm ( the right implemention of ISomething). 

**.Net core background services**
In .net core, background services can be implemented using hosted services. A hosted services in .net core is just a class that implements [IHostedService](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostedservice?view=aspnetcore-2.2). The class might do a number of background tasks which could be scheduled tasks (timer based), queue based tasks etc.  A hosted service can be hosted using Web Host ([IWebHostBuilder](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.iwebhostbuilder?view=aspnetcore-2.2)) or Generic host ([IHostBuilder](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostbuilder?view=aspnetcore-2.2)) (.net core version 2.1 and above). In this article, I will be using the generic host ( after all I am trying to show that Scoped lifetime is not tied to web request). If you are not familiar about hosted services, I would recommend reading [Background tasks with hosted services](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2) before continuing reading this article but you can still skip it.



