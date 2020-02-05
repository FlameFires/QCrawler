using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler
{
    public interface IPipeline
    {
        void Process(Request request, Response response);
    }
}
