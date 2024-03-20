# Better Startup Hooks for .NET
The low-level api for startup hooks does not include [dependency resolution for non-app assemblies](https://github.com/dotnet/runtime/blob/main/docs/design/features/host-startup-hook.md#no-dependency-resolution-for-non-app-assemblies). In other words hooks, dependencies to other assemblies than included in the injected application need extra dependency resolution logic.  

*Better-Startup-Hook allows to create startup hooks that automatically resolves additional dependencies.*

### How-does it work?
The project consists of a startup hook in ```BetterStartupHook.dll```, whichs loads the actual logic to inject into the application from other assemblies and does all the extra logic to resolve the dependencies. 

The BETTER_STARTUP_HOOKS environment variable can be used to specify a list of additional managed assemblies that contain a ```BetterStartupHook``` type with a ```public static void Initialize()``` method, each of which will be called in the order specified, before the Main entry point. 


```
using System;

internal class BetterStartupHook
{
    public static void Initialize()
    {
       //Your custom logic.. 
    }
}
```

### How-To create/use a BetterStartupHook 
#### 1. Implement your hook by creating a .Net class library project e.g. 

```
using OpenTelemetry;
using OpenTelemetry.Trace;
using System;

internal class BetterStartupHook
{
    public static void Initialize()
    {
        //Your custom logic goes here...
        Console.WriteLiine("Hello World");
    }
}
```
#### 2. Extend your .csproj to copy nuget references into your output folder

Add ```CopyLocalLockFileAssemblies``` as shown in the following

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
  </PropertyGroup>

    ...
</Project>
```

Read more about [CopyLocalLockFileAssemblies property](https://learn.microsoft.com/de-de/dotnet/core/project-sdk/msbuild-props#copylocallockfileassemblies)

#### 3. Register your better-startup-hook assembly 
Windows:

```BETTER_STARTUP_HOOKS="<path-to-betterstartuphook>\\MyStartupHook.dll```

Linux:

```BETTER_STARTUP_HOOKS="<path-to-betterstartuphook>\\MyStartupHook.dll```

#### 4. Register the BetterStartupHook assembly as your applications startup hook. 
Windows:

```DOTNET_STARTUP_HOOKS="<path-to-betterstartuphook>\\BetterStartupHook.dll```

Linux:

```DOTNET_STARTUP_HOOKS="<path-to-betterstartuphook>/BetterStartupHook.dll```

## Contribute
This is an open source project, and we gladly accept new contributions and contributors.  

## License
Licensed under Apache 2.0 license. See [LICENSE](LICENSE) for details.