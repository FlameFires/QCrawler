using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using QCrawler.EventArgs;

namespace QCrawler.TaskModule
{
    internal sealed class CrawlerCoreModule
    {
        #region Members
        public string ID { get; private set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 请求
        /// </summary>
        IList<Request> requests;
        /// <summary>
        /// 管线
        /// </summary>
        IList<IPipeline> pipelines;
        /// <summary>
        /// 字典上下文
        /// </summary>
        IDictionary<Request, Response> dictContext = new Dictionary<Request, Response>();
        /// <summary>
        /// 任务列表
        /// </summary>
        IList<Task> RequestList
        {
            get => taskFactory.GetAllTasks();
        }
        /// <summary>
        /// 任务工厂
        /// </summary>
        TaskFactory taskFactory;
        /// <summary>
        /// 管线处理线程工厂
        /// </summary>
        TaskFactory pipelineDealFac;
        readonly ManualResetEvent manualReset = new ManualResetEvent(false);
        /// <summary>
        /// 爬取者
        /// </summary>
        Crawler crawler;
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
        // 外部调用
        public void Run(Crawler crawler, IList<Request> requests, IList<IPipeline> pipelines)
        {
            this.crawler = crawler;
            this.requests = requests;
            this.pipelines = pipelines;
            taskFactory = new TaskFactory(crawler.AllowMaxThread);
            pipelineDealFac = new TaskFactory(requests.Count);
            Go();
        }
        // 开始
        private async void Go()
        {
            try
            {
                AllBeforCrawlEventHandler?.Invoke(crawler, new AllBeforCrawlArg(requests));
                foreach (var item in requests)
                {
                    Request req = item;
                    if (req.EnableTaskPlan) // 启用调度
                    {
                        await EnableTaskModule.AddTask(req, BingRequest);
                    }
                    else
                        taskFactory.Run<Request>(InvokeReq, req);
                }

                taskFactory.WaitAll();
                pipelineDealFac.WaitAll();
                AllAfterCrawlEventHandler?.Invoke(crawler, new AllAfterCrawlArg(dictContext));
                // 开始下个任务
                NextCrawler();
            }
            catch (Exception ex)
            {
                OverEventHandler?.Invoke(crawler, new OverExceptionArg(ex)); // 异常事件
            }
        }
        // 开始下个任务
        private void NextCrawler()
        {
            var c = crawler.NextCrawler;
            if (c != null)
            {
                c.Crawl();
                crawler.NextCrawler = null;
            }
        }
        //调用请求
        private void InvokeReq(Request req)
        {
            try
            {
                int forNum = req.CycleNum; // 循环数
                bool forCycle = req.IsCycle; // 无限循环，知道cancel
                if (forCycle)
                {
                    do
                    {
                        BingRequest(req, a =>
                        {
                            forCycle = a.IsCycle;
                        }); // 从里传值到外面
                    } while (forCycle);
                    return; // 防止循环数存在，无限循环也存在
                }
                if (forNum > 0)
                {
                    for (int i = 0; i < forNum; i++)
                    {
                        BingRequest(req);
                    }
                };
            }
            catch (Exception ex)
            {
                OverEventHandler?.Invoke(crawler, new OverExceptionArg(ex)); // 异常事件
            }
        }
        // 绑定请求
        void BingRequest(Request req, Action<Request> action = null)
        {
            try
            {

                BeforCrawlEventHandler?.Invoke(crawler, new BeforCrawlArg(req)); // 开始请求事件

                Stopwatch sw = new Stopwatch(); // 计时器
                sw.Start();

                Response response = new Response(crawler); // 组装响应
                action?.Invoke(req);
                mHttpHelper mHelper = new mHttpHelper(req.Item);
                mHelper.GetHttpResult(); // 响应
                if (mHelper.httpResult == null) // 超时或其他异常
                {
                    var exception = new TimeoutException("mHelper");
                    response.Item.RequestException = exception;
                    OverEventHandler?.Invoke(crawler, new OverExceptionArg(exception)); // 异常事件
                }
                else if (mHelper.httpResult.RequestException == null) // 请求正常
                {
                }
                else // 其他异常情况
                {
                    OverEventHandler?.Invoke(crawler, new OverExceptionArg(mHelper.httpResult.RequestException)); // 异常事件
                }

                response.Item = mHelper.httpResult;

                sw.Stop();
                //swlist.Add(sw);
                response.RequestNum++;
                response.ConsumeTime = sw.ElapsedMilliseconds; // 请求消耗时长
                int threadId = Thread.CurrentThread.ManagedThreadId;
                sw.Reset();
                pipelineDealFac.Run<Request, Response>(PipelineRun, req, response);// 管线处理

                OneAfterCrawlEventHandler?.Invoke(crawler, new OneAfterCrawlArg(req, response));

                Thread.Sleep(req.RequsetDelay); // 延迟请求
            }
            catch (Exception ex)
            {
                OverEventHandler?.Invoke(crawler, new OverExceptionArg(ex)); // 异常事件
            }
        }
        // 管线运行
        private void PipelineRun(Request request, Response response)
        {
            try
            {
                int count = pipelines.Count;
                if (count > 0)
                {
                    var tempList = new IPipeline[count];
                    Array.Copy(pipelines.ToArray(), tempList, count);
                    foreach (var item in pipelines)
                    {
                        item.Process(request, response);
                    }
                }
            }
            catch (Exception ex)
            {
                OverEventHandler?.Invoke(crawler, new OverExceptionArg(ex)); // 异常事件
            }

        }
        #endregion

    }
}
