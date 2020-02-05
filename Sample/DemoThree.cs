using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

using QCrawler.SimpleHttpWork;

namespace QCrawler.Sample
{
    /// <summary>
    /// 事件使用和静态帮助方法使用
    /// </summary>
    public class DemoThree : BaseDemo
    {
        public void Run()
        {
            Console.WriteLine(typeof(DemoThree).Name);

            string url = "https://www.baidu.com/";
            Crawler c = new Crawler(url,new DealPipeline());
            // 添加事件,异常处理事件,总共5个事件
            c.OverEventHandler += C_OverEventHandler;
            c.Crawl();

            Console.WriteLine();
        }

        private void C_OverEventHandler(object sender, EventArgs.OverExceptionArg e)
        {
            Console.WriteLine("异常事件 -> 错误信息："+e.exception.Message);
        }

        public class DealPipeline : IPipeline
        {
            public void Process(Request request, Response response)
            {
                // 等于null就是发生了异常
                if (response.Item != null)
                {
                    var info = string.Format("请求ID: {0}\r\n链接：{1}\r\n耗时：{2}\r\n请求内容：{3}", request.Id, request.Item.URL, response.ConsumeTime, response.Item.StrResult.Substring(0, 10));
                    Console.WriteLine(info);

                    string html = response.Item.StrResult;

                    // 引用了 HtmlAgilityPack
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(response.Item.StrResult);
                    // ...操作

                    // 或者使用静态帮助类
                    string xpath_express = "";
                    HtmlNodeCollection result = html.XPath(xpath_express);
                    // ...操作

                    //多个表达式
                    string[] expressList = new string[] { "", "" };
                    IList<HtmlNodeCollection> results = html.XPath(expressList);
                    // 使用自带静态方法遍历
                    results.Foreach(t =>
                    {
                        // ... 操作
                    });

                }
            }
        }
    }
}
