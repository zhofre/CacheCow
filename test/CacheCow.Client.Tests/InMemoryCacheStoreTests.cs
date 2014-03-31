using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using CacheCow.Common;
using NUnit.Framework;

namespace CacheCow.Client.Tests
{
    public class InMemoryCacheStoreTests
    {

        [Test]
        public void Stores()
        {
            var cacheStore = new InMemoryCacheStore();
            var response = new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent("hello")
            };
            var key = new CacheKey("/api/v1/hey", new []{"hello","world"});
            cacheStore.AddOrUpdate(key, response);

        }

        [Test]
        public void StoresAndRetrieves()
        {
            var cacheStore = new InMemoryCacheStore();
            var response = new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent("hello")
            };
            var key = new CacheKey("/api/v1/hey", new[] { "hello", "world" });
            cacheStore.AddOrUpdate(key, response);
            
            var keyWithSameValues = new CacheKey("/api/v1/hey", new[] { "hello", "world" });
            HttpResponseMessage cachedResponse = null;
            Assert.IsTrue(cacheStore.TryGetValue(keyWithSameValues, out cachedResponse));

            Assert.AreEqual("hello", cachedResponse.Content.ReadAsStringAsync().Result);
        }

      
    }
}
