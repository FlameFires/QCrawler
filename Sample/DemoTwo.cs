using QCrawler.SimpleHttpWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler.Sample
{
    public class DemoTwo : BaseDemo
    {
        public void Run()
        {
            Console.WriteLine(typeof(DemoTwo).Name);
            // 简单获取,异常需要自己捕捉
            string url = "https://www.baidu.com/";
            string html = SimpleHttp.HttpGet(url);
            Console.WriteLine("简单请求："+html.Substring(0,10));
            Console.WriteLine();
        }
    }

    
}
