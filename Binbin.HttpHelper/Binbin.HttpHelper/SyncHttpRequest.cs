using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Binbin.HttpHelper
{
    public class SyncHttpRequest
    {
        /// <summary>
        /// ͬ����ʽ����http get����
        /// </summary>
        /// <param name="url">����URL</param>
        /// <param name="queryString">�����ַ���</param>
        /// <returns>���󷵻�ֵ</returns>
        public string HttpGet(string url, string queryString)
        {
            string responseData = null;

            if (!string.IsNullOrEmpty(queryString))
            {
                url += "?" + queryString;
            }

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;



            try
            {
                using (var responseStream = webRequest.GetResponse().GetResponseStream())
                {
                    using (var responseReader = new StreamReader(responseStream))
                    {
                        responseData = responseReader.ReadToEnd();
                    }
                }
            }
            catch
            {
            }


            return responseData;
        }

        /// <summary>
        /// ͬ����ʽ����http get����
        /// </summary>
        /// <param name="url">����URL</param>
        /// <param name="paras">��������б�</param>
        /// <returns>���󷵻�ֵ</returns>
        public string HttpGet(string url, List<APIParameter> paras)
        {
            string querystring = HttpUtil.GetQueryFromParas(paras);
            return HttpGet(url, querystring);
        }

        /// <summary>
        /// ͬ����ʽ����http post����
        /// </summary>
        /// <param name="url">����URL</param>
        /// <param name="queryString">�����ַ���</param>
        /// <returns>���󷵻�ֵ</returns>
        public string HttpPost(string url, string queryString)
        {
            string responseData = null;

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;

            try
            {
                //POST the data.
                using (var requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(queryString);
                }

                using (var responseStream = webRequest.GetResponse().GetResponseStream())
                {
                    using (var responseReader = new StreamReader(responseStream))
                    {
                        responseData = responseReader.ReadToEnd();
                    }
                }
            }
            catch
            {
            }


            return responseData;
        }

        /// <summary>
        /// ͬ����ʽ����http post����
        /// </summary>
        /// <param name="url">����URL</param>
        /// <param name="paras">��������б�</param>
        /// <returns>���󷵻�ֵ</returns>
        public string HttpPost(string url, List<APIParameter> paras)
        {
            string querystring = HttpUtil.GetQueryFromParas(paras);
            return HttpPost(url, querystring);
        }


        /// <summary>
        /// ͬ����ʽ����http post���󣬿���ͬʱ�ϴ��ļ�
        /// </summary>
        /// <param name="url">����URL</param>
        /// <param name="queryString">��������ַ���</param>
        /// <param name="files">�ϴ��ļ��б�</param>
        /// <returns>���󷵻�ֵ</returns>
        public string HttpPostWithFile(string url, string queryString, List<APIParameter> files)
        {
            string responseData;
            string boundary = DateTime.Now.Ticks.ToString("x");

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            byte[] tempBuffer;
            using (Stream memStream = new MemoryStream())
            {
                byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                List<APIParameter> listParams = HttpUtil.GetQueryParameters(queryString);

                foreach (APIParameter param in listParams)
                {
                    string formitem = string.Format(formdataTemplate, param.Name, param.Value);
                    byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }

                memStream.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: \"{2}\"\r\n\r\n";

                foreach (APIParameter param in files)
                {
                    string name = param.Name;
                    string filePath = param.Value;
                    string file = Path.GetFileName(filePath);
                    string contentType = HttpUtil.GetContentType(file);

                    string header = string.Format(headerTemplate, name, file, contentType);
                    byte[] headerbytes = Encoding.UTF8.GetBytes(header);

                    memStream.Write(headerbytes, 0, headerbytes.Length);

                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = 0;

                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            memStream.Write(buffer, 0, bytesRead);
                        }
                        memStream.Write(boundarybytes, 0, boundarybytes.Length);
                    }
                }

                webRequest.ContentLength = memStream.Length;
                memStream.Position = 0;

                tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
            }
            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            }

            using (Stream responseStream = webRequest.GetResponse().GetResponseStream())
            {
                using (var responseReader = new StreamReader(responseStream))
                {
                    responseData = responseReader.ReadToEnd();
                }
            }

            return responseData;
        }

        /// <summary>
        /// ͬ����ʽ����http post���󣬿���ͬʱ�ϴ��ļ�
        /// </summary>
        /// <param name="url">����URL</param>
        /// <param name="paras">��������б�</param>
        /// <param name="files">�ϴ��ļ��б�</param>
        /// <returns>���󷵻�ֵ</returns>
        public string HttpPostWithFile(string url, List<APIParameter> paras, List<APIParameter> files)
        {
            string querystring = HttpUtil.GetQueryFromParas(paras);
            return HttpPostWithFile(url, querystring, files);
        }

    }
}