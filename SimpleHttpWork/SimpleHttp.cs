using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler.SimpleHttpWork
{
    /// <summary>
    /// 封装的简单的 Http 请求
    /// 依赖 Msdn5HttpCode
    /// </summary>
    public static class SimpleHttp
    {
        #region Member
        public static string UserAgent { get; set; } = UserAgentPool.GetOne();
        public static int TimeOut { get; set; } = 10 * 1000;
        public static bool AllowRedirect { get; set; } = false;
        private static mHttpHelper helper = new mHttpHelper();
        #endregion

        public static string HttpGet(this string url, string cookies = "", string referer = "")
        {
            var item = new mHttpItem
            {
                URL = url,
                UserAgent = UserAgent,
                TimeOut = TimeOut
            };
            if (!string.IsNullOrEmpty(cookies))
                item.InitCookie = cookies;
            if (!string.IsNullOrEmpty(referer))
                item.Referer = referer;

            var result = helper.GetHttpResult(item);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return result.StrResult;
            }
            else
                return result.RequestException.Message;
        }

        public static string HttpPost(this string url, string postData = "")
        {
            var item = new mHttpItem
            {
                URL = url,
                MethodItem = new System.Net.Http.HttpMethod("POST"),
                UserAgent = UserAgent,
                TimeOut = TimeOut
            };
            if (!string.IsNullOrEmpty(postData)) item.PostData = postData;
            var result = helper.GetHttpResult(item);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return result.StrResult;
            }
            else
                return result.RequestException.Message;
        }
        public static Image HttpImg(this string url)
        {
            var item = new mHttpItem
            {
                URL = url,
                UserAgent = UserAgent,
                TimeOut = TimeOut
            };
            var result = helper.GetHttpResult(item);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Image img = result.ImageResult;
                return img;
            }
            else
                return null;
        }
    }
}
