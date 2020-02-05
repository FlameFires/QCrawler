using System;

namespace QCrawler.Sample
{
    public class DemoOne : BaseDemo
    {
        public void Run()
        {
            Console.WriteLine(typeof(DemoOne).Name);
            // 使用爬取框架，异常可通过异常事件来捕捉
            string url = "https://www.baidu.com/";
            Crawler c = new Crawler(url);
            // 此爬取没有展示，下面开始开始配置
            Console.WriteLine("未设置管线");
            c.Crawl();
            Console.WriteLine("第一次请求结束\r");

            // 第二次请求添加管线处理程序
            c.AddPipeline(new DemoOneDeal());
            c.Crawl();
            Console.WriteLine("第二次请求结束\r");
            Console.WriteLine();
        }

        public class DemoOneDeal : IPipeline
        {
            public void Process(Request request, Response response)
            {
                var info = string.Format("请求ID: {0}\r\n链接：{1}\r\n耗时：{2}\r\n请求内容：{3}", request.Id, request.Item.URL, response.ConsumeTime, response.Item.StrResult.Substring(0,10));
                Console.WriteLine(info);
            }
        }
    }

    
}
