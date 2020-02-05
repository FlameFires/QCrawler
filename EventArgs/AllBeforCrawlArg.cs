using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler.EventArgs
{
    public class AllBeforCrawlArg
    {
        public IList<Request> requests;

        public AllBeforCrawlArg()
        {
        }

        public AllBeforCrawlArg(IList<Request> requests)
        {
            this.requests = requests;
        }
    }
}
