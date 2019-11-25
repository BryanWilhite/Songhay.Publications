using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Activities;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Songhay.Publications.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayCredits();

            var configuration = ProgramUtility.LoadConfiguration(Directory.GetCurrentDirectory());
            TraceSources.ConfiguredTraceSourceName = configuration[DeploymentEnvironment.DefaultTraceSourceNameConfigurationKey];
            using (var listener = new TextWriterTraceListener(Console.Out))
            {
                ProgramUtility.InitializeTraceSource(listener);

                var getter = new PublicationsActivitiesGetter(args);
                var activity = getter.GetActivity();

                if (getter.Args.IsHelpRequest())
                    Console.WriteLine(activity.DisplayHelp(getter.Args));
                else
                    activity.Start(getter.Args);

                listener.Flush();
            }
        }

        internal static void DisplayCredits()
        {
            Console.Write(FrameworkAssemblyUtility.GetAssemblyInfo(Assembly.GetExecutingAssembly(), true));
            Console.WriteLine(string.Empty);
            Console.WriteLine("Activities Assembly:");
            Console.Write(FrameworkAssemblyUtility.GetAssemblyInfo(typeof(PublicationsActivitiesGetter).Assembly, true));
        }
    }
}
