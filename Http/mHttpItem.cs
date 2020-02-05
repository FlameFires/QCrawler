using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace QCrawler
{
    /// <summary>
    /// 请求设置类
    /// </summary>
    public class mHttpItem
    {
        public mHttpItem()
        {

        }
        /// <summary>
        /// 请求的地址
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// 请求的方法类型(默认为Get)
        /// </summary>
        public HttpMethod MethodItem { get; set; } = HttpMethod.Get;

        /// <summary>
        /// Accept
        /// </summary>
        public string Accept { get; set; } = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
        /// <summary>
        /// UserAgent
        /// </summary>
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
        /// <summary>
        /// Referer
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; } = "application/x-www-form-urlencoded";
        /// <summary>
        /// 初始化的Cookie
        /// 如果不需要不用填写,默认为空字符串
        /// </summary>
        public string InitCookie { get; set; } = string.Empty;
        /// <summary>
        /// 按照字符串提交时选用
        /// </summary>
        public string PostData { get; set; }

        /// <summary>
        /// 按照字节数组提交时选用
        /// </summary>
        public byte[] PostDatas { get; set; } = new byte[0];
        /// <summary>
        /// Post时选择的类型,如果是上传文件则为BytesType
        /// 默认值(StringType)
        /// </summary>
        public PostTypeEnum PostType { get; set; } = PostTypeEnum.StringType;
        /// <summary>
        /// 写入Pdata时使用的编码类型
        /// 默认为UTF8
        /// </summary>
        public Encoding RequestEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        ///  GetResponse 和 GetRequestStream 方法的超时值（以毫秒为单位）。
        ///  默认(10秒 10000毫秒)
        /// </summary>
        public int TimeOut { get; set; } = 10*1000;

        /// <summary>
        /// 读取或写入 GetResponseStream(主要影响读取)
        /// GetRequestStream 方法的超时值（以毫秒为单位）
        /// 如果返回数据比较慢,并且你不关心返回结果时,这个超时你可以设置的很短.避免大量的死链接占用通道
        /// 默认(3秒 3000毫秒)
        /// </summary>
        public int ReadWriteTimeOut { get; set; } = 3000;

        /// <summary>
        /// 如果你不关心返回结果,设置它为true即可.
        /// 是否不解析ResponseStream 
        /// 默认为false
        /// </summary>
        public bool IsSoMode { get; set; }

        /// <summary>
        /// 请求时代理类型,
        /// SystemProxy 使用系统代理(IE设置的代理)
        /// NewProxy  使用新的自定义代理
        /// 默认不使用代理
        /// </summary>
        public HttpProxyType ProxyType { get; set; } = HttpProxyType.NoProxy;
        /// <summary>
        /// 当代理模式为NewProxy时设置的IP地址
        /// 格式: 127.0.0.1
        /// </summary>
        public string ProxyIP { get; set; }
        /// <summary>
        /// 当代理模式为NewProxy时设置的端口号
        /// 格式: 1234
        /// </summary>
        public int ProxyPort { get; set; }
        /// <summary>
        /// 当代理模式为NewProxy时设置的用户名 
        /// </summary>
        public string ProxyUserName { get; set; }
        /// <summary>
        /// 当代理模式为NewProxy时设置的密码
        /// </summary>
        public string ProxyPass { get; set; }

        /// <summary>
        /// 如果需要携带证书,那么就填写证书绝对路径 
        /// </summary>
        public string CerPath { get; set; }
        /// <summary>
        /// 如果需要携带证书,并且需要密码,则填写
        /// </summary>
        public string CerPass { get; set; }

        /// <summary>
        /// 如果提示以下错误,请设置为true
        /// 服务器提交了协议冲突. Section=ResponseHeader Detail=CR 后面必须是 LF
        /// </summary>
        public bool UseUnsafe { get; set; }

        /// <summary>
        /// 字符串头(可以直接从浏览器粘贴)
        /// 如果设置了这里, httpItem.UserAgent httpItem.Referer httpItem.ContentType 将无效 
        /// </summary>
        public string HeaderStr { get; set; }
        /// <summary>
        /// 本地出口地址例如
        /// items.LocalEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.123"),80);
        /// 192.168.1.123 为本地网卡 
        /// 80 为端口号
        /// </summary>
        public EndPoint LocalEndPoint { get; set; }
    }

    /// <summary>
    /// 代理的类型
    /// </summary>
    public enum HttpProxyType
    {
        /// <summary>
        /// 使用系统代理
        /// </summary>
        SystemProxy,
        /// <summary>
        /// 使用新的自定义代理
        /// </summary>
        NewProxy,
        /// <summary>
        /// 不使用代理
        /// </summary>
        NoProxy
    }
    /// <summary>
    /// Post的类型
    /// </summary>
    public enum PostTypeEnum
    {
        /// <summary>
        /// 字符串类型提交
        /// </summary>
        StringType,
        /// <summary>
        /// 字节数组类型提交
        /// </summary>
        BytesType

    }
}
