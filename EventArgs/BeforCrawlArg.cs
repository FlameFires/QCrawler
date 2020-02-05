using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler.EventArgs
{
    public class BeforCrawlArg
    {
        public Request request;

        public BeforCrawlArg()
        {
        }

        public BeforCrawlArg(Request request)
        {
            this.request = request;
        }
    }
}
