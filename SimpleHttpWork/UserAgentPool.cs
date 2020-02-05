using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler.SimpleHttpWork
{
    /// <summary>
    /// UserAgent 池
    /// </summary>
    public static class UserAgentPool
    {
        static UserAgentPool() => uaInit();
        private static void uaInit()
        {
            Add("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36");
        }

        public static List<string> UA_List { get; private set; } = new List<string>();

        private static Random _rd { get; set; } = new Random();

        public static void Add(string ua)
        {
            UA_List.Add(ua);
        }
        public static void Remove(string ua)
        {
            UA_List.Remove(ua);
        }
        /// <summary>
        /// 获取多个
        /// </summary>
        /// <param name="count"></param>
        /// <param name="IsRemoveRepeat"></param>
        /// <returns></returns>
        public static IEnumerable<string> Get(int count = 1, bool IsRemoveRepeat = true)
        {
            if (count < 1 || count > UA_List.Count) throw new ArgumentOutOfRangeException("count");

            if (IsRemoveRepeat)
            {
                return RemoveRepeatGet(count);
            }
            else
            {
                List<string> list = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    int index = _rd.Next(0, UA_List.Count);
                    list.Add(UA_List[index]);
                }
                return list;
            }
        }
        /// <summary>
        /// 获取一个ua
        /// </summary>
        /// <returns></returns>
        public static string GetOne()
        {
            if (UA_List.Count < 1) throw new ArgumentOutOfRangeException("UA_List");
            int index = _rd.Next(0, UA_List.Count);
            return UA_List[index];
        }

        /// <summary>
        /// 去重获取
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private static IEnumerable<string> RemoveRepeatGet(int count)
        {
            List<string> uaList = UA_List;
            List<string> values = new List<string>();
            for (int i = 0; i < count; i++)
            {
                int index = _rd.Next(0, uaList.Count);
                string selectItem = uaList[index];
                values.Add(selectItem);
                uaList.Remove(selectItem);
            }
            return uaList;
        }
    }
}
