using System.IO;
using System;
using System.Reflection;
using System.Diagnostics;
using BetterStartupHook;

internal class StartupHook
{
    private const string StartupHooks = "BETTER_STARTUP_HOOKS";
    private const string StartupHookTypeName = "BetterStartupHook";
    private const string InitializeMethodName = "Initialize";

    public static void ExecuteStartupHook(Assembly assembly)
    {

        Debug.Assert(assembly != null);
        Type type = assembly.GetType(StartupHookTypeName, throwOnError: true)!;

        // Look for a static method without any parameters
        MethodInfo? initializeMethod = type.GetMethod(InitializeMethodName,
                                                     BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                                                     null, // use default binder
                                                     Type.EmptyTypes, // parameters
                                                     null); // no parameter modifiers
        if (initializeMethod == null)
        {
            // There weren't any static methods without
            // parameters. Look for any methods with the correct
            // name, to provide precise error handling.
            try
            {
                // This could find zero, one, or multiple methods
                // with the correct name.
                MethodInfo? wrongSigMethod = type.GetMethod(InitializeMethodName,
                                                  BindingFlags.Public | BindingFlags.NonPublic |
                                                  BindingFlags.Static | BindingFlags.Instance);
                // Didn't find any
                if (wrongSigMethod == null)
                {
                    throw new MissingMethodException(StartupHookTypeName, InitializeMethodName);
                }
            }
            catch (AmbiguousMatchException)
            {
                // Found multiple. Will throw below due to initializeMethod being null.
                Debug.Assert(initializeMethod == null);
            }
        }

        // Found Initialize method(s) with non-conforming signatures
        if (initializeMethod == null || initializeMethod.ReturnType != typeof(void))
        {
            throw new ArgumentException("Couldn't find startuphook");
        }

        Debug.Assert(initializeMethod != null &&
                     initializeMethod.IsStatic &&
                     initializeMethod.ReturnType == typeof(void) &&
                     initializeMethod.GetParameters().Length == 0);

        initializeMethod.Invoke(null, null);
    }

    public static void TryExecuteStartupHooks()
    {
        var startupAssemblies = Environment.GetEnvironmentVariable(StartupHooks, EnvironmentVariableTarget.Process);
        if (!String.IsNullOrEmpty(startupAssemblies))
        {
            foreach (var startupAssembly in startupAssemblies.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var loadContext = new SharedAssemblyLoadContext(startupAssembly);
                var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(startupAssembly)));

                if (assembly == null)
                    Console.WriteLine("[BetterStartupHooks] Couldn't load " + startupAssembly);
                else
                {
                    try
                    {
                        ExecuteStartupHook(assembly);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[BetterStartupHooks] Error in startuphook " + startupAssembly + ":  " + ex.ToString());
                    }
                }
            }
        }
    }

    public static void Initialize()
    {
        TryExecuteStartupHooks();
    }
}
