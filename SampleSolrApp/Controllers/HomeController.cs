using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SampleSolrApp.Models;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace SampleSolrApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISolrReadOnlyOperations<Product> _solr;

        public HomeController(ILogger<HomeController> logger, ISolrReadOnlyOperations<Product> solr)
        {
            _logger = logger;
            _solr = solr;
        }

        public async Task<IActionResult> Index([FromQuery] SearchParameters parameters)
        {
            _logger.LogInformation("Search: " + parameters);
            var results = await _solr.QueryAsync(BuildQuery(parameters), BuildQueryOptions(parameters));
            var model = new SearchResultsModel(results, BuildFacetModels(results.FacetFields), results.NumFound, parameters);
            return View(model);
        }
        
        [Pure]
        static QueryOptions BuildQueryOptions(SearchParameters parameters) =>
            new QueryOptions
            {
                Rows = parameters.EffectivePageSize,
                StartOrCursor = new StartOrCursor.Start(parameters.Start),
                FilterQueries = BuildFilterQueries(parameters),
                Facet = new FacetParameters
                {
                    Queries = AllFacetFields.Keys.Except(parameters.FilteredFields)
                        .Select(f => (ISolrFacetQuery)new SolrFacetFieldQuery(f) {MinCount = 1})
                        .ToList()
                }
            };

        [Pure]
        static IReadOnlyList<FacetModel> BuildFacetModels(IDictionary<string, ICollection<KeyValuePair<string, int>>> facetResults) =>
            facetResults
                .Select(f => new FacetModel(
                    Key: f.Key, 
                    FriendlyName: AllFacetFields[f.Key], 
                    Values: f.Value.Select(v => new FacetValueModel(v.Key, v.Value)).ToList()
                ))
                .ToList();

        /// <summary>
        /// Builds the Solr query from the search parameters
        /// </summary>
        [Pure]
        static ISolrQuery BuildQuery(SearchParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters?.Q))
                return SolrQuery.All;
            return new LocalParams
            {
                {"type", "dismax"},
                {"qf", string.Join(" ", new[] {"manu", "cat", "features", "name^5"})}
            } + new SolrQuery(parameters.Q);
        }

        public static readonly IReadOnlyDictionary<string, string> AllFacetFields =
            new Dictionary<string, string>
            {
                {"cat_str", "Category"},
                {"manu_str", "Manufacturer"},
            };

        [Pure]
        static ICollection<ISolrQuery> BuildFilterQueries(SearchParameters parameters) =>
            (parameters.Filters ?? new Dictionary<string, string>())
            .Select(f => (ISolrQuery) new SolrQueryByField(f.Key, f.Value))
            .ToList();

    }
}