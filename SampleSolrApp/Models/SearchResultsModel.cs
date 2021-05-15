using System.Collections.Generic;

namespace SampleSolrApp.Models
{
    public record SearchResultsModel(
        IReadOnlyList<Product> Results
    );
}