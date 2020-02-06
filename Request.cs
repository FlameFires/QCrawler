using System;
using System.Net.Http;

namespace QCrawler
{
    /// <summary>
    /// 请求配置
    /// </summary>
    public class Request
    {
        #region 构造函数
        public Request() { CycleNum = 1; }
        public Request(string url) : this()
        {
            Url = url;
        }
        public Request(mHttpItem item) : this()
        {
            Item = item;
        }
        public Request(string url, HttpMethod method) : this()
        {
            this.Url = url;
            this.Method = method;
        }
        #endregion

        #region Members
        public string Id { get; private set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 请求头参数
        /// </summary>
        public mHttpItem Item { get; set; }
        /// <summary>
        /// 过滤信息
        /// </summary>
        public object Filter { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get { return string.IsNullOrEmpty(url) ? Item?.URL : url; } set { url = value; if (Item == null) { Item = new mHttpItem() { URL = value }; } else Item.URL = url; } }
        private string url;
        /// <summary>
        /// 请求方法
        /// </summary>
        public HttpMethod Method { get { return method == null ? Item.MethodItem : method; } set { method = value; if (method == null) { Item = new mHttpItem() { MethodItem = value }; } else Item.MethodItem = method; } }
        private HttpMethod method;
        /// <summary>
        /// 循环次数，若 IsCycle 为 True,则此项无效,最小1，最大1024
        /// </summary>
        public int CycleNum { get => cycleNum; set { if (value < 1) cycleNum = 1; else if (value > 1024) cycleNum = 1024; else cycleNum = value; } }
        private int cycleNum;
        /// <summary>
        /// 是否取消请求
        /// </summary>
        public Boolean IsCancel { get; set; }
        /// <summary>
        /// 请求延时，默认1秒
        /// </summary>
        public int RequsetDelay { get; set; } = 1000;
        /// <summary>
        /// 是否无限循环请求
        /// </summary>
        public Boolean IsCycle { get; set; } = false;
        /// <summary>
        /// 是否启用任务计划
        /// </summary>
        public Boolean EnableTaskPlan { get { return string.IsNullOrEmpty(CronExpress) ? false : true; } }
        /// <summary>
        /// Cron表达式
        /// </summary>
        public string CronExpress { get; set; }
        /// <summary>
        /// 下个爬取者
        /// </summary>
        public Crawler NextCrawler { get; set; }
        #endregion
    }

    /// <summary>
    /// 响应配置
    /// </summary>
    public class Response
    {
        public Response()
        {
        }

        public Response(Crawler crawler)
        {
            this.crawler = crawler;
        }

        /// <summary>
        /// 响应
        /// </summary>
        public mHttpResult Item { get; set; }
        /// <summary>
        /// 请求耗费时间
        /// </summary>
        public long ConsumeTime { get; set; }
        /// <summary>
        /// 请求次数
        /// </summary>
        public int RequestNum { get; set; } = 0;
        /// <summary>
        /// 出现的异常
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// 当前的Crawler
        /// </summary>
        public Crawler crawler { get; set; }
    }
}
