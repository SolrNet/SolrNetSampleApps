using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleSolrApp.Models
{
    public record FacetValueModel(
        string Value,
        int Count
    );
    
    public record FacetModel(
        string Key,
        string FriendlyName,
        IReadOnlyList<FacetValueModel> Values
    );
    
    public record SearchResultsModel(
        IReadOnlyList<Product> Results,
        IReadOnlyList<FacetModel> Facets,
        long NumFound,
        SearchParameters Parameters
    )
    {
        const int PageSlide = 2;
        
        public IEnumerable<int> Pages {
            get
            {
                var pageCount = LastPage;
                var pageFrom = Math.Max(1, Parameters.EffectivePageIndex - PageSlide);
                var pageTo = Math.Min(pageCount - 1, Parameters.EffectivePageIndex + PageSlide);
                pageFrom = Math.Max(1, Math.Min(pageTo - 2*PageSlide, pageFrom));
                pageTo = Math.Min(pageCount, Math.Max(pageFrom + 2*PageSlide, pageTo));
                return Enumerable.Range(pageFrom, pageTo-pageFrom+1);                
            }
        }
        
        public int LastPage => (int) Math.Floor(((decimal) NumFound - 1) / Parameters.EffectivePageSize) + 1;
    }
}