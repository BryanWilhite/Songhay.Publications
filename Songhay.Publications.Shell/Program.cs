﻿using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Songhay.Publications.Hosting.Extensions;

[assembly: InternalsVisibleTo("Songhay.Publications.Shell.Tests")]

namespace Songhay.Publications.Shell;

class Program
{
    internal static void DisplayCredits()
    {
        Console.Write(ProgramAssemblyUtility.GetAssemblyInfo(Assembly.GetExecutingAssembly(), true));
        Console.WriteLine(string.Empty);
        Console.WriteLine("Activities Assembly:");
        Console.Write(ProgramAssemblyUtility.GetAssemblyInfo(typeof(PublicationContext).Assembly, true));
    }

    static void Main(string[] args)
    {
        DisplayCredits();

        IHostBuilder builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureServices((hostContext, services) =>
        {
            string activityName = hostContext.Configuration["activity-name"];

            services.AddLogging();

            services.AddPublicationsHostedService(activityName);
        });

        using IHost host = builder.Build();
        host.Run();
    }
}
