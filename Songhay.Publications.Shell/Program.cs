using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Songhay;
using Songhay.Publications;
using Songhay.Publications.Hosting.Extensions;

static void DisplayCredits()
{
    Console.Write(ProgramAssemblyUtility.GetAssemblyInfo(Assembly.GetExecutingAssembly(), true));

    Console.WriteLine(string.Empty);

    Console.WriteLine("Activities Assembly:");
    Console.Write(ProgramAssemblyUtility.GetAssemblyInfo(typeof(PublicationContext).Assembly, true));

    Console.WriteLine(string.Empty);

    Console.WriteLine("IHost Assembly:");
    Console.Write(ProgramAssemblyUtility.GetAssemblyInfo(typeof(IHost).Assembly, true));
}

DisplayCredits();

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    string? activityName = hostContext.Configuration["activity-name"];

    services.AddLogging();

    services.AddPublicationsHostedService(activityName);
});

using IHost host = builder.Build();
host.Run();
