using SoundCloudDownloader.Utils;
using System.IO;
using System.Net;

namespace SoundCloudDl.Methods
{
    class Http
    {
        /*public string GetHttpResponseString(string uri, string parameterString)
        {
            var webRequest = WebRequest.Create(uri + parameterString);
            return PrepareHttpResponseString(webRequest);
        }

        public string GetHttpResponseString(string uri)
        {
            var webRequest = WebRequest.Create(uri);
            return PrepareHttpResponseString(webRequest);
        }

        private string PrepareHttpResponseString(WebRequest webRequest)
        {
            var webResponseStream = webRequest.GetResponse().GetResponseStream();
            var webResponseStreamReader = new StreamReader(webResponseStream);
            return webResponseStreamReader.ReadToEnd();
        }*/

        public string GetHttpResponseString(string uri, string parameterString)
        {
            string html = HtmlUtil.GetHtml(uri + parameterString);
            return html;
        }

        public string GetHttpResponseString(string uri)
        {
            string html = HtmlUtil.GetHtml(uri);
            return html;
        }
    }
}
