using System;
using System.Collections.Generic;
using JSIStudios.SimpleRESTServices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JSIStudios.SimpleRestServices.Testing.UnitTests.Client
{
    [TestClass]
    public class UrlBuilderTests
    {
        [TestMethod]
        public void Should_Add_Query_String_Params_To_Existing_Url()
        {
            var paramList = new Dictionary<string, string> {{"key1", "value1"}, {"key2", "value2"}};
            var url = "http://www.mytestsite.com";
            var expectedUrl = "http://www.mytestsite.com?key1=value1&key2=value2";

            var urlBuilder = new UrlBuilder();

            var newUri = urlBuilder.Build(url, paramList);

            Assert.AreEqual(expectedUrl, newUri);
        }

        [TestMethod]
        public void Should_Add_Query_String_Params_To_Existing_Uri()
        {
            var paramList = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } };
            var uri = new Uri("http://www.mytestsite.com");
            var expectedUrl = uri.AbsoluteUri + "?key1=value1&key2=value2";

            var urlBuilder = new UrlBuilder();

            var newUri = urlBuilder.Build(uri, paramList);

            Assert.AreEqual(expectedUrl, newUri.AbsoluteUri);
        }

        [TestMethod]
        public void Should_Append_Query_String_Params_To_Existing_Url()
        {
            var paramList = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } };
            var url = "http://www.mytestsite.com?key0=value0";
            var expectedUrl = "http://www.mytestsite.com?key0=value0&key1=value1&key2=value2";

            var urlBuilder = new UrlBuilder();

            var newUri = urlBuilder.Build(url, paramList);

            Assert.AreEqual(expectedUrl, newUri);
        }

        [TestMethod]
        public void Should_Append_Query_String_Params_To_Existing_Uri()
        {
            var paramList = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } };
            var uri = new Uri("http://www.mytestsite.com?key0=value0");
            var expectedUrl = uri.AbsoluteUri + "&key1=value1&key2=value2";

            var urlBuilder = new UrlBuilder();

            var newUri = urlBuilder.Build(uri, paramList);

            Assert.AreEqual(expectedUrl, newUri.AbsoluteUri);
        }
    }
}
