using System.Collections.Generic;

namespace Chasm.Proxys.Modules.Scraper
{
    public interface IScraper<T>
    {

        public HashSet<string> StartScrape(T source);

        public void StopScrape();

    }
}
