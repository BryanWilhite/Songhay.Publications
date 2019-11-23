using Songhay.Models;
using System;
using System.Collections.Generic;

namespace Songhay.Publications.Models
{
    [Obsolete("Use RestApiMetadata instead.")]
    public class GenericWebApiMetadata
    {
        public RestApiMetadata ApiMetadata { get; set; }

        public Dictionary<string, string> ClaimsSet { get; set; }
    }
}
