using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler.TaskModule
{
    internal sealed class EnableTaskModule
    {
        #region 构造函数
        static EnableTaskModule()
        {
            schedFact = new StdSchedulerFactory();
            On();
        }
        #endregion

        #region Members
        private const string constJob = "_job";
        private const string constTrigger = "_trigger";

        public static ISchedulerFactory schedFact;
        public static IScheduler sched;
        /// <summary>
        /// 调度任务列表
        /// </summary>
        private static IList<SchedTask> schedTasks = new List<SchedTask>();
        public static string ID { get; private set; } = Guid.NewGuid().ToString("N");
        #endregion

        #region Methods
        /// <summary>
        /// 启动
        /// </summary>
        public static void On()
        {
            if (sched == null)
            {
                sched = schedFact.GetScheduler().Result;
            }
            if (!sched.IsStarted)
            {
                sched.Start();
            }
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="request"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task AddTask(Request request, Action<Request, Action<Request>> action)
        {
            On();
            IJobDetail job = JobBuilder.Create<RunJob>()
                .WithIdentity(request.Id + constJob, ID)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(request.Id + constTrigger, ID)
                .StartNow()
                .WithCronSchedule(request.CronExpress)
                .Build();

            SchedTask schedTask = new SchedTask(ID, job, trigger, request, sched,action);
            // 传值过去
            job.JobDataMap["schedTask"] = schedTask;

            schedTasks.Add(schedTask);
            await sched?.ScheduleJob(job, trigger);
        }
        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="request"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task ModifyTask(Request request, Action<Request, Action<Request>> action)
        {
            await DeleteSchedTask(request.Id);
            await AddTask(request, action);
        }
        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns></returns>
        public static IList<SchedTask> GetAllTask()
        {
            var lists = schedTasks;
            return lists;
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public static Task<bool> DeleteSchedTask(string requestId)
        {
            SchedTask schedTask = schedTasks.Where(t => t.JobDetail.Key.Name == requestId + constJob).FirstOrDefault();
            return DeleteSchedTask(schedTask);
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="schedTask"></param>
        /// <returns></returns>
        public static Task<bool> DeleteSchedTask(SchedTask schedTask)
        {
            return sched.DeleteJob(schedTask.Jobkey);
        }
        /// <summary>
        /// 关闭任务
        /// </summary>
        public static void Shutdown()
        {
            if (!sched.IsShutdown)
            {
                sched.Shutdown();
            }
        }
        #endregion
    }

    /// <summary>
    /// 调度任务模型
    /// </summary>
    public class SchedTask
    {
        public SchedTask(string groupId, IJobDetail jobDetail, ITrigger trigger, Request request, IScheduler scheduler, Action<Request, Action<Request>> action)
        {
            GroupId = groupId;
            Jobkey = jobDetail.Key;
            TriggerKey = trigger.Key;
            JobDetail = jobDetail;
            Trigger = trigger;
            Request = request;
            Scheduler = scheduler;
            Action = action;
        }

        public string GroupId { get; private set; }
        /// <summary>
        /// 根据jobkey 删除指定任务
        /// </summary>
        public JobKey Jobkey { get; private set; }
        public TriggerKey TriggerKey { get; private set; }
        public IScheduler Scheduler { get; private set; }
        public IJobDetail JobDetail { get; set; }
        public ITrigger Trigger { get; set; }
        /// <summary>
        /// 任务调用次数
        /// </summary>
        public int InvokeNum { get; set; }
        public Request Request { get; set; }
        public Action<Request, Action<Request>> Action { get; set; }
        public TriggerState TriggerState { get { return Scheduler.GetTriggerState(TriggerKey).Result; } }
    }
}
