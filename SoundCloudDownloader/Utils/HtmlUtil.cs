using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoundCloudDownloader.Utils
{
    public static class HtmlUtil
    {
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

        private const int NumberOfRetries = 3;
        private const int DelayOnRetry = 1000;

        public static string GetHtml(string url, WebHeaderCollection headers = null)
        {
            var task = GetHtmlAsync(url, headers);
            task.Wait();

            return task.Result;
        }

        public async static Task<string> GetHtmlAsync(string url, WebHeaderCollection headers = null)
        {
            url = url.Replace(" ", "%20");

            //Exceptions
            if (url.Contains("https://gogoanime.pe/category/hataraku-saibou-2"))
            {
                url = url.Replace("hataraku-saibou-2", "hataraku-saibou");
            }

            string html = "";

            for (int i = 1; i <= NumberOfRetries; ++i)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    if (headers != null)
                    {
                        request.Headers = headers;
                    }

                    HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader streamReader = null;
                    BrotliStream brotli = null;

                    //Headers: "Accept-Encoding" - "gzip, deflate, br"
                    //br (Brotli)
                    if (!string.IsNullOrEmpty(response.ContentEncoding) 
                        && response.ContentEncoding.ToLower().Contains("br"))
                    {
                        //Example: Getting Twist Moe episodes uses "br" encoding ("response" variable above)
                        brotli = new BrotliStream(receiveStream, CompressionMode.Decompress, true);
                        streamReader = new StreamReader(brotli);
                    }
                    else if (string.IsNullOrEmpty(response.CharacterSet))
                    {
                        streamReader = new StreamReader(receiveStream);
                    }
                    else
                    {
                        streamReader = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }

                    html = await streamReader.ReadToEndAsync();
                    streamReader.Close();
                    brotli?.Close();

                    break;
                }
                catch when (i < NumberOfRetries)
                {
                    await Task.Delay(DelayOnRetry);
                }
            }

            return html;
        }

        /*public async static Task<string> GetDecompressedHtmlAsync(string url, WebHeaderCollection headers = null)
        {
            url = url.Replace(" ", "%20");

            //Exceptions
            if (url.Contains("https://gogoanime.pe/category/hataraku-saibou-2"))
            {
                url = url.Replace("hataraku-saibou-2", "hataraku-saibou");
            }

            string responseContent = "";

            for (int i = 1; i <= NumberOfRetries; ++i)
            {
                try
                {
                    using (var httpGet = new HttpClient())
                    {
                        //if (headers != null)
                        //{
                        //    //httpGet.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                        //    for (int j = 0; j < headers.Count; j++)
                        //    {
                        //        httpGet.DefaultRequestHeaders.Add(headers.GetKey(j), headers.Get(j));
                        //    }
                        //}

                        if (headers != null)
                        {
                            foreach (string key in headers.Keys)
                            {
                                var values = headers.GetValues(key);
                                foreach (string value in values)
                                {
                                    httpGet.DefaultRequestHeaders.Add(key, value);
                                }
                            }
                        }
                        
                        var response = await httpGet.GetAsync(url);
                        var stream = await response.Content.ReadAsStreamAsync();

                        try
                        {
                            using (var brotli = new BrotliStream(stream, CompressionMode.Decompress, true))
                            {
                                var streamReader = new StreamReader(brotli);
                                responseContent = streamReader.ReadToEnd();
                            }

                            //var doc = XDocument.Parse(responseContent);
                            //return !response.IsSuccessStatusCode ? $"ERROR: {response.StatusCode} - {link}".Dump() : doc.Descendants("title").FirstOrDefault().Value;
                        }
                        catch
                        {
                            using (StreamReader sr = new StreamReader(stream))
                            {
                                responseContent = await sr.ReadToEndAsync();
                            }
                        }
                    }

                    break;
                }
                catch when (i < NumberOfRetries)
                {
                    await Task.Delay(DelayOnRetry);
                }
            }

            return responseContent;
        }*/

        /*public static int GetUrlFileSize(string url)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    // Build and set timeout values for the request.
                    Java.Net.URLConnection connection = new Java.Net.URL(url).OpenConnection();
                    //Java.Net.URL urlCon = new Java.Net.URL(url);
                    //Java.Net.URLConnection connection = urlCon.OpenConnection();
                    connection.ConnectTimeout = 5000;
                    connection.ReadTimeout = 5000;
                    
                    //var httpURLConnection = (connection as Java.Net.HttpURLConnection);
                    //var test = httpURLConnection as Javax.Net.Ssl.HttpsURLConnection;
                    //
                    //test.SSLSocketFactory = Android.Net.SSLCertificateSocketFactory.GetInsecure(0, null);
                    //test.HostnameVerifier = new Org.Apache.Http.Conn.Ssl.AllowAllHostnameVerifier();

                    //if (connection is HttpsURLConnection) 
                    //{
                    //    HttpsURLConnection httpsConn = (HttpsURLConnection) conn;
                    //    httpsConn.setSSLSocketFactory(SSLCertificateSocketFactory.getInsecure(0, null));
                    //    httpsConn.setHostnameVerifier(new AllowAllHostnameVerifier());
                    //}

                    connection.Connect();

                    int file_size = connection.ContentLength;

                    return file_size;
                }
                catch (Exception e)
                {
                    return 0;
                }
            });

            task.Wait();

            return task.Result;
        }*/

        public static async Task<long> GetFileSizeFromUrl(string url)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
            request.Method = "HEAD";
            using (var response = await request.GetResponseAsync())
            {
                var length = response.ContentLength;

                return length;
            }
        }

        public static string RemoveBadChars1(this string name)
        {
            //string result = new string(name.Where(c => char.IsLetter(c) || c == '\'').ToArray());
            string result = new string(name.Where(c => char.IsLetter(c) || c == ' ').ToArray());
            return result.Replace(' ', '-');
        }

        public static string RemoveBadChars(string word)
        {
            //Regex reg = new Regex("[^a-zA-Z']");
            Regex reg = new Regex("[^a-zA-Z' ]"); //Don't replace spaces
            string regString = reg.Replace(word, string.Empty);

            return regString.Replace(' ', '-');
        }

        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            var sourceProps = typeof(T).GetProperties()
                .Where(x => x.CanRead)
                .ToList();

            var destProps = typeof(TU).GetProperties()
                .Where(x => x.CanWrite)
                .ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (destProps.Any(x => x.Name == sourceProp.Name))
                {
                    var p = destProps.First(x => x.Name == sourceProp.Name);
                    if (p.CanWrite)
                    {
                        // check if the property can be set or no.
                        p.SetValue(dest, sourceProp.GetValue(source, null), null);
                    }
                }
            }
        }
    }

    public static class StringExtensions
    {
        //? - any character (one and only one)
        //* - any characters (zero or more)
        public static string WildCardToRegular(string value)
        {
            // If you want to implement both "*" and "?"
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";

            // If you want to implement "*" only
            //return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }

        static void Test()
        {
            string test = "Some Data X";

            bool endsWithEx = Regex.IsMatch(test, WildCardToRegular("*X"));
            bool startsWithS = Regex.IsMatch(test, WildCardToRegular("S*"));
            bool containsD = Regex.IsMatch(test, WildCardToRegular("*D*"));

            // Starts with S, ends with X, contains "me" and "a" (in that order) 
            bool complex = Regex.IsMatch(test, WildCardToRegular("S*me*a*X"));
        }
    }

    /// <summary>
    /// Contains approximate string matching
    /// </summary>
    public static class LevenshteinDistance
    {
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}