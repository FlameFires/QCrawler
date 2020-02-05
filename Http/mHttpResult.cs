using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace QCrawler
{
    /// <summary>
    /// 返回的结果类
    /// </summary>
    public class mHttpResult
    {
        /// <summary>
        /// 返回的Byte数组结果
        /// </summary>
        public byte[] BytesResult { get; set; } = new byte[0];

        /// <summary>
        /// 返回的字符串结果
        /// 数据来源于BytesResult,
        /// 如果为空,则需要判断RequestException
        /// </summary>
        public string StrResult { get => BytesResult.Length == 0 ? "" : ResultEncoding.GetString(BytesResult); }
        /// <summary>
        /// 返回的图片结果
        /// 数据来源于BytesResult,
        /// 如果为空,则需要判断RequestException
        /// </summary>
        public Image ImageResult { get => BytesResult.Length == 0 ? null : Image.FromStream(new MemoryStream(BytesResult)); }
        /// <summary>
        /// 请求结果转换为字符串时使用的编码类型
        /// 默认为UTF8
        /// </summary>
        public Encoding ResultEncoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 请求异常时会产生结果,
        /// 如果没有异常表示请求成功
        /// </summary>
        public Exception RequestException { get; set; }

        /// <summary>
        /// 请求回应的Header集合
        /// </summary>
        public WebHeaderCollection ResponseHeader { get; set; }
        /// <summary>
        /// 表明当前请求状态
        /// 为OK时则表示请求成功.具体参照HttpStatusCode描述
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }
}
