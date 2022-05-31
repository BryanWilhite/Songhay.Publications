using System;
using System.Collections.Generic;
using System.Text.Json;
using Songhay.Publications.Models;

namespace Songhay.Publications.Extensions
{
    /// <summary>
    /// Extensions of <see cref="IIndexEntry"/>.
    /// </summary>
    public static class IndexEntryExtensions
    {
        /// <summary>
        /// Converts the <see cref="IEnumerable{IIndexEntry}"/>
        /// to a JSON <see cref="string"/>
        /// with conventional <see cref="JsonSerializerOptions"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(this IEnumerable<IIndexEntry> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = true,
            };

            var json = JsonSerializer
                .Serialize<IEnumerable<IIndexEntry>>(data, options);

            return json;
        }
    }
}