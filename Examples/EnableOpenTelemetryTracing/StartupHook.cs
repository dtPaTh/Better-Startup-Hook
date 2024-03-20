using OpenTelemetry;
using OpenTelemetry.Trace;
using System;

internal class BetterStartupHook
{
    public static void Initialize()
    {
        
        var traceProvider = Sdk.CreateTracerProviderBuilder()
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation()
                       .AddConsoleExporter()
                       .Build();

        Console.WriteLine("BetterStartupHook::EnableOpenTelemetry executed.");
    }
}