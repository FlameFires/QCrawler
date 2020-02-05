using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QCrawler.TaskModule
{
    public class TaskFactory
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="max"></param>
        public TaskFactory(int max) { MaxThread = max; semaphore = new Semaphore(MaxThread, MaxThread); }

        #region Members
        /// <summary>
        /// 工厂id，可以用来判断是否同一实例
        /// </summary>
        public string Id { get; private set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 任务列表
        /// </summary>
        IList<Task> tasks = new List<Task>();
        /// <summary>
        /// 缓存任务列表，记录上次的任务里列表
        /// </summary>
        public IList<Task> TempTask { get; private set; }
        /// <summary>
        /// 信号量
        /// </summary>
        Semaphore semaphore;
        /// <summary>
        /// 最大线程数,1024
        /// </summary>
        public int MaxThread { get { return maxThread; } private set { if (value < 1) maxThread = 1; else if (value > 1024) maxThread = 1024; else maxThread = value; } }
        private int maxThread;
        #endregion

        #region Method
        public IList<Task> GetAllTasks()
        {
            return tasks;
        }
        public bool DeleteTask(Task task)
        {
            try
            {
                var t = tasks.Where(ta => ta == task).FirstOrDefault();
                if (t != null)
                { tasks.Remove(task); return true; }
                else return false;
            }
            catch
            {
                return false;
            }
        }
        public void Clear()
        {
            if (tasks.Count > 0)
            {
                Task[] temp = new Task[tasks.Count];
                tasks.CopyTo(temp, 0);
                TempTask = new List<Task>(temp);
                tasks.Clear();
            }
        }
        public void Run(Action action)
        {
            semaphore.WaitOne();
            var t = Task.Run(() =>
            {
                action?.Invoke();
                semaphore.Release();
            });
            tasks.Add(t);
        }
        public void Run<T>(RunDelegate<T> action, T paras)
        {
            semaphore.WaitOne();
            var t = Task.Run(() =>
            {
                action?.Invoke(paras);
                semaphore.Release();
            });
            tasks.Add(t);
        }
        public void Run<T1, T2>(RunDelegate<T1, T2> runDelegate, T1 t1, T2 t2)
        {
            semaphore.WaitOne();
            var t = Task.Run(() =>
            {
                runDelegate?.Invoke(t1, t2);
                semaphore.Release();
            });
            tasks.Add(t);
        }
        public void Run<T1, T2, T3>(RunDelegate<T1, T2, T3> runDelegate, T1 t1, T2 t2, T3 t3)
        {
            semaphore.WaitOne();
            var t = Task.Run(() =>
            {
                runDelegate?.Invoke(t1, t2, t3);
                semaphore.Release();
            });
            tasks.Add(t);
        }

        public void WaitAll()
        {
            Task.WaitAll(tasks.ToArray());
            Clear();
        }
        #endregion

    }
    public delegate void RunDelegate<T>(T data);
    public delegate void RunDelegate<T1, T2>(T1 t1, T2 t2);
    public delegate void RunDelegate<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
}
