using System;

namespace QCrawler.Sample
{
    /// <summary>
    /// 多个请求和设置下个请求和请求配置
    /// </summary>
    public class DemoFour : BaseDemo
    {
        public void Run()
        {
            Console.WriteLine(typeof(DemoFour).Name);

            string[] urls = new string[] { "https://www.baidu.com/", "https://www.bilibili.com/", "https://blog.csdn.net/api/ArticleHighWords/list" };
            Crawler c = new Crawler(urls, new PipelineDeal())
            {
                // 现在配置功能还少,在使用 AllowMaxThread 时要关闭 OnMaxThreadOfRquestNum
                OnMaxThreadOfRquestNum = false,
                AllowMaxThread = 2
            };
            c.OverEventHandler += C_OverEventHandler;
            c.Crawl();

            Console.WriteLine();
        }

        private void C_OverEventHandler(object sender, EventArgs.OverExceptionArg e)
        {
            Console.WriteLine(e.exception.Message);
        }

        public class PipelineDeal : IPipeline
        {
            public void Process(Request request, Response response)
            {
                string url = request.Item.URL;
                long time = response.ConsumeTime;
                Console.WriteLine(string.Format("链接：{0}\r\n耗时：{1}\r\n内容:{2}", url, time, response.Item.StrResult.Substring(0, 10)));

                // 注意事项!!!
                // 注意事项!!!
                // 注意事项!!! 重要的事情说三遍
                // 请勿使用当前管线处理程序，避免循环操作出现异常

                // 设置下个请求,会在当前所有链接获取完后开始下列新爬取
                Crawler c = response.crawler;
                c.SetNextCrawl(new Crawler("https://www.zhihu.com/", new NextDealPipeline()));

                // 在每个请求实例下设置下一个爬取，需要手动Crawl()
                request.NextCrawler = new Crawler("https://blog.csdn.net/api/ArticleHighWords/list", new RequestNextPipeline())
                {
                    
                };
                request.NextCrawler.CrawlAsync();

                // 或者自己过滤
                if(request.Url == "https://www.bilibili.com/")
                {
                    request.NextCrawler = new Crawler("http://cron.qqe2.com/", new RequestNextPipeline());
                    request.NextCrawler.Crawl();
                }

                
            }
        }

        public class RequestNextPipeline : IPipeline
        {
            public void Process(Request request, Response response)
            {
                if (response.Item != null)
                {
                    Console.WriteLine("\r\n我是请求头设置的管线处理程序");
                    string url = request.Item.URL;
                    long time = response.ConsumeTime;
                    Console.WriteLine(string.Format("链接：{0}\r\n耗时：{1}\r\n内容:{2}", url, time, response.Item.StrResult.Substring(0, 10)));
                }
            }
        }

        public class NextDealPipeline : IPipeline
        {
            public void Process(Request request, Response response)
            {
                if (response.Item != null)
                {
                    Console.WriteLine("\r\n我是响应头设置的管线处理程序");
                    string url = request.Item.URL;
                    long time = response.ConsumeTime;
                    Console.WriteLine(string.Format("链接：{0}\r\n耗时：{1}\r\n内容:{2}", url, time, response.Item.StrResult.Substring(0, 10)));
                }
            }
        }
    }
}
