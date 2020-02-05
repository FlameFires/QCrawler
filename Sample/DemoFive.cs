using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler.Sample
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class DemoFive : BaseDemo
    {
        public void Run()
        {
            Console.WriteLine(typeof(DemoFive).Name);

            string url = "https://www.baidu.com/";
            Request req = new Request()
            {
                CronExpress = "0 * * * * ? *", // 每分钟执行一次
                CycleNum = 2,
                Url = url
            };
            Crawler c = new Crawler(req, new DealPipeline());
            c.OverEventHandler += C_OverEventHandler;
            c.Crawl();
            Console.WriteLine("到打指定时间会执行 - " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            Console.WriteLine();
        }

        private void C_OverEventHandler(object sender, EventArgs.OverExceptionArg e)
        {
            Console.WriteLine(e.exception.Message);
        }

        public class DealPipeline : IPipeline
        {
            public void Process(Request request, Response response)
            {
                string html = response.Item.StrResult;
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            }
        }
    }
}
