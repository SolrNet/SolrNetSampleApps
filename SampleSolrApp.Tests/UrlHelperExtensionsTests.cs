#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using Xunit;
using SampleSolrApp.Helpers;

namespace SampleSolrApp.Tests {
    public class UrlHelperExtensionsTests {

        [Fact]
        public void ParseQueryString_empty() {
            var d = UrlHelperExtensions.ParseQueryString("");
            Assert.Equal(0, d.Count);
        }

        [Fact]
        public void ParseQueryString_null() {
            var d = UrlHelperExtensions.ParseQueryString(null);
            Assert.Equal(0, d.Count);
        }

        [Fact]
        public void ParseQueryString_only_question_mark() {
            var d = UrlHelperExtensions.ParseQueryString("?");
            Assert.Equal(0, d.Count);            
        }

        [Fact]
        public void ParseQueryString_admits_question_mark() {
            var d = UrlHelperExtensions.ParseQueryString("?pep=1");
            Assert.Equal(1, d.Count);
            Assert.Equal("1", d["pep"]);
        }

        [Fact]
        public void ParseQueryString_admits_no_question_mark() {
            var d = UrlHelperExtensions.ParseQueryString("pep=1");
            Assert.Equal(1, d.Count);
            Assert.Equal("1", d["pep"]);
        }

        [Fact]
        public void ParseQueryString_is_case_insensitive() {
            var d = UrlHelperExtensions.ParseQueryString("?pep=1&Nothing=bla");
            Assert.Equal(2, d.Count);
            Assert.Equal("1", d["PeP"]);
            Assert.Equal("1", d["pep"]);
            Assert.Equal("bla", d["nothing"]);
        }

        [Fact]
        public void ParseQueryString_url_decodes() {
            var d = UrlHelperExtensions.ParseQueryString("?pep=1&Nothing=%3D%25");
            Assert.Equal(2, d.Count);
            Assert.Equal("=%", d["nothing"]);
        }

        [Fact]
        public void ParseQueryString_admits_empty_parameters() {
            var d = UrlHelperExtensions.ParseQueryString("?pep=&Nothing=");
            Assert.Equal(2, d.Count);
            Assert.Equal("", d["pep"]);
            Assert.Equal("", d["Nothing"]);
        }

        [Fact]
        public void ParseQueryString_admits_extra_ampersands() {
            var d = UrlHelperExtensions.ParseQueryString("?&pep=&&&Nothing=&");
            Assert.Equal(2, d.Count);
            Assert.Equal("", d["pep"]);
            Assert.Equal("", d["Nothing"]);
        }

        [Fact]
        public void ParseQueryString_admits_duplicate_parameters_but_keeps_last() {
            var d = UrlHelperExtensions.ParseQueryString("pep=1&pep=2");
            Assert.Equal(1, d.Count);
            Assert.Equal("2", d["pep"]);
        }

        [Fact]
        public void DictToQuerystring_empty() {
            var s = UrlHelperExtensions.DictToQuerystring(new Dictionary<string, string>());
            Assert.Equal("", s);
        }

        [Fact]
        public void DictToQuerystring_empty_key() {
            var s = UrlHelperExtensions.DictToQuerystring(new Dictionary<string, string> {
                {"", "a"},
            });
            Assert.Equal("", s);
        }

        [Fact]
        public void DictToQuerystring_url_encodes() {
            var s = UrlHelperExtensions.DictToQuerystring(new Dictionary<string, string> {
                {"pp", "=="},
            });
            Assert.Equal("pp=%3d%3d", s);
        }

        [Fact]
        public void DictToQuerystring_many_params() {
            var s = UrlHelperExtensions.DictToQuerystring(new Dictionary<string, string> {
                {"pp", "1"},
                {"two", "two2"},
            });
            Assert.Equal("pp=1&two=two2", s);
        }

        [Fact]
        public void DictToQuerystring_null_value() {
            var s = UrlHelperExtensions.DictToQuerystring(new Dictionary<string, string> {
                {"pp", null},
            });
            Assert.Equal("pp=", s);
        }

    }
}