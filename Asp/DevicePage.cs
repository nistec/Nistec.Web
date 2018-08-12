using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Collections;
using System.Text.RegularExpressions;


namespace Nistec.Web.Asp
{
    /// <summary>
    /// Asp Mobi page
    /// </summary>
    public abstract class DevicePage : System.Web.UI.Page
    {
      
        #region IsMobile

        static Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        static Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        static string[] mobileDevices = new string[] {"iphone","ppc",
                                                      "windows ce","blackberry",
                                                      "opera mini","mobile","palm",
                                                      "portable","opera mobi" };

        public bool IsMobile()
        {
             string u = Request.ServerVariables["HTTP_USER_AGENT"];

            if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
            {
                return true;// Response.Redirect("http://detectmobilebrowser.com/mobile");
            }
            return false;
        }
                       

        public bool IsCurrentDevice(string device)
        {
            if (device == null)
                return false;
            device = device.ToLower();
            return mobileDevices.Any(x => device.Contains(x));
        }

        string _deviceName = null;
        public string GetDeviceName()
        {
            if (_deviceName != null)
                return _deviceName;

            string UA = Request.ServerVariables["HTTP_USER_AGENT"];
            if (UA == null)
            {
                _deviceName = "NA";
            }
            string lUA = UA.ToLower();

            int i = 0;
            i = lUA.IndexOf("nokia");
            if (i != -1) { _deviceName = "Nokia"; return _deviceName; }
            i = lUA.IndexOf("blackberry");
            if (i != -1) { _deviceName = "BlackBerry"; return _deviceName; }
            i = lUA.IndexOf("iphone");
            if (i != -1) { _deviceName = "iPhone"; return _deviceName; }
            i = lUA.IndexOf("sonyericsson");
            if (i != -1) { _deviceName = "SonyEricsson"; return _deviceName; }
            i = UA.IndexOf("LG");
            if (i != -1) { _deviceName = "LG"; return _deviceName; }
            i = UA.IndexOf("MOT-");
            if (i != -1) { _deviceName = "MOT"; return _deviceName; }
            i = UA.IndexOf("SAMSUNG");
            if (i != -1) { _deviceName = "SAMSUNG"; return _deviceName; }
            i = lUA.IndexOf("ipod");
            if (i != -1) { _deviceName = "iPod"; return _deviceName; }
            i = lUA.IndexOf("ipad");
            if (i != -1) { _deviceName = "iPad"; return _deviceName; }
            i = UA.IndexOf("DoCoMo");
            if (i != -1) { _deviceName = "DoCoMo"; return _deviceName; }
            i = UA.IndexOf("Toshiba");
            if (i != -1) { _deviceName = "Toshiba"; return _deviceName; }
            i = UA.IndexOf("HTC");
            if (i != -1) { _deviceName = "HTC"; return _deviceName; }
            i = lUA.IndexOf("toshiba");
            if (i != -1) { _deviceName = "Toshiba"; return _deviceName; }
            i = lUA.IndexOf("palm");
            if (i != -1) { _deviceName = "Palm"; return _deviceName; }

            i = lUA.IndexOf("iemobile");
            if (i != -1) { _deviceName = "IEMobile"; return _deviceName; }
            i = lUA.IndexOf("android");
            if (i != -1) { _deviceName = "Android"; return _deviceName; }
            
            i = lUA.IndexOf("opera");
            if (i != -1) { _deviceName = "Opera"; return _deviceName; }
            i = lUA.IndexOf("firefox");
            if (i != -1) { _deviceName = "Firefox"; return _deviceName; }
            i = lUA.IndexOf("chrome");
            if (i != -1) { _deviceName = "Chrome"; return _deviceName; }

            i = lUA.IndexOf("safari");
            if (i != -1) { _deviceName = "Safari"; return _deviceName; }
            i = lUA.IndexOf("msie");
            if (i != -1) { _deviceName = "MSIE"; return _deviceName; }

            _deviceName = "NA";
            return _deviceName;

        }

        #endregion

        /// <summary>
        /// Set response NoCache
        /// </summary>
        protected void SetResponseNoCache()
        {

            Response.Cache.SetAllowResponseInBrowserHistory(false);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(60));
            Response.Cache.SetValidUntilExpires(true);
        }

        public string BaseUrl
        {
            get
            {
                return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/");
            }
        }

                     
        #region Mobile methods

       
        /// <summary>
        /// Get IsMobile
        /// </summary>
        protected bool IsMobileDevice
        {
            get
            {
                    return IsMobile();
            }
        }

        #endregion

        #region HashCapabilities

         protected bool HasForm
         {
             get { return this.Form != null; }
         }

        #endregion

        #region Meta tags

         protected virtual void RenderMobileMeta()
         {
             AddMeta("viewport", "width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=no;");
             //if (EnableMobileWebAppCapable && IsCurrentDevice("iphone"))
             //{
             //    AddMeta("apple-mobile-web-app-capable", "yes");
             //    AddMeta("apple-mobile-web-app-status-bar-style", "black-translucent");
             //}
         }

         protected virtual void SetPageTitle(string title)
         {
             if (Page.Header == null)
                 return;
             Page.Header.Title = title;
         }

         protected virtual void AddMeta(string name, string content)
         {
             if (Page.Header == null)
                 return;
             HtmlMeta meta = new HtmlMeta();
             meta.Name = name;
             meta.Content = content;
             Page.Header.Controls.Add(meta);
         }
        
         #endregion
        
        #region Render

        public const string html_5 = "<!DOCTYPE html>";

        protected void RenderDocType(XhtmlTextWriter writer)
        {
            writer.Write(html_5);
        }

        protected void RenderHtmlPage(XhtmlTextWriter writer, string html)
        {
            writer.Write(html);
        }

 
        protected void RenderBeginHtml(XhtmlTextWriter writer)
        {
            RenderDocType(writer);
            writer.Write("<html>");
        }

        protected void RenderBeginHeader(XhtmlTextWriter writer, string title, string favicon, string target, string Meta)
        {
            writer.Write("<head>");
            writer.Write(string.Format("<title>{0}</title>", title));
            if (!string.IsNullOrEmpty(Meta))
                writer.Write(Meta);
            writer.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            writer.Write(string.Format("<link rel=\"shortcut icon\" href=\"{0}\" />", favicon));
            if(!string.IsNullOrEmpty(target))
            writer.Write(string.Format("<base target=\"{0}\" />", target));
        }

        protected void RenderBeginHeader(XhtmlTextWriter writer, string title, string favicon, string keywords, string description, string target, string Meta)
        {
            writer.Write("<head>");
            writer.Write(string.Format("<title>{0}</title>", title));
            if (!string.IsNullOrEmpty(Meta))
                writer.Write(Meta);
            writer.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            writer.Write(string.Format("<meta name=\"keywords\" content=\"{0}\" />", keywords));
            writer.Write(string.Format("<meta name=\"description\" content=\"{0}\" />",description));
            writer.Write(string.Format("<link rel=\"shortcut icon\" href=\"{0}\" />", favicon));
            if(!string.IsNullOrEmpty(target))
            writer.Write(string.Format("<base target=\"{0}\" />", target));
        }

        protected void RenderBaseTag(XhtmlTextWriter writer, string type,string value)
        {
             writer.Write(string.Format("<base {0}=\"{1}\" />", type,value));
        }
        protected void RenderWcss(XhtmlTextWriter writer, string cssUrl)
        {
            writer.Write(string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" media=\"screen\" />", cssUrl));
        }
        protected void RenderCss(XhtmlTextWriter writer, string css)
        {
            if (!string.IsNullOrEmpty(css))
            {
                if (css.ToLower().Contains("<style"))
                    writer.Write(css);
                else
                    writer.Write(string.Format("<style type=\"text/css\">{0}</style>", css));
            }
        }
        protected void RenderHead(XhtmlTextWriter writer, string head)
        {
            if (!string.IsNullOrEmpty(head))
            {
                if (head.ToLower().Contains("<head"))
                {
                    head = Regx.RegexReplace("<head.*?>", head, "");
                    head = Regx.RegexReplace("</head>", head, "");
                }
                writer.Write(head);
            }
        }

        protected void RenderJs(XhtmlTextWriter writer, string jsUrl)
        {
            writer.Write(string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", jsUrl));
        }

        protected void RenderContent(XhtmlTextWriter writer, string content)
        {
            writer.Write(content);
        }

        protected void RenderMetaTag(XhtmlTextWriter writer, string name, string content)
        {
            writer.Write(string.Format("<meta name=\"{0}\" content=\"{1}\" />", name, content));
        }

        protected void RenderMetaTag(string name, string content, bool overwriteExisting)
        {
            if (this.Header == null)
                return;

            if (content == null)
                content = string.Empty;

            HtmlMeta control = this.Header.Controls.OfType<HtmlMeta>().FirstOrDefault(
                meta => string.Equals(meta.Name, name, StringComparison.OrdinalIgnoreCase));
            if (control == null)
            {
                control = new HtmlMeta();
                control.Name = name;
                control.Content = content;
                this.Header.Controls.Add(control);
            }
            else
            {
                if (overwriteExisting)
                    control.Content = content;
                else
                {
                    if (String.IsNullOrEmpty(control.Content))
                        control.Content = content;
                }
            }
        }

        protected void RenderBody(XhtmlTextWriter writer, string body, string dir)
        {
            //writer.Write(string.Format("<body>{0}</body>", body));
            writer.Write(string.Format("<body{1}>{0}</body>", body, (dir == null) ? " dir=\"rtl\"" : string.Format(" dir=\"{0}\"", dir)));
        }

        protected void RenderBodyWithEndPage(XhtmlTextWriter writer, string body, string dir)
        {
            writer.Write("</head>");
            writer.Write(string.Format("<body{1}>{0}</body>", body, (dir == null) ? " dir=\"rtl\"" : string.Format(" dir=\"{0}\"", dir)));
            writer.Write("</html>");
        }

        protected void RenderEndHeader(XhtmlTextWriter writer)
        {
            writer.Write("</head>");
        }

        protected void RenderEndHtml(XhtmlTextWriter writer, string title, string cssUrl)
        {
            writer.Write("</html>");
        }


        protected virtual void RenderViewPort(XhtmlTextWriter writer)
        {
            writer.Write("<meta name=\"viewport\" content=\"width=device-width; initial-scale=1.0; maximum-scale=1.0;user-scalable=no;\">");
        }

        #endregion

        #region static

        public static string CaptureElement(string tag, string html, bool removeOuterTags)
        {
            string pattern = @"<" + tag + ".*?>(.|\n)*?</" + tag + ">";
            Regex rg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match m = rg.Match(html);
            if (m.Success)
            {
                string value = m.Groups[0].Value;
                if (removeOuterTags)
                {
                    value = Regx.RegexReplace("<" + tag + ".*?>", value, "");
                    value = Regx.RegexReplace("</" + tag + ">", value, "");
                }
                return value;
            }
            return null;
        }
        #endregion

    }
}