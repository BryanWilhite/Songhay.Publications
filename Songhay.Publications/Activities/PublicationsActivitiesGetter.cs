using Songhay.Models;
using System;
using System.Collections.Generic;

namespace Songhay.Publications.Activities
{
    public class PublicationsActivitiesGetter : ActivitiesGetter
    {
        public PublicationsActivitiesGetter(string[] args) : base(args)
        {
            this.LoadActivities(new Dictionary<string, Lazy<IActivity>>
            {
                {
                    nameof(Activities.MarkdownEntryActivity),
                    new Lazy<IActivity>(() => new Activities.MarkdownEntryActivity())
                }
            });
        }
    }
}
