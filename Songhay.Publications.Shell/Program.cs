﻿using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Activities;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Songhay.Publications.Shell.Tests")]

namespace Songhay.Publications.Shell;

class Program
{
    internal static void DisplayCredits()
    {
        Console.Write(ProgramAssemblyUtility.GetAssemblyInfo(Assembly.GetExecutingAssembly(), true));
        Console.WriteLine(string.Empty);
        Console.WriteLine("Activities Assembly:");
        Console.Write(ProgramAssemblyUtility.GetAssemblyInfo(typeof(PublicationsActivitiesGetter).Assembly, true));
    }

    internal static string? Run(string[] args)
    {
        var configuration = ProgramUtility.LoadConfiguration(Directory.GetCurrentDirectory());

        TraceSources.ConfiguredTraceSourceName = configuration[DeploymentEnvironment.DefaultTraceSourceNameConfigurationKey];

        var traceSource = TraceSources
            .Instance
            .GetTraceSourceFromConfiguredName()
            .WithSourceLevels();

        var getter = new PublicationsActivitiesGetter(args);
        var activity = getter.GetActivity().WithConfiguration(configuration);

        return getter.Args.IsHelpRequest() ?
            activity.DisplayHelp(getter.Args)
            :
            activity.StartActivity(getter.Args, traceSource);
    }

    static void Main(string[] args)
    {
        DisplayCredits();
        Console.WriteLine(Run(args));
    }
}
