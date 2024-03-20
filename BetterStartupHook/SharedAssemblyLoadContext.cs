using System.IO;
using System;
using System.Reflection;
using System.Runtime.Loader;

namespace BetterStartupHook
{

    public class SharedAssemblyLoadContext : AssemblyLoadContext
    {
        AssemblyDependencyResolver _resolver;

        public SharedAssemblyLoadContext(string? assemblyPath = default)
        {
            var location = Assembly.GetExecutingAssembly().Location; //Default location

            if (!String.IsNullOrEmpty(assemblyPath))
            {
                if (File.Exists(assemblyPath))
                    location = assemblyPath;
                else
                    Console.WriteLine("SharedAssemblyLoadContext::Could not find '" + assemblyPath + "'");
            }

            _resolver = new AssemblyDependencyResolver(location);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

            if (assemblyPath != null)
                return LoadFromAssemblyPath(assemblyPath);
            else
#pragma warning disable CS8603 //Possible null reference return.
                return null;
#pragma warning restore CS8603 //Possible null reference return.

        }
    }

}
