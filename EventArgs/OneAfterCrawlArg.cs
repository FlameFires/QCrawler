using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler.EventArgs
{
    public class OneAfterCrawlArg
    {
        public Request request;
        public Response response;

        public OneAfterCrawlArg()
        {
        }

        public OneAfterCrawlArg(Request request, Response response)
        {
            this.request = request;
            this.response = response;
        }
    }
}
