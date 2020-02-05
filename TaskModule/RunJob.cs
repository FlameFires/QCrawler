using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QCrawler.TaskModule;
using Quartz;

namespace QCrawler
{
    internal class RunJob : IJob
    {
        SchedTask _schedTask;
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                IJobDetail job = context.JobDetail;
                JobDataMap datas = job.JobDataMap;
                SchedTask schedTask = (SchedTask)datas["schedTask"];
                if (schedTask != null)
                {
                    var ssss = EnableTaskModule.GetAllTask();
                    ++schedTask.InvokeNum;
                    _schedTask = schedTask;
                    if (schedTask.Request.IsCancel) // 取消请求
                    {
                        Del(); return;
                    }

                    // 启动爬取
                    schedTask.Action?.Invoke(schedTask.Request, null);

                    if (!schedTask.Request.IsCycle && schedTask.InvokeNum >= schedTask.Request.CycleNum)// 是否循环结束
                    {
                        Del(); return;
                    }

                    if (CheckTask(schedTask)) return;
                }
            });
        }

        /// <summary>
        /// 检查状态，过滤无效爬取
        /// </summary>
        /// <param name="schedTask"></param>
        /// <returns></returns>
        public bool CheckTask(SchedTask schedTask)
        {
            var status = schedTask.TriggerState;
            if (status == TriggerState.Error)
            {
                return Del();
            }
            return false;
        }

        public bool Del()
        {
            if (_schedTask != null)
            {
                try
                {
                    EnableTaskModule.DeleteSchedTask(_schedTask);
                    return true;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    return false;
                }
            }
            return false;
        }
    }
}
