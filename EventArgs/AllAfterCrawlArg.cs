using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler.EventArgs
{
    public class AllAfterCrawlArg
    {
        IDictionary<Request, Response> context;

        public AllAfterCrawlArg(IDictionary<Request, Response> context)
        {
            this.context = context;
        }

        
    }
}
