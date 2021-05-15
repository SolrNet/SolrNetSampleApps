using System;
using System.Collections.Generic;
using SolrNet.Attributes;

namespace SampleSolrApp.Models
{
    public class Product
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("sku")]
        public string SKU { get; set; }

        [SolrField("name")]
        public ICollection<string> Name { get; set; }

        [SolrField("manu_exact")]
        public string Manufacturer { get; set; }

        [SolrField("cat")]
        public ICollection<string> Categories { get; set; }

        [SolrField("features")]
        public ICollection<string> Features { get; set; }

        [SolrField("price")]
        public decimal Price { get; set; }

        [SolrField("popularity")]
        public ICollection<int> Popularity { get; set; }

        [SolrField("inStock")]
        public ICollection<bool> InStock { get; set; }

        [SolrField("timestamp")]
        public DateTime Timestamp { get; set; }

        [SolrField("weight")]
        public ICollection<double?> Weight { get; set;}        
    }
}