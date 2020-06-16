using Microsoft.Extensions.Configuration;
using Songhay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Songhay.Publications.Activities
{
    public class IndexActivity : IActivity, IActivityConfigurationSupport
    {
        public void AddConfiguration(IConfigurationRoot configuration)
        {
            throw new NotImplementedException();
        }

        public string DisplayHelp(ProgramArgs args)
        {
            throw new NotImplementedException();
        }

        public void Start(ProgramArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
