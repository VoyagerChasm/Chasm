using Chasm.Proxys.Modules.Scraper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Chasm.Proxys.Test.Modules.Scrapers
{
    public class WebScraperTest
    {

        [Theory]
        [InlineData(1, null)]
        [InlineData(1, "")]
        [InlineData(1, " ")]
        public void TestConstructorThrowsArgumentException(uint timeout, string regex)
        {
            Assert.Throws<ArgumentException>(() => new WebScraper(timeout, regex));
        }

        [Theory]
        [InlineData(0)]
        public void TestConstructorThrowsArgumentOutOfRangeException(uint timeout)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WebScraper(timeout));
        }

        [Theory]
        [InlineData(null)]
        public void TestStartScrapeThrowsArgumentNullException(ICollection<string> source)
        {
            WebScraper webScraper = new WebScraper();
            Assert.Throws<ArgumentNullException>(() => webScraper.StartScrape(source));
        }

        [Fact]
        public void TestStartScrapeThrowsArgumentOutOfRangeException()
        {
            WebScraper webScraper = new WebScraper();
            Assert.Throws<ArgumentOutOfRangeException>(() => webScraper.StartScrape(new List<string>()));
        }

        [Fact]
        public void TestStartScrape()
        {
            WebScraper webScraper = new WebScraper();
            webScraper.OnErrorScrapingEventHandler += WebScraper_OnErrorScrapingEventHandler; ;
            webScraper.OnSourceParsedEventHandler += WebScraper_OnSourceParsedEventHandler; ;
            webScraper.OnStartScrapingEventHandler += WebScraper_OnStartScrapingEventHandler; ;
            webScraper.OnStopScrapingEventHandler += WebScraper_OnStopScrapingEventHandler; ;

            List<string> source = new List<string>() { @"http://www.proxyserverlist24.top/feeds/posts/default", @"http://sslproxies24.blogspot.in/feeds/posts/default2" };

            HashSet<string> proxy = webScraper.StartScrape(source);
            Assert.NotEmpty(proxy);

            List<string> source2 = new List<string>() { @"http://www.proxyserverlist24.top/feeds/posts/default", @"http://sslproxies24.blogspot.in/feeds/posts/default2" };
            Task task = new Task(() => webScraper.StartScrape(source2));
            task.Start();
            webScraper.StopScrape();

            HashSet<string> proxy2 = null;
            Task task2 = new Task(() => proxy2 = webScraper.StartScrape(source2));
            task2.Start();
            Thread.Sleep(2000);
            webScraper.StopScrape();

            if (!(proxy2 is null))
                Assert.True(proxy2.Count >= 0);
        }

        [ExcludeFromCodeCoverage]
        private void WebScraper_OnStopScrapingEventHandler(object sender, EventArgs e)
        {
        }

        [ExcludeFromCodeCoverage]
        private void WebScraper_OnStartScrapingEventHandler(object sender, EventArgs e)
        {
        }

        [ExcludeFromCodeCoverage]
        private void WebScraper_OnSourceParsedEventHandler(ICollection<string> proxys, string source)
        {
        }

        [ExcludeFromCodeCoverage]
        private void WebScraper_OnErrorScrapingEventHandler(Exception exception)
        {
        }

    }
}
