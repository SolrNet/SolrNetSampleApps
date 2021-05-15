using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SampleSolrApp.Models;
using SolrNet;

namespace SampleSolrApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var solr = SolrNet.Startup.Container.GetInstance<ISolrReadOnlyOperations<Product>>();
            var results = await solr.QueryAsync("*:*");
            var model = new SearchResultsModel(results);
            return View(model);
        }
    }
}