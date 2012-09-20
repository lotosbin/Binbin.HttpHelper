using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Win32;

namespace Binbin.HttpHelper
{
    public class HttpUtil
    {
        /// <summary>
        /// 根据文件名获取文件类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetContentType(string fileName)
        {
            string contentType = "application/octetstream";
            string ext = Path.GetExtension(fileName).ToLower();
            RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(ext);

            if (registryKey != null && registryKey.GetValue("Content Type") != null)
            {
                contentType = registryKey.GetValue("Content Type").ToString();
            }

            return contentType;
        }

        /// <summary>
        /// 根据query String获取parameter数据
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static List<APIParameter> GetQueryParameters(string queryString)
        {
            if (queryString.StartsWith("?"))
            {
                queryString = queryString.Remove(0, 1);
            }

            List<APIParameter> result = new List<APIParameter>();

            if (!string.IsNullOrEmpty(queryString))
            {
                string[] p = queryString.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && s.IndexOf('=') > -1)
                    {
                        string[] temp = s.Split('=');
                        result.Add(new APIParameter(temp[0], temp[1]));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 把ApiParameter拼成url
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static string GetQueryFromParas(List<APIParameter> paras)
        {
            if (paras == null || paras.Count == 0)
                return "";
            StringBuilder sbList = new StringBuilder();
            int count = 1;
            foreach (APIParameter para in paras)
            {
                sbList.AppendFormat("{0}={1}", para.Name, para.Value);
                if (count < paras.Count)
                {
                    sbList.Append("&");
                }
                count++;
            }
            return sbList.ToString();
        }

        /// <summary>
        /// 把APIParameter中加入URL中
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static string AddParametersToURL(string url, List<APIParameter> paras)
        {
            string querystring = GetQueryFromParas(paras);
            if (querystring != "")
            {
                url += "?" + querystring;
            }
            return url;
        }

        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string MD5Encrpt(string plainText)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            StringBuilder sbList = new StringBuilder();
            foreach (byte d in data)
            {
                sbList.Append(d.ToString("x2"));
            }
            return sbList.ToString();
        }

        /// <summary>
        /// 客户端注册脚本
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="key">同一键名</param>
        /// <param name="script">脚本</param>
        public static void Script(System.Web.UI.Page page, string key, string script)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), key, script, true);
        }

        /// <summary>
        /// 向客户端写入脚本
        /// </summary>
        /// <param name="script">脚本</param>
        /// <param name="isReload">是否刷新本页面</param>
        public static void Script(string script, bool isReload)
        {
            if (isReload)
            {
                script += "document.location=document.location;";
            }
            HttpContext.Current.Response.Write("<script type='text/javascript'>" + script + "</script>");
        }

        /// <summary>
        /// 获取HTTP请求参数   转换失败默认值 default(T)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            return Get<T>(key, default(T));
        }

        /// <summary>
        /// 获取HTTP请求参数
        /// </summary>
        /// <typeparam name="T">需转换的类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">转换失败默认值</param>
        /// <returns></returns>
        public static T Get<T>(string key, T defaultValue)
        {
            string value =  HttpContext.Current.Request[key];
            return Helper.Convert<T>(value, defaultValue);
        }

        /// <summary>
        /// 解析Json
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="key">要获取的Key</param>
        /// <returns>key 对应的 value</returns>
        public static string ParseJson(string source, string key)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            int index = source.IndexOf(key);
            string result = string.Empty;
            if (index != -1)
            {
                result = source.Substring(index + key.Length + 2);
                result = result.Substring(0, result.IndexOf(","));
            }

            return result.Replace("\"", "");
        }

    }
}