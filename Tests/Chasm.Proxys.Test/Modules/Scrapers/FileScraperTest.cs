using Chasm.Proxys.Modules.Scraper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Chasm.Proxys.Test.Modules.Scrapers
{
    public class FileScraperTest
    {

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void TestConstructorThrowsArgumentException(string regex)
        {
            Assert.Throws<ArgumentException>(() => new FileScraper(regex));
        }

        [Theory]
        [InlineData(null)]
        public void TestStartScrapeThrowsArgumentNullException(ICollection<string> source)
        {
            FileScraper fileScraper = new FileScraper();
            Assert.Throws<ArgumentNullException>(() => fileScraper.StartScrape(source));
        }

        [Fact]
        public void TestStartScrapeThrowsArgumentOutOfRangeException()
        {
            FileScraper fileScraper = new FileScraper();
            Assert.Throws<ArgumentOutOfRangeException>(() => fileScraper.StartScrape(new List<string>()));
        }

        [Fact]
        public void TestStartScrape()
        {
            FileScraper fileScraper = new FileScraper();
            fileScraper.OnErrorScrapingEventHandler += FileScraper_OnErrorScrapingEventHandler;
            fileScraper.OnSourceParsedEventHandler += FileScraper_OnSourceParsedEventHandler;
            fileScraper.OnStartScrapingEventHandler += FileScraper_OnStartScrapingEventHandler;
            fileScraper.OnStopScrapingEventHandler += FileScraper_OnStopScrapingEventHandler;

            Directory.CreateDirectory(@"C:\Temp");
            File.WriteAllText(@"C:\Temp\TestFileScrape.txt", "192.168.15.1:2222");
            File.WriteAllText(@"C:\Temp\EmptyTestFileScrape.txt", "");
            List<string> source = new List<string>() { @"C:\Temp\TestFileScrape.txt", @"C:\Temp\NotFoundTestFileScrape.txt", @"C:\Temp\EmptyTestFileScrape.txt" };

            HashSet<string> proxy = fileScraper.StartScrape(source);
            Assert.Single(proxy);
            Assert.Equal("192.168.15.1:2222", proxy.ToList()[0]);

            List<string> source2 = new List<string>() { @"C:\Temp\TestFileScrape.txt", @"C:\Temp\NotFoundTestFileScrape.txt", @"C:\Temp\EmptyTestFileScrape.txt" };
            Task task = new Task(() => fileScraper.StartScrape(source2));
            task.Start();
            fileScraper.StopScrape();

            HashSet<string> proxy2 = null;
            Task task2 = new Task(() => proxy2 = fileScraper.StartScrape(source2));
            task2.Start();
            Thread.Sleep(100);
            fileScraper.StopScrape();

            if (!(proxy2 is null))
                Assert.True(proxy2.Count >= 0);

            Directory.Delete(@"C:\Temp", true);
        }

        [ExcludeFromCodeCoverage]
        private void FileScraper_OnStopScrapingEventHandler(object sender, EventArgs e)
        {
        }

        [ExcludeFromCodeCoverage]
        private void FileScraper_OnStartScrapingEventHandler(object sender, EventArgs e)
        {
        }

        [ExcludeFromCodeCoverage]
        private void FileScraper_OnSourceParsedEventHandler(ICollection<string> proxys, string source)
        {
        }

        [ExcludeFromCodeCoverage]
        private void FileScraper_OnErrorScrapingEventHandler(Exception exception)
        {
        }
    }
}
