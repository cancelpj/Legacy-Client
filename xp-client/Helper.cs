using System;
using System.IO;
using System.Net;

namespace xp_client
{
    public static class Helper
    {
        public static string HttpPost(string url, string data)
        {
            //try
            //{
                var request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var response = request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            //}
            //catch (Exception e)
            //{
            //    return e.Message;
            //}
        }
    }
}