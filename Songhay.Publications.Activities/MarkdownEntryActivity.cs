using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Models;
using System;
using System.IO;

namespace Songhay.Publications.Activities
{
    public class MarkdownEntryActivity : IActivity
    {
        public string DisplayHelp(ProgramArgs args)
        {
            throw new NotImplementedException();
        }

        public void Start(ProgramArgs args)
        {
            this._presentationShellInfo = new DirectoryInfo(args.GetArgValue(ProgramArgs.BasePath));
            this.CheckPresentationShellInfo();
        }

        internal void CheckPresentationShellInfo()
        {
            if (!this._presentationShellInfo.Name.EqualsInvariant(MarkdownPresentationDirectories.DirectoryNamePresentationShell))
                throw new DirectoryNotFoundException($"The expected Presentation Shell directory is not here. [actual: { this._presentationShellInfo?.Name ?? "[name]" }");
        }

        DirectoryInfo _presentationShellInfo;
    }
}
