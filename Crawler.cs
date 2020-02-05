using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QCrawler.EventArgs;
using QCrawler.TaskModule;

namespace QCrawler
{
    /// <summary>
    /// 任务调度爬虫框架
    /// </summary>
    public class Crawler : IQCrawler
    {
        #region 构造函数

        public Crawler()
        {
            CurrentCrawler = this;
        }
        private Crawler(params IPipeline[] pipelines) : this()
        {
            this.pipelines = pipelines;
        }
        public Crawler(string url, params IPipeline[] pipelines) : this(pipelines)
        {
            var reqs = Crawler.SimpleExport(url);
            requests = reqs;
        }
        public Crawler(string[] urls, params IPipeline[] pipelines) : this(pipelines)
        {
            var reqs = Crawler.SimpleExport(urls);
            requests = reqs;
        }
        public Crawler(Request request, params IPipeline[] pipelines) : this(pipelines)
        {
            this.requests = new List<Request>();
            requests.Add(request);
        }
        public Crawler(IList<Request> request, params IPipeline[] pipelines) : this(pipelines)
        {
            this.requests = request;
        }
        #endregion

        #region Members
        /// <summary>
        /// 请求头
        /// </summary>
        public IList<Request> requests { get; private set; }
        /// <summary>
        /// 是否多个请求
        /// </summary>
        public Boolean IsMulRequest { get { return requests.Count > 1 ? true : false; } }
        /// <summary>
        /// 所有管线
        /// </summary>
        public IList<IPipeline> pipelines { get; set; }
        /// <summary>
        /// 下个爬取者
        /// </summary>
        public Crawler NextCrawler
        {
            get { return nextCrawler; }
            set { nextCrawler = value; }
        }
        private Crawler nextCrawler;
        /// <summary>
        /// 当前爬取者
        /// </summary>
        public Crawler CurrentCrawler { get; set; }
        /// <summary>
        /// 爬取队列
        /// </summary>
        private Queue<Crawler> waitCrawlers = new Queue<Crawler>();

        /// <summary>
        /// 核心执行者模块
        /// </summary>
        private CrawlerCoreModule core = new CrawlerCoreModule();
        private static readonly object o = new object();
        #endregion

        #region 配置

        /// <summary>
        /// 允许的最大线程数，最多1024个，最少1个
        /// </summary>
        public int AllowMaxThread { get { return OnMaxThreadOfRquestNum ? requests.Count : allowMaxThread; } set { allowMaxThread = value; } }
        private int allowMaxThread;

        /// <summary>
        /// 开启最大线程数根据请求数获取,默认开启；true:开启
        /// </summary>
        public Boolean OnMaxThreadOfRquestNum { get; set; } = true;

        /// <summary>
        /// 管线执行是否同步,默认同步false,解释- true：同步；影响：不同步，可能会造成下个QCrawler未null
        /// </summary>
        [Obsolete("管线异步执行会造成很多问题，所以取消了此功能", true)]
        public Boolean IsPipelineAsync { get; set; } = false;

        #endregion

        #region 事件
        /// <summary>
        /// 出现异常事件
        /// </summary>
        public event EventHandler<OverExceptionArg> OverEventHandler;
        /// <summary>
        /// 全部请求开始之前
        /// </summary>
        public event EventHandler<AllBeforCrawlArg> AllBeforCrawlEventHandler;
        /// <summary>
        /// 个请求开始之前
        /// </summary>
        public event EventHandler<BeforCrawlArg> BeforCrawlEventHandler;
        /// <summary>
        /// 个请求完成事件
        /// </summary>
        public event EventHandler<OneAfterCrawlArg> OneAfterCrawlEventHandler;
        /// <summary>
        /// 全部请求完成之后
        /// </summary>
        public event EventHandler<AllAfterCrawlArg> AllAfterCrawlEventHandler;
        #endregion

        #region 方法
        /// <summary>
        /// 开始执行
        /// </summary>
        public void Crawl()
        {
            InitEvents();
            //Console.WriteLine("core: " + core.ID);
            core.Run(CurrentCrawler, requests, pipelines);
        }
        /// <summary>
        /// 异步执行
        /// </summary>
        /// <returns></returns>
        public Task<bool> CrawlAsync()
        {
            return Task.Run<bool>(()=> {
                try
                {
                    Crawl();
                    return true;
                }catch(Exception ex)
                {
                    OverEventHandler?.Invoke(this, new OverExceptionArg(ex));
                    return false;
                }
            });
        }
        /// <summary>
        /// 设置下个请求
        /// </summary>
        /// <param name="crawler"></param>
        public void SetNextCrawl(Crawler crawler)
        {
            this.NextCrawler = crawler;
        }
        /// <summary>
        /// 初始化事件
        /// </summary>
        private void InitEvents()
        {
            if (OverEventHandler != null)
                core.OverEventHandler += OverEventHandler;
            if (AllAfterCrawlEventHandler != null)
                core.AllAfterCrawlEventHandler += AllAfterCrawlEventHandler;
            if (AllBeforCrawlEventHandler != null)
                core.AllBeforCrawlEventHandler += AllBeforCrawlEventHandler;
            if (OneAfterCrawlEventHandler != null)
                core.OneAfterCrawlEventHandler += OneAfterCrawlEventHandler;
            if (BeforCrawlEventHandler != null)
                core.BeforCrawlEventHandler += BeforCrawlEventHandler;
        }
        public void AddPipeline(IPipeline pipeline)
        {
            pipelines = new List<IPipeline>(); pipelines.Add(pipeline);
        }

        /// <summary>
        /// 简单到处请求头，简化操作
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public static List<Request> SimpleExport(params string[] urls)
        {
            var list = new List<Request>();
            foreach (var url in urls)
            {
                Request request = new Request()
                {
                    Item = new mHttpItem
                    {
                        URL = url
                    }
                };
                list.Add(request);
            }
            return list;

        }
        #endregion

    }
}
