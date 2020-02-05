namespace QCrawler.EventArgs
{
    public class StartCrawlArg
    {
        public Request request;

        public StartCrawlArg()
        {
        }

        public StartCrawlArg(Request request)
        {
            this.request = request;
        }
    }
}