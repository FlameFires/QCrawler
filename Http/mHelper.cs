using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QCrawler
{
    /// <summary>
    /// 常用方法助手类
    /// </summary>
    public class mHelper
    {
        #region 设置/获取当前/指定日期时间戳方法 / GMT时间与本地时间互转
        /// <summary>
        /// 获取过去几日的集合(n天前至今所有的日期)
        /// 注意,默认时间包含今日.(最后一天为今天) 
        /// 如果只取日期字符串部分:
        /// new mHelper().GetPastDays().Select(v=>v.CurrentDateStr).ToArray()
        /// </summary>
        /// <param name="day">默认为7天前</param>
        /// <returns>过去几日的集合</returns>
        public List<CustomDatetime> GetPastDays(int day = -7)
        {
            List<CustomDatetime> daylist = new List<CustomDatetime>();

            DateTime dtnow = DateTime.Now;
            DateTime dtlast = DateTime.Now.Date.AddDays(day);
            for (; dtlast.CompareTo(dtnow) < 0;)
            {
                daylist.Add(new CustomDatetime(dtlast));
                dtlast = dtlast.AddDays(1);
            }
            return daylist;
        }
        /// <summary>
        /// 获取未来几日的集合(今天至未来间所有的日期)
        /// 注意,默认时间包含今日.(第一天为今天) 
        /// 如果只取日期字符串部分:
        /// new mHelper().GetFutureDays().Select(v=>v.CurrentDateStr).ToArray()
        /// </summary>
        /// <param name="day">默认为7天后</param>
        /// <returns>过去几日的集合</returns>
        public List<CustomDatetime> GetFutureDays(int day = 7)
        {
            List<CustomDatetime> daylist = new List<CustomDatetime>();
            DateTime dtnow = DateTime.Now;
            DateTime dtlast = DateTime.Now.Date.AddDays(day);
            for (; dtnow.CompareTo(dtlast) < 0;)
            {
                daylist.Add(new CustomDatetime(dtnow));
                dtnow = dtnow.AddDays(1);
            }
            daylist.Add(new CustomDatetime(dtlast));
            return daylist;
        }
        /// <summary>
        /// 获取当前时间yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <returns>当前时间yyyy-MM-dd HH:mm:ss</returns>
        public string GetDateTimeNowString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 获取当前时间
        /// 仅时间部分 HH:mm:ss
        /// </summary>
        /// <returns>获取当前时间</returns>
        public string GetTimeString()
        {
            return DateTime.Now.ToShortTimeString();
        }
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);

        }
        /// <summary>
        /// 返回13位时间戳 非JS方式
        /// 如果传参则获取指定时间戳
        /// 默认获取当前时间戳
        /// </summary>
        /// <param name="dt">如果传入则获取指定时间时间戳</param>
        /// <returns>时间戳字符串</returns>
        public string GetTimeByCSharp13(DateTime dt = default(DateTime))
        {
            if (dt == default(DateTime))
            {
                return (DateTime.UtcNow - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0))).TotalMilliseconds.ToString("0");
            }
            else
            {
                return GetTimeToStamp(dt);
            }

        }

        /// <summary>  
        /// 获取时间戳 C# 10位 
        /// </summary>  
        /// <returns>10位时间戳</returns>  
        public string GetTimeByCSharp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        /// <summary>
        /// 指定时间转换时间戳
        /// </summary>
        /// <param name="time">要转换的时间</param>
        /// <returns>13位时间戳</returns>
        public string GetTimeToStamp(DateTime time)
        {
            return (time - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds.ToString("0");

        }

        /// <summary>
        /// 获取服务器返回的时间,如果Header中没有Date则返回当前时间
        /// </summary>
        /// <param name="Date">请求结果对象</param>
        /// <returns>返回本地时区Datatime数据</returns>
        public DateTime GetServerTime(string Date)
        {
            try
            {
                return GetTime4Gmt(Date);
            }
            catch
            {

                return DateTime.Now;
            }

        }
        /// <summary>
        /// 本地时间转成GMT时间 (参数如果不传入则为当前时间)
        /// 本地时间为：2011-9-29 15:04:39
        /// 转换后的时间为：Thu, 29 Sep 2011 07:04:39 GMT
        /// </summary>
        /// <param name="dt">参数如果不传入则为当前时间 DateTime.Now</param>
        /// <returns></returns>
        public string GetTimeToGMTString(DateTime dt = default(DateTime))
        {
            if (dt == default(DateTime))
            {
                dt = DateTime.Now;
            }
            return dt.ToUniversalTime().ToString("r");
        }

        /// <summary>
        ///本地时间转成GMT格式的时间(参数如果不传入则为当前时间)
        ///本地时间为：2011-9-29 15:04:39
        ///转换后的时间为：Thu, 29 Sep 2011 15:04:39 GMT+0800
        /// </summary>
        /// <param name="dt">参数如果不传入则为当前时间 DateTime.Now</param>
        /// <returns></returns>
        public string GetTimeToGMTFormat(DateTime dt = default(DateTime))
        {

            if (dt == default(DateTime))
            {
                dt = DateTime.Now;
            }
            return dt.ToString("r") + dt.ToString("zzz").Replace(":", "");
        }

        /// <summary>  
        /// GMT时间转成本地时间  
        /// DateTime dt1 = GMT2Local("Thu, 29 Sep 2011 07:04:39 GMT");
        /// 转换后的dt1为：2011-9-29 15:04:39
        /// DateTime dt2 = GMT2Local("Thu, 29 Sep 2011 15:04:39 GMT+0800");
        /// 转换后的dt2为：2011-9-29 15:04:39
        /// </summary>  
        /// <param name="gmt">字符串形式的GMT时间</param>  
        /// <returns></returns>  
        public DateTime GetTime4Gmt(string gmt)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                string pattern = "";
                if (gmt.IndexOf("+0") != -1)
                {
                    gmt = gmt.Replace("GMT", "");
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss zzz";
                }
                if (gmt.ToUpper().IndexOf("GMT") != -1)
                {
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
                }
                if (pattern != "")
                {
                    dt = DateTime.ParseExact(gmt, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    dt = dt.ToLocalTime();
                }
                else
                {
                    dt = Convert.ToDateTime(gmt);
                }
            }
            catch
            {
            }
            return dt;
        }

        #endregion
        #region 字符串处理方法
        /// <summary>
        /// 取文本中间
        /// </summary>
        /// <param name="allStr">原字符</param>
        /// <param name="firstStr">前面的文本</param>
        /// <param name="lastStr">后面的文本</param>
        /// <returns>返回获取的值</returns>
        public string GetStringMid(string allStr, string firstStr, string lastStr)
        {
            //取出前面的位置
            int index1 = allStr.IndexOf(firstStr);
            //取出后面的位置
            int index2 = allStr.IndexOf(lastStr, index1 + firstStr.Length);

            if (index1 < 0 || index2 < 0)
            {
                return "";
            }
            //定位到前面的位置
            index1 = index1 + firstStr.Length;
            //判断要取的文本的长度
            index2 = index2 - index1;

            if (index1 < 0 || index2 < 0)
            {
                return "";
            }
            //取出文本
            return allStr.Substring(index1, index2);
        }
        /// <summary>
        /// 批量取文本中间
        /// </summary>
        /// <param name="allStr">原字符</param>
        /// <param name="firstStr">前面的文本</param>
        /// <param name="lastStr">后面的文本</param>
        /// <param name="regexCode">默认为万能表达式(.*?)</param>
        /// <returns>返回结果集合</returns>
        public List<string> GetStringMids(string allStr, string firstStr, string lastStr, string regexCode = "(.*?)")
        {
            List<string> list = new List<string>();
            string reString = string.Format("{0}{1}{2}", firstStr, regexCode, lastStr);
            Regex reg = new Regex(reString);
            MatchCollection mc = reg.Matches(allStr);
            for (int i = 0; i < mc.Count; i++)
            {
                GroupCollection gc = mc[i].Groups; //得到所有分组 
                for (int j = 1; j < gc.Count; j++) //多分组
                {
                    string temp = gc[j].Value;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        list.Add(temp);
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// Url编码,encoding默认为utf8编码
        /// </summary>
        /// <param name="str">需要编码的字符串</param>
        /// <param name="encoding">指定编码类型</param>
        /// <returns>编码后的字符串</returns>
        public string UrlEncoding(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                return System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            }
            else
            {
                return System.Web.HttpUtility.UrlEncode(str, encoding);
            }
        }

        /// <summary>
        /// Url解码,encoding默认为utf8编码
        /// </summary>
        /// <param name="str">需要解码的字符串</param>
        /// <param name="encoding">指定解码类型</param>
        /// <returns>解码后的字符串</returns>
        public string UrlDecoding(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                return System.Web.HttpUtility.UrlDecode(str, Encoding.UTF8);
            }
            else
            {
                return System.Web.HttpUtility.UrlDecode(str, encoding);
            }
        }

        /// <summary>
        /// Html解码
        /// </summary>
        /// <param name="str">需要解码的字符</param>
        /// <returns></returns>
        public string HtmlDecode(string str)
        {
            string[] strsx = str.Split('△');
            if (strsx.Length > 1)
            {
                return FromUnicodeString(strsx[0], strsx[1].Trim());//Decode2Html(strsx[0], strsx[1].Trim());
            }
            else
            {
                return Decode2Html(str);
            }
        }
        /// <summary>
        /// 解析任意符号开头的编码后续数据符合Hex编码
        /// </summary>
        /// <param name="param"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        private string Decode2Html(string param, string sp = "&#")
        {

            string[] paramstr = param.Replace(sp, sp + " ").Replace(sp, "").Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string str = string.Empty;
            foreach (string item in paramstr)
            {
                try
                {

                    str += (char)int.Parse(item, System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    str += item;
                    continue;
                }
            }
            // 如果失败请尝试这种办法 可解决变异HTML 开头非&# :->  StringWriter myWriter = new StringWriter(); System.Web.HttpUtility.HtmlDecode(param,myWriter); return myWriter.ToString(); 
            return str;
        }

        /// <summary>
        /// Html编码 
        /// </summary>
        /// <param name="param">需要编码的字符</param>
        /// <returns>返回编码后数据</returns>
        public string HtmlEncode(string param)
        {
            string str = string.Empty;
            foreach (char item in param.ToCharArray())
            {
                try
                {
                    str += "&#" + Convert.ToInt32(item).ToString("x4") + " ";
                }
                catch
                {
                    str += "ToHtml Error";
                }
            }

            return str;
        }

        /// <summary>
        /// 取文本右边 
        /// 默认取出右边所有文本,如果需要取固定长度请设置 length参数
        /// 异常则返回空字符串
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="right">需要确认位置的字符串</param>
        /// <param name="length">默认0,如果设置按照设置的值取出数据</param>
        /// <returns>返回结果</returns>
        public string Right(string str, string right, int length = 0)
        {
            int pos = str.IndexOf(right, StringComparison.Ordinal);
            if (pos < 0) return "";
            int len = str.Length;
            if (len - pos - right.Length <= 0) return "";
            string result = "";
            if (length == 0)
            {
                result = str.Substring(pos + right.Length, len - (pos + right.Length));
            }
            else
            {
                result = str.Substring(pos + right.Length, length);
            }
            return result;
        }
        /// <summary>
        ///  取文本左边
        ///  默认取出左边所有文本,如果需要取固定长度请设置 length参数
        /// 异常则返回空字符串
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="left">需要确认位置的字符串</param>
        /// <param name="length">默认0,如果设置按照设置的值取出数据</param>
        /// <returns>返回结果</returns>
        public string Left(string str, string left, int length = 0)
        {
            var pos = str.IndexOf(left, StringComparison.Ordinal);
            if (pos < 0) return "";
            string result = "";
            if (length == 0)
            {
                result = str.Substring(0, pos);
            }
            else
            {
                result = str.Substring(length, pos);
            }
            return result;
        }
        /// <summary>
        /// 取文本中间 正则方式
        /// </summary>
        /// <param name="html">原始Html</param>
        /// <param name="s">开始字符串</param>
        /// <param name="e">结束字符串</param>
        /// <returns>返回获取结果</returns>
        public string GetMidHtml(string html, string s, string e)
        {
            string rx = string.Format("{0}{1}{2}", s, "([\\s\\S]*?)", e);
            if (Regex.IsMatch(html, rx, RegexOptions.IgnoreCase))
            {
                Match match = Regex.Match(html, rx, RegexOptions.IgnoreCase);
                if (match != null && match.Groups.Count > 0)
                {
                    return match.Groups[1].Value.Trim();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Unicode字符转汉字 允许自定义分隔字符
        /// </summary>
        /// <param name="str">需要转换的字符串</param>
        /// <param name="SplitString">分隔字符</param>
        /// <param name="TrimStr">如果有尾部数据则填写尾部</param>
        /// <returns>处理后结果</returns>
        public string FromUnicodeString(string str, string SplitString = "u", string TrimStr = ";")
        {
            string regexCode = SplitString == "u" ? "\\\\u(\\w{1,4})" : SplitString + "(\\w{1,4})";
            string reString = str;
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regexCode);
            System.Text.RegularExpressions.MatchCollection mc = reg.Matches(reString);
            for (int i = 0; i < mc.Count; i++)
            {
                try
                {
                    var outs = (char)int.Parse(mc[i].Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
                    if (str.IndexOf(mc[i].Groups[0].Value + TrimStr) > 0)
                    {
                        //如果出现(封号);结尾则连带符号替换
                        str = str.Replace(mc[i].Groups[0].Value + TrimStr, outs.ToString());
                    }
                    else
                    {
                        str = str.Replace(mc[i].Groups[0].Value, outs.ToString());
                    }
                }
                catch
                {
                    continue;
                }
            }
            return str;
        }
        /// <summary>
        /// 汉字转Unicode字符 默认\u1234 
        /// </summary>
        /// <param name="param">需要转换的字符</param>
        /// <param name="SplitString">分隔结果</param>
        /// <returns>转换后结果</returns>
        public string GetUnicodeString(string param, string SplitString = "u")
        {
            string outStr = "";
            for (int i = 0; i < param.Length; i++)
            {
                try
                {
                    outStr += "\\" + SplitString + ((int)param[i]).ToString("x4");
                }
                catch
                {
                    outStr += param[i];
                    continue;
                }

            }

            return outStr;
        }
        /// <summary>
        /// 将字符串转换为base64格式 默认UTF8编码
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>结果</returns>
        public string GetString2Base64(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return Convert.ToBase64String(encoding.GetBytes(str));
        }
        /// <summary>
        /// base64字符串转换为普通格式 默认UTF8编码
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>结果</returns>
        public string GetStringbyBase64(string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] buffer = Convert.FromBase64String(str);
            return encoding.GetString(buffer);
        }
        /// <summary>
        /// 将byte数组转换为AscII字符
        /// </summary>
        /// <param name="b">需要操作的数组</param>
        /// <returns>结果</returns>
        public string GetAscii2string(byte[] b)
        {
            string str = "";
            for (int i = 7; i < 19; i++)
            {
                str += (char)b[i];
            }
            return str;
        }

        /// <summary>
        /// 将字节数组转化为十六进制字符串，每字节表示为两位
        /// </summary>
        /// <param name="bytes">需要操作的数组</param>
        /// <param name="start">起始位置</param>
        /// <param name="len">长度</param>
        /// <returns>字符串结果</returns>
        public string Bytes2HexString(byte[] bytes, int start, int len)
        {
            string tmpStr = "";

            for (int i = start; i < (start + len); i++)
            {
                tmpStr = tmpStr + bytes[i].ToString("X2");
            }

            return tmpStr;
        }
        /// <summary>
        /// 字符串转16进制
        /// </summary>
        /// <param name="mHex">需要转换的字符串</param>
        /// <returns>返回十六进制代表的字符串</returns>
        public string HexToStr(string mHex) // 返回十六进制代表的字符串 
        {
            byte[] bTemp = System.Text.Encoding.Default.GetBytes(mHex);
            string strTemp = "";
            for (int i = 0; i < bTemp.Length; i++)
            {
                strTemp += bTemp[i].ToString("X");
            }
            return strTemp;


        }
        /// <summary>
        /// 将十六进制字符串转化为字节数组
        /// </summary>
        /// <param name="src">需要转换的字符串</param>
        /// <returns>结果数据</returns>
        public byte[] HexString2Bytes(string src)
        {
            byte[] retBytes = new byte[src.Length / 2];

            for (int i = 0; i < src.Length / 2; i++)
            {
                retBytes[i] = byte.Parse(src.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return retBytes;
        }
        #endregion
    }

    /// <summary>
    /// 自定义时间
    /// 包含当前时间的 起始时间 00:00:00 结束时间 23:59:59
    /// 经常要写时间的字符串(基本就是时间的00-23),所以封装一个这个时间类.
    /// </summary>
    public class CustomDatetime
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime CurrentDateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 当前时间的字符串格式
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string CurrentDateTimeStr { get => CurrentDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        /// <summary>
        /// 当前时间的年月日部分
        /// </summary>
        public string CurrentDateStr { get => CurrentDateTime.ToShortDateString(); }
        /// <summary>
        /// 当前日期的时分秒部分
        /// </summary>
        public string CurrentTimeStr { get => CurrentDateTime.ToShortTimeString(); }
        /// <summary>
        /// 自定义时间格式,如果传递时间,则显示传递时间
        /// </summary>
        /// <param name="dt">自定义时间</param>
        public CustomDatetime(DateTime dt = default(DateTime))
        {
            if (dt != default(DateTime))
            {
                CurrentDateTime = dt;
            }
        }
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime StartTime { get => DateTime.Parse($"{CurrentDateTime.ToShortDateString()} 00:00:00"); }
        /// <summary>
        /// 起始时间字符串格式
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string StartTimeStr { get => StartTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get => DateTime.Parse($"{CurrentDateTime.ToShortDateString()} 23:59:59"); }
        /// <summary>
        /// 结束时间的字符串格式
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string EndTimeStr { get => EndTime.ToString("yyyy-MM-dd HH:mm:ss"); }

    }
}
