using Statiq.Common;
using Statiq.App;
using Statiq.Docs;
using Statiq.Web;

await Bootstrapper
    .Factory
    .CreateDocs(args)
    .AddExcludedPath("../../../../Statiq.Docs.Shell")
    .AddSettings(new Dictionary<string, object>
    {
        { Keys.Title, "API" },
        { Keys.LinkRoot, "Songhay.Publications" },
        { DocsKeys.ApiPath, "latest" },
        {
            DocsKeys.SourceFiles,
            new []
            {
                "../../../../../Songhay.Publications/**/{!.git,!bin,!obj,!packages,!*.Tests,}/**/*.cs",
            }
        },
        { DocsKeys.OutputApiDocuments, true },
        { WebKeys.Author, "@BryanWilhite" },
        { WebKeys.Copyright, $"(c) {DateTime.Now.Year} Songhay System" },
        { WebKeys.GitHubName, "Bryan Wilhite" },
        { WebKeys.GitHubUsername, "BryanWilhite" },
        { WebKeys.OutputPath, "../../../../docs" },
    })
    .RunAsync();
