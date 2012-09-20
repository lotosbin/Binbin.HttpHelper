using System;
using System.Text.RegularExpressions;

namespace Binbin.HttpHelper
{
    public static class Helper
    {
        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="o">待转换数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T Convert<T>(object o, T defaultValue)
        {
            if (o == null || o == DBNull.Value)
            {
                return defaultValue;
            }
            return (T)System.Convert.ChangeType(o, typeof(T));
        }

        /// <summary>
        /// 转换数据类型 默认值:default(T)
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="o">待转换数据</param>
        /// <returns></returns>
        public static T Convert<T>(object o)
        {
            return Convert(o, default(T));
        }

        /// <summary>
        /// 截段字符串
        /// </summary>
        /// <param name="obj">目标数据</param>
        /// <param name="size">需要的长度</param>
        /// <param name="defaultValue">为空时的值 </param>
        /// <returns></returns>
        public static string SnipContent(object obj, int size, string defaultValue)
        {
            size -= 3;
            string result = defaultValue;

            if (obj == null)
                return result;

            result = obj.ToString();

            result = Regex.Replace(result, "<[^>]*>", "");

            if (result.Length > size)
            {
                result = result.Substring(0, size) + "...";
            }
            return result;
        }

        /// <summary>
        /// 是否全都是空
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool NullEmpty<T>(params  T[] list)
        {
            bool result = true;
            foreach (T item in list)
            {
                if (typeof(string) == typeof(T))
                {
                    if (!string.IsNullOrEmpty(System.Convert.ToString(item).Trim()))
                    {
                        result = false;
                    }
                }
                else if (item != null)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 用正则检查字符串类型
        /// </summary>
        /// <param name="sourceInfo">要检测的字符串</param>
        /// <returns>number  mail  ip  url  未匹配到返回string.Empty</returns>
        public static string RegCheckInfo(string sourceInfo)
        {
            if (string.IsNullOrEmpty(sourceInfo))
            {
                return string.Empty;
            }

            string numberPattern = @"^[0-9]*$";
            string mailPattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
            string ipPattern = @"\d+.\d+.\d+.\d+";
            string urlPattern = @"(http://)?[0-9a-zA-Z]+\.+";
            int index = -1;
            string result = string.Empty;

            string[] patternArray = { numberPattern, mailPattern, ipPattern, urlPattern };


            for (int i = 0; i < patternArray.Length; i++)
            {
                if (Regex.IsMatch(sourceInfo, patternArray[i]))
                {
                    index = i + 1;
                    break;
                }
            }

            switch (index)
            {
                case 1:
                    result = "number";
                    break;
                case 2:
                    result = "mail";
                    break;
                case 3:
                    result = "ip";
                    break;
                case 4:
                    result = "url";
                    break;
            }

            return result;
        }

    }
}