# QCrawler
一个简单方便，熟悉灵活，的爬虫框架

## 欢迎来到 FlameFires 的GitHub

下面是个人开发的一个爬虫框架，代码观赏性可能有点差，使用起来类似NCrawler(主要的借鉴，尊重开发者)	<br/>
NCrawler的链接（赠人玫瑰，手有余香）<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;官网链接: <https://archive.codeplex.com/?p=ncrawler><br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;GitHub: <https://github.com/esbencarlsen/NCrawler>

***

<!-- 内嵌数学公式$\sum_{i=1}^{10}f(i)\,\,\text{thanks}$
$$
\sum_{i=1}^{10}f(i)\,\,\text{thanks}
$$ -->

示例在 Sample 文件夹里，如下在Main方法使用可直接查看相关示例

>DemoFactory.Run(Demo.All); # 选择要看的示例枚举

框架的简单使用
>string url = "https://www.baidu.com/";<br/>
>Crawler c = new Crawler(url);<br/>
>c.Crawl();

>public class DealPipeline : IPipeline <br/>
{ <br/>
&emsp;public void Process(Request request, Response response) <br/>
&emsp;{ <br/>
&emsp;// ...    相关处理 <br/>
&emsp;} <br/>
>} 

看了上面的示例，发现和NCrawler简直一摸一样，一开始我挺喜欢NCrawler的，但是有些想要的功能不是很完美。
下面介绍此框架的一些功能：
<br>
1.引入定时调度框架，可通过设置Cron表达式来调度执行请求
 [推荐一个很好用的Cron表达式生成网站](http://cron.qqe2.com/ "表达式生成网站") &emsp;<http://cron.qqe2.com/>
```
    Request req = new Request()
        {
            CronExpress = "0 * * * * ? *", // 每分钟执行一次
            CycleNum = 2,
            Url = url
        };
    Crawler c = new Crawler(req); // 记得加上处理类，否则没啥反应
    c.Crawl();
```

2.可设置多个url请求
<br/>

```
    string[] urls = new string[] { "https://www.baidu.com/", "https://www.bilibili.com/", "https://blog.csdn.net/api/ArticleHighWords/list" };
    Crawler c = new Crawler(urls, new PipelineDeal())
    {
        // 在使用 AllowMaxThread 时要关闭 OnMaxThreadOfRquestNum
        OnMaxThreadOfRquestNum = false, //  开启最大线程数根据请求数获取,默认开启；true:开启
        AllowMaxThread = 2 // 允许的最大线程数
    };
```

3. 灵活的配置下个请求
```
    public class DealPipeline : IPipeline
        {
            public void Process(Request request, Response response)
            {
                // 最好检查一下,若 response.Item 为null 就是出异常，设置好异常处理事件OverEventHandler
                if(response.Item == null) return;

                // 注意事项!!!
                // 注意事项!!!
                // 注意事项!!! 重要的事情说三遍
                // 请勿使用当前管线处理程序，避免循环操作出现异常

                // 设置下个请求,会在当前所有链接获取完后开始下列新爬取
                Crawler c = response.crawler;
                c.SetNextCrawl(new Crawler("https://www.zhihu.com/", new NextDealPipeline()));

                // 在每个请求实例下设置下一个爬取，需要手动Crawl()
                request.NextCrawler = new Crawler("https://blog.csdn.net/api/ArticleHighWords/list", new RequestNextPipeline());
                request.NextCrawler.CrawlAsync();

                // 或者通过一些参数过滤
                if(request.Url == "https://www.bilibili.com/")
                {
                    request.NextCrawler = new Crawler("http://cron.qqe2.com/", new RequestNextPipeline());
                    request.NextCrawler.Crawl();
                }
                // 或者通过自己设置Filter属性来过滤配置想要的请求
                if (request.Filter.ToString() == "baidu")
                    {
                        var c = request.NextCrawler = new Crawler("https://www.bilibili.com/",new BaiduPipeline());
                        c.Crawl();
                    }

                
            }
        }
```
4. 内置多个静态帮助方法，省去常用的方法去封装
```
        string url = "https://www.baidu.com/";
        string html = SimpleHttp.HttpGet(url);
        // 或者使用静态帮助类
        string xpath_express = "";
        HtmlNodeCollection result = html.XPath(xpath_express);
        // ...操作

        //多个表达式
        string[] expressList = new string[] { "",""};
        IList<HtmlNodeCollection> results = html.XPath(expressList);
        // 使用自带静态方法遍历
        results.Foreach( t=> {
            // ... 操作
        });
```
<br/>


<br/>

需要引用的第三方框架 <br/>
&emsp;Quartz.Net
<br/>
&emsp;Newtonsoft.Json
<br/>
&emsp;HtmlAgilityPack
<br/>
需要引用的程序集
>using System.Drawing; <br/>
>using System.Web;
<br/>


个人引言：
<br/>
&emsp;&emsp;希望各位大佬能给出有力的批评和给力的建议

---
个人邮箱：<fire2019@qq.com> <br/>
QQ: < 941049777 >
