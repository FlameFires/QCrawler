using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler
{
    internal interface IQCrawler
    {
        void Crawl();

        Task<bool> CrawlAsync();
    }
}
