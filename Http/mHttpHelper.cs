/* 
 * 当前版本:v5.5
 * 次版本号:19.11.23(更新日期)
 * m基类库 Http请求类
 * 基础功能1:基于HttpWebRequest方式同步提交数据 包含(Get/Post)
 * 基础功能2:集合常用字符串处理/时间转换处理
 * 
 * Coding by 君临
 * 2019-11-23
 * 
 * 更新日志
 *              Cookie处理部分
 *              1.SetHeaders如果传递Cookie项 则添加
 *              2.如果存在相同CookieName 则更新当前Container对象中存放的数据
 *              
 *              助手类处理部分
 *              1.增加一个CustomDatetime 自定义时间类,免去频繁书写获取各种时间的麻烦
 *              2.增加获取过去几日(GetPastDays) 未来几日(GetFutureDays)集合对象
 *              3.增加对某一个时间获取时间戳 GetTimeByCSharp13 原先只能获取当前时间.现在不传参数获取当前时间,传递后获取指定时间的时间戳.
 * 
 * 更新地址
 * http://bbs.m.com/forum.php?mod=post&action=edit&fid=37&tid=11&pid=58&page=1
 * 官方文档
 * http://bbs.m.com/forum.php?mod=forumdisplay&fid=37&filter=typeid&typeid=23
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace QCrawler
{
    /// <summary>
    /// 重新封装的HttpCode
    /// </summary>
    public class mHttpHelper
    {
        public mHttpHelper()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            //并发数原始只有2
            ServicePointManager.DefaultConnectionLimit = 1024;
            ServicePointManager.SetTcpKeepAlive(true, 5000, 2000);//使用TCP的检测方式  testing 

            //证书回调验证
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { return true; };
        }
        /// <summary>
        /// 每次使用时对item进行赋值
        /// </summary>
        /// <param name="_HttpItem">请求的配置项</param>
        public mHttpHelper(mHttpItem _HttpItem) : this()
        {
            HttpItem = _HttpItem;
        }
        #region 私有字段
        /// <summary>
        /// 请求的主要类
        /// </summary>
        HttpWebRequest request = null;
        /// <summary>
        /// Cookie的主要类
        /// </summary>
        CookieContainer Container = new CookieContainer();
        /// <summary>
        /// HttpWebRequest的属性
        /// </summary>
        PropertyInfo[] PropertyInfos => typeof(HttpWebRequest).GetProperties();
        /// <summary>
        /// 需要单独设置的字符串
        /// 直接使用反射获得效率太慢,这边直接写死并且移除Host,Method,Connection
        /// typeof(System.Net.HttpWebRequest).GetProperties().Where(x => x.PropertyType.Name == "String").Select(x=>x.Name).ToArray()
        /// </summary>
        string[] PropertyInfoStrings => "ConnectionGroupName,ContentType,MediaType,TransferEncoding,Accept,Referer,UserAgent,Expect".ToLower().Split(',');
        /// <summary>
        /// 获取应答结果的主要类
        /// </summary>
        HttpWebResponse response;
        /// <summary>
        /// 得到应答内容流
        /// </summary>
        MemoryStream memoryStream;
        /// <summary>
        /// 读取数据的缓存
        /// </summary>
        byte[] buffer;
        /// <summary>
        /// 设置请求的类
        /// </summary>
        mHttpItem HttpItem { get; set; }
        #endregion

        #region 公开的属性 

        /// <summary>
        /// 返回的结果类
        /// </summary>
        public mHttpResult httpResult { get; set; } = new mHttpResult();
        #endregion
        /// <summary>
        /// 服务器提交了协议冲突. Section=ResponseHeader Detail=CR 后面必须是 LF
        /// </summary>
        /// <param name="useUnsafe">是否开启</param>
        void SetAllowUnsafeHeaderParsing20(bool useUnsafe)
        {
            if (HttpItem.UseUnsafe)
            {
                System.Reflection.Assembly aNetAssembly = System.Reflection.Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
                if (aNetAssembly != null)
                {

                    Type aSettingsType = aNetAssembly.GetType("System.Net.Configuration.SettingsSectionInternal");
                    if (aSettingsType != null)
                    {

                        object anInstance = aSettingsType.InvokeMember("Section",
                          System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.NonPublic, null, null, new object[] { });

                        if (anInstance != null)
                        {
                            System.Reflection.FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (aUseUnsafeHeaderParsing != null)
                            {
                                aUseUnsafeHeaderParsing.SetValue(anInstance, useUnsafe);

                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 设置请求头部
        /// </summary>
        void InitRequestHeader()
        {
            #region 设置基础数据 
            request = (HttpWebRequest)HttpWebRequest.Create(HttpItem.URL);
            request.Method = HttpItem.MethodItem.Method;
            request.Accept = HttpItem.Accept;
            request.UserAgent = HttpItem.UserAgent;
            request.Referer = HttpItem.Referer;
            request.Timeout = HttpItem.TimeOut;
            request.ReadWriteTimeout = HttpItem.ReadWriteTimeOut;
            request.AllowAutoRedirect = true;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None;
            InitCookie();
            SetHeaders();
            request.CookieContainer = Container;
            request.MaximumAutomaticRedirections = 9999;
            request.ServicePoint.UseNagleAlgorithm = false;//关闭小包优化 testing 
            request.AllowWriteStreamBuffering = false;//禁用缓冲
            //本地网卡出口
            if (HttpItem.LocalEndPoint != null)
            {
                request.ServicePoint.BindIPEndPointDelegate = (servicePoint, remoteEndPoint, retryCount) => { return (IPEndPoint)HttpItem.LocalEndPoint; };
            }
            #endregion 
            SetCer();
            SetProxy();
            SetAllowUnsafeHeaderParsing20(HttpItem.UseUnsafe);
            SetPostData();
        }
        void InitCookie()
        {
            if (!string.IsNullOrEmpty(HttpItem.InitCookie))
            {
                //如果有初始Cookie则赋值
                AddCookies(HttpItem.InitCookie);
            }
        }
        /// <summary>
        /// 检测用户输入的头是否标准设置,
        /// 如果包含则返回其对应的C#属性名.
        /// 否则返回空字符串
        /// </summary>
        /// <returns>属性名或空字符串</returns>
        string CheckIsStringPropertyInfo(string key)
        {
            return PropertyInfoStrings.FirstOrDefault(x => x.ToString() == key)?.ToString();
        }
        /// <summary>
        /// 通过反射设置Request头
        /// </summary>
        void SetHeaders()
        {
            if (!string.IsNullOrEmpty(HttpItem.HeaderStr))
            {
                var chs = HttpItem.HeaderStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in chs)
                {
                    try
                    {
                        if (!item.StartsWith(":"))//过滤掉2.0协议
                        {
                            //每次都还原key和value.否则单纯按照=进行split一旦内容部分包含:则会出现错误
                            int index = item.IndexOf(":");
                            string key = item.Substring(0, index).ToLower();
                            string value = item.Substring(index + 1).Trim();
                            if (key == "cookie")
                            {
                                AddCookies(value);
                            }
                            //user-agent  content-type http命名规则与C#不同,所以检测时去除中间-
                            string chkey = CheckIsStringPropertyInfo(key.Replace("-", ""));
                            if (!string.IsNullOrEmpty(chkey))
                            {
                                //如果存在,使用反射设置属性
                                SetPropertyValue(chkey, value);
                            }
                            else
                            {
                                //非必须使用属性设置值时跳过host直接添加
                                if (key != "host" && key != "connection")
                                {
                                    request.Headers.Add(key, value);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
            }

        }
        /// <summary>
        /// 通过反射设置属性值
        /// </summary>
        /// <param name="key">属性名</param>
        /// <param name="value">值</param>
        void SetPropertyValue(string key, string value)
        {
            try
            {
                PropertyInfo propertyInfo = PropertyInfos.Where(x => x.Name.ToLower() == key.ToLower()).FirstOrDefault(); //获取指定名称的属性
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(request, value, null);
                }
            }
            catch
            {
                throw;
            }

        }
        /// <summary>
        /// 设置代理信息
        /// </summary>
        void SetProxy()
        {
            IWebProxy webProxy = null;
            if (!string.IsNullOrEmpty(HttpItem.ProxyIP) && HttpItem.ProxyPort > 0)
            {
                HttpItem.ProxyType = HttpProxyType.NewProxy;
            }
            switch (HttpItem.ProxyType)
            {
                case HttpProxyType.SystemProxy:
                    webProxy = HttpWebRequest.GetSystemWebProxy();
                    break;
                case HttpProxyType.NewProxy:
                    webProxy = new WebProxy(HttpItem.ProxyIP, HttpItem.ProxyPort);
                    //127.0.0.1:1234  
                    if (!string.IsNullOrEmpty(HttpItem.ProxyUserName) && !string.IsNullOrEmpty(HttpItem.ProxyPass))
                    {
                        webProxy.Credentials = new NetworkCredential()
                        {
                            UserName = HttpItem.ProxyUserName,
                            Password = HttpItem.ProxyPass
                        };
                    }
                    else
                    {
                        webProxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    }
                    break;
                case HttpProxyType.NoProxy:
                default:
                    webProxy = null;
                    break;
            }
            request.Proxy = webProxy;
        }
        /// <summary>
        /// 设置证书
        /// </summary>
        void SetCer()
        {
            if (!string.IsNullOrEmpty(HttpItem.CerPath))
            {
                //这是一种调用证书的方式,适合被导出的其他证书,并且需要携带
                X509Certificate x509;
                if (!string.IsNullOrEmpty(HttpItem.CerPass))
                {
                    x509 = new X509Certificate(HttpItem.CerPath, HttpItem.CerPass);
                }
                else
                {
                    x509 = new X509Certificate(HttpItem.CerPath);
                }
                request.ClientCertificates.Add(x509);
            }
        }
        /// <summary>
        /// 设置Post提交的数据
        /// </summary>
        void SetPostData()
        {
            if (HttpItem.MethodItem == HttpMethod.Post)
            {
                request.ContentType = HttpItem.ContentType;
                buffer = HttpItem.PostType == PostTypeEnum.BytesType ? HttpItem.PostDatas : HttpItem.RequestEncoding.GetBytes(HttpItem.PostData);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);

            }
        }
        /// <summary>
        /// 获取数据内容
        /// </summary>
        void GetRespones()
        {
            //这个位置上基本就是最耗时的部分了. request.BeginGetResponse
            using (response = (HttpWebResponse)request.GetResponse())
            {
                //如果为so就不解析数据
                if (!HttpItem.IsSoMode)
                {
                    using (memoryStream = new MemoryStream())
                    {
                        #region 处理压缩过的流

                        Stream ResponseStream;
                        if (response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ResponseStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                        }
                        else if (response.ContentEncoding.Equals("deflate", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ResponseStream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress);
                        }
                        else
                        {
                            ResponseStream = response.GetResponseStream();
                        }
                        #endregion
                        #region 还原压缩的数据
                        //这个位置上基本就是最耗时的部分了.
                        using (ResponseStream)
                        {
                            buffer = new byte[1024];
                            int readCount = ResponseStream.Read(buffer, 0, buffer.Length);
                            while (readCount > 0)
                            {
                                memoryStream.Write(buffer, 0, readCount);
                                readCount = ResponseStream.Read(buffer, 0, buffer.Length);
                            }
                        }
                        #endregion
                        httpResult.BytesResult = memoryStream.ToArray();
                    }
                }
                httpResult.StatusCode = response.StatusCode;
                httpResult.ResponseHeader = response.Headers;
                response.Close();
            }
            if (request.HaveResponse)
            {
                request.Abort();
            }
            GC.Collect();
        }


        /// <summary>
        /// 获取Http请求的内容
        /// </summary>
        /// <param name="HttpItem">设置的请求类型</param>
        /// <returns>返回的结果类</returns>
        public mHttpResult GetHttpResult(mHttpItem httpItem = null)
        {
            try
            {
                if (httpItem != null)
                {
                    HttpItem = httpItem;
                }
                InitRequestHeader();
                GetRespones();
            }
            catch (Exception ex)
            {
                //request.Abort();
                request = null;
                httpResult.RequestException = ex;
                GC.Collect();
            }

            return httpResult;
        }

        #region Cookie操作

        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="cookie">Cookie实体对象</param>
        public void AddCookie(Cookie cookie)
        {
            if (string.IsNullOrEmpty(cookie.Domain))
            {
                cookie.Domain = request?.RequestUri.Host;
            }
            if (string.IsNullOrEmpty(GetCookieValue(cookie.Name)))
            {
                //如果Cookie不存在则添加
                Container.Add(cookie);
            }
            else
            {
                //否则更新
                UpdateCookie(cookie.Name, cookie.Value);
            }

        }
        /// <summary>
        /// 批量添加Cookie
        /// </summary>
        /// <param name="CookieStr">Cookie的字符串数据</param>
        public void AddCookies(string CookieStr)
        {
            var cookiestrs = CookieStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < cookiestrs.Length; i++)
            {
                var spCookie = cookiestrs[i].Split('=');
                try
                {
                    AddCookie(new Cookie(spCookie[0].Trim(), spCookie[1].Trim()));
                }
                catch
                {
                    throw;
                }

            }

        }

        /// <summary>
        /// 更新Cookie数据
        /// </summary>
        /// <param name="ckName">Cookie名</param>
        /// <param name="ckValue">Cookie值</param>
        public void UpdateCookie(string ckName, string ckValue)
        {
            var cc = Container.GetCookies(request.RequestUri);
            cc[ckName].Value = ckValue;
        }
        /// <summary>
        /// 删除Cookie项(设置过期时间)
        /// </summary>
        /// <param name="ckname">需要设置的Cookie名</param>
        public void DelCookie(string ckname)
        {
            var cc = Container.GetCookies(request.RequestUri);
            cc[ckname].Expires = DateTime.Now.AddYears(-1);
            if (cc[ckname].Expired == false)
            {
                cc[ckname].Expired = true;
            }
        }

        /// <summary>
        /// 获取某一项Cookie值
        /// </summary>
        /// <param name="ckname">Cookie名</param>
        /// <returns>对应的Cookie值</returns>
        public string GetCookieValue(string ckname)
        {
            var cc = Container.GetCookies(request.RequestUri);
            return cc[ckname]?.Value;
        }
        /// <summary>
        /// 获取所有Cookie信息
        /// </summary>
        /// <returns>CookieContainer对象中的所有信息</returns>
        public string GetCookieString()
        {
            return GetCookies();//Container.GetCookieHeader(request.RequestUri);
        }

        string GetCookies()
        {
            System.Collections.Generic.List<Cookie> lstCookies = new System.Collections.Generic.List<Cookie>();
            Hashtable table = (Hashtable)Container.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, Container, new object[] { });
            StringBuilder sb = new StringBuilder();
            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies)
                    {
                        sb.Append(c.Name).Append("=").Append(c.Value).Append(";");
                    }
            }
            return sb.ToString();
        }
        #endregion
    }
}
