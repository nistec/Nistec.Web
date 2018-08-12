using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Nistec.Web.Asp
{
    internal class StringUtil //a14
    {
        private static string AttrPattern = "(?<attr>{0})\\s*=\\s*('(?<value>[^']*)'|\"(?<value>[^\"]*)\"|(?<value>[^\\s=>])*)";
        private Uri _uri;
        private static char[] chars = new char[] { 
            '%', '<', '>', '!', '"', '#', '$', '&', '\'', '(', ')', ',', ':', ';', '=', '?', 
            '[', '\\', ']', '^', '`', '{', '|', '}', '~', '+'
         };
        private static byte[] bytes = Encoding.ASCII.GetBytes(chars);//_a_3
        internal static readonly string startScriptTag = "\r\n<script type=\"text/javascript\">\r\n//<![CDATA[\r\n";//a15
        internal static readonly string endScriptTag = "//]]>\r\n</script>\r\n";//a16
        public static string EMPTY_STATUS = "<status empty='true'></status>";

        internal StringUtil(HttpContext c)
        {
            if (c != null)
            {
                this._uri = c.Request.Url;
            }
        }

        private static string FormatCulture(Match m)
        {
            return m.ToString().ToLower(new CultureInfo("en-US", true));
        }

        private static string a_11(Match m)
        {
            string pattern = "=([^\",^\\s,.]*)[\\s]";
            string input = Regex.Replace(m.ToString(), pattern, new MatchEvaluator(StringUtil.a_12));
            pattern = "\\s[^=\"]*=";
            input = Regex.Replace(input, pattern, new MatchEvaluator(StringUtil.FormatCulture));
            pattern = "=([^\",^\\s,.]*)[>]";
            return Regex.Replace(input, pattern, new MatchEvaluator(StringUtil.a_13));
        }

        private static string a_12(Match m)
        {
            string str = m.ToString().Remove(0, 1).Trim();
            return ("=\"" + str + "\" ");
        }

        private static string a_13(Match m)
        {
            string str = m.ToString().Remove(0, 1);
            str = str.Remove(str.Length - 1, 1);
            return ("=\"" + str + "\">");
        }

        private static string FormtAttrib(Match m)
        {
            string str = m.ToString().Remove(0, 1).Trim();
            return ("=\"" + str + "\"");
        }


        private string FormatSrc(Match me)
        {
            string pattern = string.Format(AttrPattern, "(href)|(src)");
            return Regex.Replace(me.ToString(), pattern, new MatchEvaluator(this.FormatUri), RegexOptions.IgnoreCase);
        }

        private string FormatUri(Match me)
        {
            string relativeUri = me.Groups["value"].Value;
            string str2 = me.Groups["attr"].Value;
            if (this._uri != null)
            {
                Uri uri = new Uri(this._uri, relativeUri);
                relativeUri = uri.AbsoluteUri;
            }
            return string.Format("{0}='{1}'", str2, relativeUri);
        }

        internal static string ToHexRvs(string m)
        {
            if ((m != null) && (m.Length > 0))
            {
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    m = m.Replace("%" + bytes[i].ToString("x"), chars[i].ToString());
                }
            }
            return m;
        }

        internal static string ToHex(string m)
        {
            if ((m != null) && (m.Length > 0))
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    m = m.Replace(chars[i].ToString(), "%" + bytes[i].ToString("x"));
                }
            }
            return m;
        }
        //ToMinus
        internal static string ToJsCh(string m)
        {
            if ((m != null) && (m.Length > 0))
            {
                m = m.Replace("+", "-");
                m = m.Replace("/", "_");
            }
            return m;
        }
        //ToPlus
        internal static string FromJsCh(string m)
        {
            if ((m != null) && (m.Length > 0))
            {
                m = m.Replace("-", "+");
                m = m.Replace("_", "/");
            }
            return m;
        }

        internal static string ToBase64String(string s)
        {
            if (IsNullOrEmptyString(s))
            {
                return s;
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        }

        internal static bool IsNullOrEmptyString(string s)//a22
        {
            if ((s != null) && (s.Length != 0))
            {
                return false;
            }
            return true;
        }

        internal static string FromBase64String(string s)//a23
        {
            if (IsNullOrEmptyString(s))
            {
                return s;
            }
            return Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }

        internal static string GetPhysicalPath(string a_6, bool addSlash)//a24
        {
            return GetPath(a_6, addSlash, true);
        }

        internal static string GetVirtualPath(string a_6, bool addSlash)//a25
        {
            return GetPath(a_6, addSlash, false);
        }
        internal static string GetVirtualDirectory(string path, bool addSlash)//a25
        {
            string file = System.IO.Path.GetFileName(path);
            path = path.Replace(file, "");

            string tpath = path.Replace("\\", "/");
            if (addSlash && !tpath.EndsWith("/"))
                tpath += "/";
            return tpath;
        }



        internal static string FormatJS(string m)//a26
        {
            string str = "X";
            Regex regex = new Regex(@"(<[^><]*\son[A-Za-z]+)(\s*[=])", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (Match match in regex.Matches(m))
            {
                m = m.Replace(match.Value, match.Groups[1].Value + str + match.Groups[2].Value);
            }
            regex = new Regex("(<\\s*a[^><]*href)(\\s*=\\s*[\"|']?\\s*javascript)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (Match match2 in regex.Matches(m))
            {
                m = m.Replace(match2.Value, match2.Groups[1].Value + str + match2.Groups[2].Value);
            }
            regex = new Regex(@"<script[^>]*>((.)|(\s))*?</script>", RegexOptions.IgnoreCase);
            return regex.Replace(m, "");
        }

        internal static string Formar7bit(string m)//a27
        {
            if (m == null)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            char[] chArray = m.ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                int num2 = Convert.ToInt32(chArray[i]);
                switch (num2)
                {
                    case 13:
                        builder.Append("\r");
                        break;

                    case 10:
                        builder.Append("\n");
                        break;

                    default:
                        if ((num2 < 0x20) || ((num2 > 0x7e) && (num2 != 0xa6)))
                        {
                            builder.Append("&#" + num2.ToString() + ";");
                        }
                        else
                        {
                            builder.Append(chArray[i].ToString());
                        }
                        break;
                }
            }
            return builder.ToString();
        }

        internal static string FormatPlainText(string m)//a28
        {
            m = Regex.Replace(m, "[ \n]+", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "<script[^>]*>.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "<br[^>]*>", "\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "<p [^>]*>", "\n\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "</p [^>]*>", "\n\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "<p>", "\n\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "</p>", "\n\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "<hr>", "\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "<!--.*?-->", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "</h2>", "\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "</h3>", "\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "</h4>", "\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = Regex.Replace(m, "<[^>]*>", "");
            m = Regex.Replace(m, "\n ", "\n");
            return HttpContext.Current.Server.HtmlDecode(m);

        }

        /*
        internal static string FormatXhtml(string html, bool matchEntities, bool cleanWord )
        {
            if ((html == null) || (html.Length == 0))
            {
                return "";
            }

            return Xhtml.XUtil.FormatXhtml(html, matchEntities, cleanWord );


            string pattern = @"<[A-Za-z0-9:]*[>,\s]";
            html = Regex.Replace(html, pattern, new MatchEvaluator(StringUtil.FormatCulture));
            pattern = "</[A-Za-z0-9]*>";
            html = Regex.Replace(html, pattern, new MatchEvaluator(StringUtil.FormatCulture));
            pattern = "<[a-zA-Z]+[\\w:\\.]*(\\s+[a-zA-Z]+[\\w:\\.]*((=[^\\s\"'<>]+)|(=\"[^\"]*\")|(='[^']*')|()))*\\s*\\/?\\s*>";
            html = Regex.Replace(html, pattern, new MatchEvaluator(StringUtil.a_11));
            //pattern = "=[^\",\'][A-Za-z0-9\u0590-\u05FF,.]*";
            //html = Regex.Replace(html, pattern, new MatchEvaluator(StringUtil.FormtAttrib));

            html = html.Replace("<br>", "<br />");
            return html.Replace("<hr>", "<hr />");
        }
         */

        internal string ReplaceUrl(string m)//a30
        {
            if (this._uri == null)
            {
                return m;
            }
            int port = this._uri.Port;
            string host = this._uri.Host;
            if (port == 80)
            {
                m = Regex.Replace(m, "http://" + host, "", RegexOptions.IgnoreCase);
                m = Regex.Replace(m, "http://www." + host, "", RegexOptions.IgnoreCase);
                return Regex.Replace(m, "https://" + host, "", RegexOptions.IgnoreCase);
            }
            m = Regex.Replace(m, string.Concat(new object[] { "http://", host, ":", port }), "", RegexOptions.IgnoreCase);
            object[] objArray = new object[] { "https://", host, ":", port };
            return Regex.Replace(m, string.Concat(objArray), "", RegexOptions.IgnoreCase);
        }

        internal string FormatElementSrc(string m)//a31
        {
            if (this._uri != null)
            {
                string pattern = string.Format(@"<((embed)|(img)|(script)|(a)|(link))\s+.*?{0}", string.Format(AttrPattern, "(href)|(src)"));
                return Regex.Replace(m, pattern, new MatchEvaluator(this.FormatSrc), RegexOptions.IgnoreCase);
            }
            return m;
        }

        //a32
        internal static bool IsArrayContains(string str, string[] param)
        {
            if ((!IsNullOrEmptyString(str) && (param != null)) && (param.Length != 0))
            {
                str = str.ToLower();
                int length = param.Length;
                for (int i = 0; i < length; i++)
                {
                    string s = param[i];
                    if (!IsNullOrEmptyString(s))
                    {
                        s = s.ToLower();
                        if (str.IndexOf(s) == 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal static bool a33(string a_18, string a_19, bool ignoreCase)
        {
            if ((IsNullOrEmptyString(a_18) || IsNullOrEmptyString(a_19)) || (a_19.Length > a_18.Length))
            {
                return false;
            }
            a_18 = a_18.Substring(a_18.Length - a_19.Length, a_19.Length);
            return IsEqual(a_18, a_19, ignoreCase);
        }

        internal static bool IsEqual(string a_18, string a_19, bool ignoreCase)//a34
        {
            return (string.Compare(a_18, a_19, ignoreCase, CultureInfo.InvariantCulture) == 0);
        }

        internal static string FormatPath(string path)
        {
            if (((path != null) && (path.Length > 0)) && (!path.EndsWith(@"\") && !path.EndsWith("/")))
            {
                path = path + "/";
            }
            return path;
        }

        internal static string a36(string a_6)
        {
            if ((a_6 != null) && (a_6.Length > 0))
            {
                a_6 = a_6.TrimEnd(new char[] { '\\', '/' });
                for (int i = a_6.Length - 1; i >= 0; i--)
                {
                    switch (a_6[i])
                    {
                        case '\\':
                        case '/':
                            a_6 = a_6.Substring(0, i);
                            return a_6;
                    }
                }
            }
            return a_6;
        }

        internal static bool IsInArray(string[] arr, string s)
        {
            if (arr != null)
            {
                int length = arr.Length;
                for (int i = 0; i < length; i++)
                {
                    string str = arr[i];
                    if (IsEqual(str, s, true))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static string ParseStr(string s)
        {
            if (s == null)
            {
                return null;
            }
            int length = s.Length;
            int startIndex = length;
            while (--startIndex >= 0)
            {
                char ch = s[startIndex];
                if (ch == '.')
                {
                    startIndex++;
                    if ((length - startIndex) > 0)
                    {
                        return s.Substring(startIndex, length - startIndex);
                    }
                    return string.Empty;
                }
                if (((ch == '\\') || (ch == '/')) || (ch == ':'))
                {
                    break;
                }
            }
            return string.Empty;
        }

        internal static string QuatStr(string s)
        {
            return ("'" + ((s != null) ? s : "") + "'");
        }

        private static string GetPath(string path, bool isPath, bool isPhysical)//a_8
        {
            if (!IsNullOrEmptyString(path))
            {
                if (HttpContext.Current == null)
                {
                    return path;
                }
                if (isPath && !path.EndsWith("/"))
                {
                    path = path + "/";
                }
                if (path.StartsWith("~/"))
                {
                    path = path.Substring(1);
                    string applicationPath = HttpContext.Current.Request.ApplicationPath;
                    if (applicationPath != "/")
                    {
                        path = applicationPath + path;
                    }
                }
                if (((isPhysical && !path.StartsWith("file://")) && (!path.StartsWith("http://") && !path.StartsWith("https://"))) && (path.IndexOf(@"\") == -1))
                {
                    path = HttpContext.Current.Server.MapPath(path);
                }
            }
            return path;
        }
    }
}
