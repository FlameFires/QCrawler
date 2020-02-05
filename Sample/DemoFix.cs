using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler.Sample
{
    /// <summary>
    /// 筛选
    /// </summary>
    public class DemoFix : BaseDemo
    {
        public void Run()
        {
            Console.WriteLine(typeof(DemoFix).Name);

            Request[] req = new Request[] {
                new Request("https://www.baidu.com/"){ Filter = "baidu"},
                new Request("http://cron.qqe2.com/")
            };
            Crawler c = new Crawler(req, new DealPipeline());
            c.Crawl();

            Console.WriteLine();
        }

        public class DealPipeline : IPipeline
        {
            public void Process(Request request, Response response)
            {
                if (response != null)
                {
                    var info = string.Format("请求ID: {0}\r\n链接：{1}\r\n耗时：{2}\r\n请求内容：{3}", request.Id, request.Item.URL, response.ConsumeTime, response.Item.StrResult.Substring(0, 10));
                    Console.WriteLine(info);

                    if (request.Filter.ToString() == "baidu")
                    {
                        var c = request.NextCrawler = new Crawler("https://www.bilibili.com/",new BaiduPipeline());
                        c.Crawl();
                    }
                }
            }
        }

        public class BaiduPipeline : IPipeline
        {
            public void Process(Request request, Response response)
            {
                if (response != null)
                {
                    var info = string.Format("请求ID: {0}\r\n链接：{1}\r\n耗时：{2}\r\n请求内容：{3}", request.Id, request.Item.URL, response.ConsumeTime, response.Item.StrResult.Substring(0, 10));
                    Console.WriteLine(info);
                }
            }
        }
    }
}
