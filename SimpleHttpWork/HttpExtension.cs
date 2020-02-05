using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QCrawler.SimpleHttpWork
{
    /// <summary>
    /// Http返回匹配 扩展类
    /// Depend  Msdn5HttpCode、HtmlAgilityPack、NewtonsoftJson、LitJSON
    /// </summary>
    public static class HttpExtension
    {
        #region XPath
        public static IDictionary<string, HtmlNodeCollection> XPath(this mHttpResult item, IEnumerable<KeyValuePair<string, string>> xpathExpress)
        {
            return XPath(item.StrResult, xpathExpress);
        }
        /// <summary>
        /// 插入多个表达式
        /// </summary>
        /// <param name="html"></param>
        /// <param name="xpathExpress"></param>
        /// <returns></returns>
        public static IDictionary<string, HtmlNodeCollection> XPath(this string html, IEnumerable<KeyValuePair<string, string>> xpathExpress)
        {
            if (CheckIsNull(html)) throw new ArgumentNullException("html");
            //if (CheckIsNull(xpathExpress.ToArray())) throw new ArgumentNullException("xpathExpress");
            foreach (var item in xpathExpress)
            {
                if (string.IsNullOrEmpty(item.Value))
                    throw new ArgumentNullException("xpathExpress item value have null of a or more ");
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            Dictionary<string, HtmlNodeCollection> values = new Dictionary<string, HtmlNodeCollection>();
            foreach (KeyValuePair<string, string> keyvalue in xpathExpress)
            {
                var key = keyvalue.Key;
                var value = doc.DocumentNode.SelectNodes(keyvalue.Value);
                values.Add(key, value);
            }
            return values;
        }
        public static IList<HtmlNodeCollection> XPath(this string html, IList<string> xpathExpress)
        {
            if (CheckIsNull(html)) throw new ArgumentNullException("html");
            if (CheckIsNull(xpathExpress.ToArray())) throw new ArgumentNullException("xpathExpress");

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var values = new List<HtmlNodeCollection>();
            foreach (var keyvalue in xpathExpress)
            {
                var value = doc.DocumentNode.SelectNodes(keyvalue);
                values.Add(value);
            }
            return values;
        }

        public static HtmlNodeCollection XPath(this mHttpResult item, string xpathExpress)
        {
            return item?.StrResult.XPath(xpathExpress);
        }

        public static HtmlNodeCollection XPath(this string html, string xpathExpress)
        {
            if (CheckIsNull(html)) throw new ArgumentNullException("html");
            if (CheckIsNull(xpathExpress)) throw new ArgumentNullException("xpathExpress");

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var collection = doc.DocumentNode.SelectNodes(xpathExpress);
            return collection;
        }

        public static HtmlNode Single_XPath(this mHttpResult item, string xpathExpress)
        {
            if(item==null || string.IsNullOrEmpty(item.StrResult))
                throw new ArgumentNullException("xpathExpress");

            return item?.StrResult.Single_XPath(xpathExpress);
        }
        public static HtmlNode Single_XPath(this string html, string xpathExpress)
        {
            if (CheckIsNull(html)) throw new ArgumentNullException("html");
            if (CheckIsNull(xpathExpress)) throw new ArgumentNullException("xpathExpress");

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.SelectSingleNode(xpathExpress);
            return node;
        }
        #endregion

        #region 正则匹配

        public static Dictionary<string, MatchCollection> Matches(this string html, IDictionary<string, string> regexExpress)
        {
            if (CheckIsNull(html)) throw new ArgumentNullException("html");
            foreach (var item in regexExpress)
            {
                if (string.IsNullOrEmpty(item.Value))
                    throw new ArgumentNullException("html");
            }

            Dictionary<string, MatchCollection> collection = new Dictionary<string, MatchCollection>();
            foreach (var item in regexExpress)
            {
                var key = item.Key;
                var value = Regex.Matches(html, item.Value);
                collection.Add(key, value);
            }
            return collection;
        }

        public static MatchCollection Matches(this string html, string regexExpress)
        {
            if (CheckIsNull(html)) throw new ArgumentNullException("html");
            if (CheckIsNull(regexExpress)) throw new ArgumentNullException("regexExpress");

            var collection = Regex.Matches(html, regexExpress);
            return collection;
        }

        public static Match Match(this string html, string regexExpress)
        {
            if (string.IsNullOrEmpty(html)) throw new ArgumentNullException("html");
            if (CheckIsNull(regexExpress)) throw new ArgumentNullException("regexExpress");

            var match = Regex.Match(html, regexExpress);
            return match;
        }

        #endregion

        #region ConvertToJson


        public static JToken ToJson(this string data)
        {
            JToken token = (JToken)JsonConvert.DeserializeObject(data);
            return token;
        }
        /// <summary>
        /// 字符串转化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T StrToObj<T>(this string data)
        {
            T model = JsonConvert.DeserializeObject<T>(data);
            return model;
        }
        /// <summary>
        /// 对象转化成json字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ObjToStr(this object data)
        {
            string json = JsonConvert.SerializeObject(data);
            return json;
        }
        #endregion

        #region 帮助方法
        /// <summary>
        /// 检查参数是否为空
        /// </summary>
        /// <param name="expressList"></param>
        /// <returns></returns>
        public static bool CheckIsNull(params string[] expressList)
        {
            foreach (var item in expressList)
            {
                if (string.IsNullOrEmpty(item)) return false;
            }
            return true;
        }
        /// <summary>
        /// 未实现
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        //private static bool CheckIsNull(params object[] list)
        //{
        //    foreach (var item in list)
        //    {
        //        Type t = list.GetType();
        //        if(t == typeof(string))
        //        {
        //            return CheckIsNull(list);
        //        }
        //    }
        //    return false;
        //}

        public static void Foreach(this IList<HtmlNodeCollection> values, Action<IList<HtmlNode>> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            List<int> nums = new List<int>();
            foreach (var item in values)
            {
                if (item != null)
                    nums.Add(item.Count);
            }
            int max = Max(nums.ToArray());
            for (int i = 0; i < max; i++)
            {
                var temp = new List<HtmlNode>();
                for (int j = 0; j < values.Count; j++)
                {
                    if (values[j] != null)
                        temp.Add(values[j][i]);
                    else
                        temp.Add(null);
                }
                action?.Invoke(temp);
            }
        }
        public static void Foreach(this IDictionary<string, HtmlNodeCollection> keyValues, Action<IDictionary<string, HtmlNode>> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            List<int> nums = new List<int>();
            foreach (var item in keyValues)
            {
                if (item.Value != null)
                    nums.Add(item.Value.Count);
            }

            int max = Max(nums.ToArray());
            for (int i = 0; i < max; i++)
            {
                Dictionary<string, HtmlNode> temp = new Dictionary<string, HtmlNode>();
                foreach (var item in keyValues)
                {
                    temp.Add(item.Key, item.Value[i]);
                }
                action?.Invoke(temp);
            }
        }
        /// <summary>
        /// 获取最大数
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        private static int Max(params int[] paras)
        {
            int[] temp = paras;
            int max = temp[0];
            for (int i = 0; i < temp.Length - 1; i++)
            {
                if (temp[i] < temp[i + 1])
                    max = temp[i + 1];
            }
            return max;
        }
        #endregion
    }
}
