using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Nistec.Web.Controls
{
    public class HttpHelper
    {
        public static String GetClientIP()
        {
            var request=HttpContext.Current.Request;
            if (request.IsLocalUrl(request.Url.AbsoluteUri))
            {
                return "";
            }
            if (request.ServerVariables.Count == 0)
                return "";
            String val =
                request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(val))
                val = request.ServerVariables["REMOTE_ADDR"];
            else
                val = val.Split(',').Last().Trim();

            return val;
        }
        public static String GetClientIP(HttpRequestBase Request)
        {
            if(Request.IsLocalUrl(Request.Url.AbsoluteUri))
            {
                return "";
            }

            if (Request.ServerVariables.Count == 0)
                return "";
            String val =
                Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(val))
                val = Request.ServerVariables["REMOTE_ADDR"];
            else
                val = val.Split(',').Last().Trim();

            return val;
        }
        public static String GetClientIP(HttpRequest Request)
        {
            if (Request.ServerVariables.Count == 0)
                return "";
            String val =
                Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(val))
                val = Request.ServerVariables["REMOTE_ADDR"];
            else
                val = val.Split(',').Last().Trim();

            return val;
        }
        public static String GetReferrer()
        {

            var uri = HttpContext.Current.Request.UrlReferrer;
            if (uri != null)
            {
                return uri.AbsoluteUri;
            }
            else if (HttpContext.Current.Request.ServerVariables.Count > 0)
            {

                String val =
                    HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                if (string.IsNullOrEmpty(val))
                    val = HttpContext.Current.Request.ServerVariables["HTTP_HOST"];

                return val;
            }
            return "";
        }
        public static String GetReferrer(HttpRequestBase Request)
        {

            var uri = Request.UrlReferrer;
            if (uri != null)
            {
                return uri.AbsoluteUri;
            }
            else if (Request.ServerVariables.Count > 0)
            {

                String val =
                    Request.ServerVariables["HTTP_REFERER"];

                if (string.IsNullOrEmpty(val))
                    val = Request.ServerVariables["HTTP_HOST"];

                return val;
            }
            return "";
        }
        public static String GetReferrer(HttpRequest Request)
        {

            var uri = Request.UrlReferrer;
            if (uri != null)
            {
                return uri.AbsoluteUri;
            }
            else if (Request.ServerVariables.Count > 0)
            {

                String val =
                    Request.ServerVariables["HTTP_REFERER"];

                if (string.IsNullOrEmpty(val))
                    val = Request.ServerVariables["HTTP_HOST"];

                return val;
            }
            return "";
        }
    }
}
