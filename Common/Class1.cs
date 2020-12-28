using Nistec.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Xml;

namespace System.Web.Http
    {

    /// <summary>
    /// Extends the HttpRequestMessage collection
    /// </summary>
    public static class HttpRequestMessageExtensions
    {

        public static string GetHostAddress(this HttpRequestMessage request)
        {

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                var requestBase = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request;//.UserHostAddress;

                if (requestBase.IsLocal)
                    return "127.0.0.1";
                else
                    return requestBase.UserHostAddress;
            }
            //else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            //{
            //    RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
            //    return prop.Address;
            //}
            else if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                if (HttpContext.Current.Request.IsLocal)
                    return "127.0.0.1";
                else
                    return HttpContext.Current.Request.UserHostAddress;

                //return GetClientIp(HttpContext.Current.Request);
            }
            else
            {
                return null;
            }
        }

        public static IDictionary<string, object> ParseRequest(this HttpRequestMessage request)
        {
            string jsonRequest = request.Content.ReadAsStringAsync().Result;
            //Netlog.InfoFormat("{0} request:{1}", action, jsonRequest);
            //TraceHelper.Log("co", "NetcellOtp", action, jsonRequest, LogType.Info);
            return Nistec.Serialization.JsonSerializer.ToDictionary(jsonRequest);
        }
        public static T ParseRequest<T>(this HttpRequestMessage request)
        {
            string jsonRequest = request.Content.ReadAsStringAsync().Result;
            //Netlog.InfoFormat("{0} request:{1}", action, jsonRequest);
            //TraceHelper.Log("co", "NetcellOtp", action, jsonRequest, LogType.Info);
            return Nistec.Serialization.JsonSerializer.Deserialize<T>(jsonRequest);
        }


        public static NameValueCollection GetQueryStrings(this HttpRequestMessage request)
        {
            if (!string.IsNullOrEmpty(request.RequestUri.Query))
                return HttpUtility.ParseQueryString(request.RequestUri.Query);
            return null;

        }

        public static string GetHeader(this HttpRequestMessage request, string key)
        {
            IEnumerable<string> keys = null;
            if (!request.Headers.TryGetValues(key, out keys))
                return null;

            return keys.First();
        }

        ///// <summary>
        ///// Retrieves an individual cookie from the cookies collection
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="cookieName"></param>
        ///// <returns></returns>
        //public static string GetCookie(this HttpRequestMessage request, string cookieName)
        //{
        //    var cs = CookieContainer.GetCookies(request.RequestUri);
        //    foreach (Cookie c in cs)
        //    {
        //        if (c.Name == cookieName)
        //        {
        //            return c.Value;
        //        }
        //    }
        //    return null;
        //}

    }

    public class HttpSender
    {

        public static int TimeoutSeconds = 10;

        //public static ApiResult Send(string url, string method, string request, int timeoutSeconds = 10)
        //{
        //    TimeoutSeconds = timeoutSeconds;
        //    string formatType = "json";
        //    CancellationToken cancelToken;
        //    try
        //    {
        //        if (url == null || url.Length == 0)
        //            throw new ArgumentException("Invalid Url!");
        //        else if (request == null || request.Length == 0)
        //            throw new ArgumentException("Invalid Request!");

        //        HttpMethod httpMethod = GetMethod(method);
        //        string contentType = "application/json";
        //        Task<string> response = null;
        //        if (httpMethod == HttpMethod.Get)
        //            response = DoJsonGetRequest(url);
        //        else //if (httpMethod == HttpMethod.Post)
        //            response = HttpClientUtil.DoHttpRequestAsync(url, httpMethod, contentType, request, 120); //DoHttpRequest(url, httpMethod, contentType, request);

        //        if (response == null)
        //            throw new Exception("No response from server!");

        //        return ApiResult.Parse(response.Result);
        //        //{

        //        //    Response = PrintJson(response.Result),
        //        //    Status = 0
        //        //};
        //    }
        //    catch (AggregateException ex)
        //    {
        //        return ApiResult.Error(-1, FormatAggrigateException(ex, cancelToken));
        //    }
        //    catch (Exception ex)
        //    {
        //        return ApiResult.Error(-1, "Send messsage error: " + FormatException(ex, formatType));
        //    }

        //}

        #region helpers

        static HttpMethod GetMethod(string method)
        {
            switch (method.ToUpper())
            {
                case "GET":
                    return HttpMethod.Get;
                case "POST":
                default:
                    return HttpMethod.Post;

            }
        }
        static string FormatException(Exception ex, string format)
        {
            if (ex == null)
                return "";
            if (ex.InnerException != null)
                return ex.Message + " Inner: " + ex.InnerException;
            if (format == "Xml")
                return PrintXML(ex.Message);
            return ex.Message;
        }

        static string FormatAggrigateException(AggregateException ex, CancellationToken cancelToken)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Send messsage error: " + ex.Message);
            foreach (var inner in ex.InnerExceptions)
            {

                if (inner is TaskCanceledException)
                {
                    if (cancelToken.IsCancellationRequested && ((TaskCanceledException)inner).CancellationToken == cancelToken)
                    {
                        // a real cancellation, triggered by the caller
                        sb.AppendLine("Send messsage canceled by user");
                    }
                    sb.AppendLine(inner.Source + ":" + inner.Message + " (possibly request timeout)");
                    continue;
                }
                // a web request timeout (possibly other things!?)
                if (inner.InnerException != null)
                    sb.AppendLine(inner.InnerException.Source + ":" + inner.InnerException.Message);

                sb.AppendLine(inner.Source + ":" + inner.Message);
            }
            return sb.ToString();
        }

        #endregion

        #region http request methods
        static Task<string> DoHttpRequest(string address, HttpMethod method, string contentType, string data)
        {
            using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(TimeoutSeconds) })
            using (var request = new HttpRequestMessage(method, address))
            using (request.Content = new StringContent(data, Encoding.UTF8, contentType))
            using (var response = httpClient.SendAsync(request))
            {

                return response.Result.Content.ReadAsStringAsync();
            }
        }

        static Task<string> DoHttpRequest(string address, HttpMethod method, string contentType, string data, CancellationToken ctsTocken)
        {
            using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(TimeoutSeconds) })
            using (var request = new HttpRequestMessage(method, address))
            using (request.Content = new StringContent(data, Encoding.UTF8, contentType))
            using (var response = httpClient.SendAsync(request, ctsTocken))
            {

                return response.Result.Content.ReadAsStringAsync();
            }
        }

        /*
         * support for framework 4.5
        static async Task<string> DoHttpRequestAsync(string address, HttpMethod method, string contentType, string data)
        {
            using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(TimeoutSeconds) })
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSeconds)))
            using (var request = new HttpRequestMessage(method, address))
            using (request.Content = new StringContent(data, Encoding.UTF8, contentType))
            using (var response = await httpClient.SendAsync(request, cts.Token))
            {

                return await response.Content.ReadAsStringAsync();
            }
        }

        static async Task<string> DoJsonPostRequest(string address, string data)
        {
            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(20) })
            using (var content = new StringContent(data, Encoding.UTF8))
            using (HttpResponseMessage response = await client.PostAsync(address, content))
            using (HttpContent result = response.Content)
            {
                return await result.ReadAsStringAsync();
            }

        }

        static async Task<string> DoJsonGetRequest(string address)
        {
            using (HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(20) })
            using (HttpResponseMessage response = await client.GetAsync(address))
            using (HttpContent content = response.Content)
            {
                return await content.ReadAsStringAsync();
            }
        }
        */
        static string DoSoapRequest(string url, string soapAction, string method, string contentType, string soapBody)
        {
            string result = null;

            try
            {
                //Create HttpWebRequest
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = method;
                request.ContentType = contentType + "; charset=utf-8";//"text/xml; charset=utf-8";
                request.Timeout = (int)TimeSpan.FromSeconds(TimeoutSeconds).TotalMilliseconds;
                request.KeepAlive = false;
                request.UseDefaultCredentials = true;
                request.Headers["SOAPAction"] = soapAction;

                byte[] bytes = Encoding.UTF8.GetBytes(soapBody);
                request.ContentLength = bytes.Length;

                //Create request stream
                using (Stream OutputStream = request.GetRequestStream())
                {
                    if (!OutputStream.CanWrite)
                    {
                        throw new Exception("Could not wirte to RequestStream");
                    }
                    OutputStream.Write(bytes, 0, bytes.Length);
                }

                //Get response stream
                using (WebResponse resp = request.GetResponse())
                {
                    using (Stream ResponseStream = resp.GetResponseStream())
                    {
                        using (StreamReader readStream =
                                new StreamReader(ResponseStream, Encoding.UTF8))
                        {
                            result = readStream.ReadToEnd();
                        }
                    }
                }

                //result = SoapRequest(url, soapAction, soapBody);
            }
            catch (WebException wex)
            {
                result = "Error: " + wex.Message;
            }
            catch (Exception ex)
            {
                result = "Error: " + ex.Message;
            }
            return result;
        }


        static string DoRequest(string url, string action, string method, string contentType, string postArgs)
        {

            string response = null;

            WebRequest request = null;
            Stream newStream = null;
            Stream receiveStream = null;
            StreamReader readStream = null;
            WebResponse wresponse = null;
            string encoding = "utf-8";
            int timeout = (int)TimeSpan.FromSeconds(TimeoutSeconds).TotalMilliseconds;

            try
            {
                Encoding enc = Encoding.GetEncoding(encoding);
                string[] args = postArgs.Replace("\r\n", "").Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder sb = new StringBuilder();
                int counter = 0;
                foreach (string s in args)
                {
                    string[] arg = s.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                    if (counter > 0)
                        sb.Append("&");
                    sb.Append(arg[0].Trim() + "=" + HttpUtility.UrlEncode(arg[1].Trim()));

                    counter++;
                }

                string postData = sb.ToString();


                if (method.ToUpper() == "GET")
                {
                    string qs = string.IsNullOrEmpty(postData) ? "" : "?" + postData;

                    request = WebRequest.Create(url + qs);
                    request.Timeout = timeout <= 0 ? 100000 : timeout;
                    if (!string.IsNullOrEmpty(contentType))
                        request.ContentType = contentType;// string.IsNullOrEmpty(contentType) ? "application/x-www-form-urlencoded" : contentType;

                }
                else
                {
                    request = WebRequest.Create(url);
                    request.Method = "POST";
                    request.Credentials = CredentialCache.DefaultCredentials;

                    request.Timeout = timeout <= 0 ? 100000 : timeout;
                    request.ContentType = string.IsNullOrEmpty(contentType) ? "application/x-www-form-urlencoded" : contentType;

                    byte[] byteArray = enc.GetBytes(postData);
                    request.ContentLength = byteArray.Length;

                    newStream = request.GetRequestStream();
                    newStream.Write(byteArray, 0, byteArray.Length);
                    newStream.Close();

                }


                // Get the response.
                wresponse = request.GetResponse();
                receiveStream = wresponse.GetResponseStream();
                readStream = new StreamReader(receiveStream, enc);
                response = readStream.ReadToEnd();

                return response;
            }
            catch (System.Net.WebException webExcp)
            {
                throw webExcp;
            }
            catch (System.IO.IOException ioe)
            {
                throw ioe;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (newStream != null)
                    newStream.Close();
                if (receiveStream != null)
                    receiveStream.Close();
                if (readStream != null)
                    readStream.Close();
            }
        }

        #endregion

        #region formatter

        internal static string Indent = "   ";

        internal static void AppendIndent(StringBuilder sb, int count)
        {
            for (; count > 0; --count) sb.Append(Indent);
        }

        internal static string PrintJson(string input)
        {
            var output = new StringBuilder();
            int depth = 0;
            int len = input.Length;
            char[] chars = input.ToCharArray();
            for (int i = 0; i < len; ++i)
            {
                char ch = chars[i];

                if (ch == '\"') // found string span
                {
                    bool str = true;
                    while (str)
                    {
                        output.Append(ch);
                        ch = chars[++i];
                        if (ch == '\\')
                        {
                            output.Append(ch);
                            ch = chars[++i];
                        }
                        else if (ch == '\"')
                            str = false;
                    }
                }

                switch (ch)
                {
                    case '{':
                    case '[':
                        output.Append(ch);
                        output.AppendLine();
                        AppendIndent(output, ++depth);
                        break;
                    case '}':
                    case ']':
                        output.AppendLine();
                        AppendIndent(output, --depth);
                        output.Append(ch);
                        break;
                    case ',':
                        output.Append(ch);
                        output.AppendLine();
                        AppendIndent(output, depth);
                        break;
                    case ':':
                        output.Append(" : ");
                        break;
                    default:
                        if (!char.IsWhiteSpace(ch))
                            output.Append(ch);
                        break;
                }
            }

            return output.ToString();
        }

        public static String PrintXML(String XML)
        {

            XmlDocument document = new XmlDocument();

            try
            {
                document.LoadXml(XML);
                using (MemoryStream mStream = new MemoryStream())
                using (XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode))
                {
                    writer.Formatting = Formatting.Indented;

                    // Write the XML into a formatting XmlTextWriter
                    document.WriteContentTo(writer);
                    writer.Flush();
                    mStream.Flush();

                    // Have to rewind the MemoryStream in order to read
                    // its contents.
                    mStream.Position = 0;

                    // Read MemoryStream contents into a StreamReader.
                    StreamReader sReader = new StreamReader(mStream);

                    // Extract the text from the StreamReader.
                    String FormattedXML = sReader.ReadToEnd();

                    return FormattedXML;
                }
            }
            catch (Exception)
            {
                return XML;
            }

        }

        #endregion
    }

    /*
   public ActionResult BreadCrumb()
   {
    Uri referrer = Request.UrlReferrer;
    if (referrer != null && referrer.IsRouteMatch("Catalog", "Category"))
    {
     string categoryName = referrer.GetRouteParameterValue("categoryName");
    }
   }
   */

    public static class UriExtensions
    {
        public static bool IsRouteMatch(this Uri uri, string controllerName, string actionName)
        {
            RouteInfo routeInfo = new RouteInfo(uri, HttpContext.Current.Request.ApplicationPath);
            return (routeInfo.RouteData.Values["controller"].ToString() == controllerName && routeInfo.RouteData.Values["action"].ToString() == actionName);
        }

        public static string GetRouteParameterValue(this Uri uri, string parameterName)
        {
            var routeInfo = new RouteInfo(uri, HttpContext.Current.Request.ApplicationPath);
            return routeInfo.RouteData.Values[parameterName] != null ? routeInfo.RouteData.Values[parameterName].ToString() : null;
        }
    }

    public class RouteInfo
    {
        public RouteInfo(Uri uri, string applicaionPath)
        {
            RouteData = RouteTable.Routes.GetRouteData(new InternalHttpContext(uri, applicaionPath));
        }

        public RouteData RouteData { get; private set; }

        private class InternalHttpContext : HttpContextBase
        {
            private readonly HttpRequestBase _request;

            public InternalHttpContext(Uri uri, string applicationPath)
            {
                _request = new InternalRequestContext(uri, applicationPath);
            }

            public override HttpRequestBase Request
            {
                get { return _request; }
            }
        }

        private class InternalRequestContext : HttpRequestBase
        {
            private readonly string _appRelativePath;
            private readonly string _pathInfo;

            public InternalRequestContext(Uri uri, string applicationPath)
            {
                _pathInfo = "";
                if (string.IsNullOrEmpty(applicationPath) || !uri.AbsolutePath.StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase))
                {
                    _appRelativePath = uri.AbsolutePath.Substring(applicationPath.Length);
                }
                else
                {
                    _appRelativePath = uri.AbsolutePath;
                }
            }

            public override string AppRelativeCurrentExecutionFilePath
            {
                get { return string.Concat("~", _appRelativePath); }
            }

            public override string PathInfo
            {
                get { return _pathInfo; }
            }
        }
    }
}

