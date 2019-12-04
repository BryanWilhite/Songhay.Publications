using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Extensions;
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
            var presentationShellInfo = new DirectoryInfo(args.GetArgValue(ProgramArgs.BasePath));
            presentationShellInfo.IsConventionalMarkdownPresentationDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationShell);

            this._presentationInfo = presentationShellInfo.Parent;
            this._presentationInfo.HasAllConventionalMarkdownPresentationDirectories();
        }

        DirectoryInfo _presentationInfo;
    }
}
