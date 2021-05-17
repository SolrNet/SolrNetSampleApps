using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SampleSolrApp.Models;

namespace SampleSolrApp.Helpers
{
    public static class UrlHelperExtensions
    {
        [Pure]
        public static string SetFacet(this IUrlHelper helper, string field, string value) =>
            helper.SetParameters(new Dictionary<string, string>
            {
                {nameof(SearchParameters.PageIndex), 1.ToString()},
                {$"{nameof(SearchParameters.Filters)}[{field}]", value},
            });

        /// <summary>
        /// Sets/changes an URL's query string parameters.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="parameters">Parameters to set/change</param>
        /// <returns>Resulting URL</returns>
        [Pure]
        public static string SetParameters(this IUrlHelper helper, IDictionary<string, string> parameters) {
            var qs = QueryHelpers.ParseQuery(helper.ActionContext.HttpContext.Request.QueryString.Value ?? "");
            foreach (var p in parameters)
                qs[p.Key] = p.Value;
            return helper.ActionContext.HttpContext.Request.Path + QueryString.Create(qs);
        }

        [Pure]
        public static string SetPageIndex(this IUrlHelper helper, SearchParameters parameters, int pageIndex) => 
            helper.SetParameters(new Dictionary<string, string>
            {
                {nameof(SearchParameters.PageIndex), pageIndex.ToString()}
            });

        [Pure]
        public static string RemoveFilter(this IUrlHelper helper, string filter) =>
            helper.RemoveParametersUrl($"{nameof(SearchParameters.Filters)}[{filter}]");

        /// <summary>
        /// Removes parameters from an url's query string
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="parameters">Query string parameter keys to remove</param>
        /// <returns>Resulting URL</returns>
        [Pure]
        public static string RemoveParametersUrl(this IUrlHelper helper, params string[] parameters) {
            var qs = QueryHelpers.ParseQuery(helper.ActionContext.HttpContext.Request.QueryString.Value ?? "");
            foreach (var p in parameters)
                qs.Remove(p);
            return helper.ActionContext.HttpContext.Request.Path + QueryString.Create(qs);
        }
    }
}