using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleSolrApp.Models
{
    public class SearchParameters
    {
        public string Q { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string Sort { get; set; }
        public Dictionary<string, string> Filters { get; set; } = new();
        
        public const int DefaultPageSize = 5;

        public int EffectivePageIndex => PageIndex ?? 1;
        public int Start => (EffectivePageIndex - 1) * EffectivePageSize;
        public int EffectivePageSize => PageSize ?? DefaultPageSize;
        public IReadOnlyList<string> FilteredFields => Filters?.Select(f => f.Key).ToArray() ?? Array.Empty<string>();

        public override string ToString() =>
            $"{nameof(Q)}: {Q}, " +
            $"{nameof(PageIndex)}: {PageIndex}, " +
            $"{nameof(PageSize)}: {PageSize}, " +
            $"{nameof(Sort)}: {Sort}, " +
            $"{nameof(Filters)}: {string.Join(",", Filters.Select(f => $"{f.Key}={f.Value}"))}";
    }
}