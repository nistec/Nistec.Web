using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Net;
using System.Web.UI.HtmlControls;

namespace Nistec.Web.Asp
{
    public static class Winbox
    {
        #region winbox

        public enum PopupDesign
        {
            html,
            popup
        }

        public enum PopupType
        {
            div = 0,
            text = 1,
            file = 2,
            input = 3,
            iframe = 4
        }

        public enum WinboxCssType
        {
            screen,
            mail,
            cell,
            pos600,
            pos500,
            pos400,
            pos300,
            msg

        }

        static string PopupPrefix(PopupType type)
        {
            switch (type)
            {
                case PopupType.div:
                    return "#";
                case PopupType.file:
                    return "@";
                case PopupType.input:
                    return "$";
                case PopupType.iframe:
                    return "^";
                default:
                    return "";
            }
        }

        static PopupType PopupPrefix(string type)
        {
            switch (type)
            {
                case "#":
                    return PopupType.div;
                case "@":
                    return PopupType.file;
                case "$":
                    return PopupType.input;
                case "^":
                    return PopupType.iframe;
                default:
                    return PopupType.text;
            }
        }
        static string WinboxCssTypeString(WinboxCssType type)
        {
            switch (type)
            {
                case WinboxCssType.screen:
                    return "winbox-screen";
                case WinboxCssType.mail:
                    return "winbox-mail";
                case WinboxCssType.cell:
                    return "winbox-cell";
                case WinboxCssType.pos600:
                    return "winbox-600";
                case WinboxCssType.pos500:
                    return "winbox-500";
                case WinboxCssType.pos400:
                    return "winbox-400";
                case WinboxCssType.pos300:
                    return "winbox-300";
                case WinboxCssType.msg:
                    return "winbox-msg";
                default:
                    return "";
            }
        }

        static string GetSettings(PopupDesign design, string width, string height = "auto", string direction = "rtl", string overflow = "none", bool enableMask = false, bool auto = false, string onclose = null)
        {
            return "{"+string.Format("design: {0}, direction: {1}, width: {2}, height: {3}, overflow:{4}, enableMask: {5}, auto: {6}, onclose: {7}", design.ToString(), direction, width, height, overflow, enableMask.ToString().ToLower(), auto.ToString().ToLower(), onclose)+"}";
        }
        
        //function winboxMsg(content, caption, auto)
        public static void Msg(Page p, string value, string caption, bool auto = false)
        {
            string msg = string.Format("winboxMsg('{0}','{1}',{2});", CleanMessage(value), caption, auto.ToString().ToLower());
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "winbox", msg, true);
        }

        //function winboxPopupCss(mode, content, caption, cls, auto) {
        public static void PopupCss(Page p, string value, string caption, WinboxCssType cssType, PopupType type)
        {

            if (type == PopupType.text)
                value = HtmlHelper.HtmlEncodeData(value);
            else if (type == PopupType.file)
            {
                if (value.StartsWith("~"))
                    value = p.ResolveClientUrl(value);
            }

            string mode = PopupPrefix(type);
            string cls = WinboxCssTypeString(cssType);
            string msg = string.Format("winboxPopupCss('{0}', '{1}', '{2}', '{3}'));", mode, value, caption, cls);
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "winbox", msg, true);
        }

        public static void Popup(Page p, string mode, string value, string caption=null)
        {
            PopupType type = PopupPrefix(mode);
            Popup(p, type, value, caption, null, null, true, PopupDesign.popup);
        }

        public static void Popup(Page p, string mode, string value, string caption, string width, string height, bool allowScroll)
        {
            PopupType type = PopupPrefix(mode);
            Popup(p, type, value, caption, width, height, allowScroll, PopupDesign.popup);
        }

        //winboxPopup(mode, content, caption, setting, css) 
        public static void Popup(Page p, PopupType type, string value, string caption, string width, string height, bool allowScroll, PopupDesign design)
        {
            string css = string.Format("{0}{1}{2}",
                width == null ? "" : "'width':'" + width.ToString() + "px',",
                height == null ? "" : "'height':'" + height.ToString() + "px',",
                allowScroll ? "'overflow':'auto'" : "");

            if (!string.IsNullOrEmpty(css))
            {
                css = "{" + css.TrimEnd(',') + "}";
            }
            else
                css = null;

            if (type == PopupType.text)
                value = HtmlHelper.HtmlEncodeData(value);
            else if (type == PopupType.file)
            {
                if (value.StartsWith("~"))
                    value = p.ResolveClientUrl(value);
            }

            string mode = PopupPrefix(type);
            //string dir = null;
            //bool auto = false;
            //string settings = string.Format("{'dir':{0}, 'width':{1}, 'height':{2}, 'auto':{3}}", dir, width, height, auto.ToString().ToLower());
            string settings = GetSettings(design, width, height);
            string msg = string.Format("winboxPopup('{0}', '{1}', '{2}', {3}, '{4}');", mode, value, caption, settings, css);
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "winbox", msg, true);
        }

        public static void Iframe(Page p, string value, string caption, string width, string height, bool allowScroll, string onclose=null)
        {
            string css = string.Format("{0}{1}{2}",
                width == null ? "" : "'width':'" + width.ToString() + "px',",
                height == null ? "" : "'height':'" + height.ToString() + "px',",
                allowScroll ? "'overflow':'auto'" : "");

            if (!string.IsNullOrEmpty(css))
            {
                css = "{" + css.TrimEnd(',') + "}";
            }
            else
                css = null;

            if (value.StartsWith("~"))
                value = p.ResolveClientUrl(value);

            string dir = null;

            string settings = "{" + string.Format("'dir':{0}, 'width':{1}, 'height':{2}, 'onclose':{3}", dir, width, height, onclose) + "}";
            string msg = string.Format("winboxIframe('{0}', '{1}', {2}, '{3}');", value, caption, settings, css);
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "winbox", msg, true);
        }

        public static void Close(Page p)
        {
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "winbox", "winboxClose();", true);
        }

         public static void CloseParent(Page p)
        {
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "winbox", "winboxCloseParent();", true);
        }
       
        #endregion

        #region window UI

        //function winboxShowArgs(elm, isModal,isDrag, func,args) {
        //function winboxShow(elm, isModal, top, left) {
        //function winboxHide(elm) {
        //function winbox_Toggle(el) {
        public static void Hide(Page p, string window)
        {
            ScriptManager.RegisterClientScriptBlock(p, p.GetType(), "winbox", "winboxHide('" + window + "');", true);
        }
        public static void Show(Page p, string window, bool isModal = false)
        {
            ScriptManager.RegisterClientScriptBlock(p, p.GetType(), "winbox", string.Format("winboxShow('{0}',{1});", window, isModal.ToString().ToLower()), true);
        }
        public static void ShowArgs(Page p, string window, bool isModal, bool isDrag, string func, string args)
        {
            ScriptManager.RegisterClientScriptBlock(p, p.GetType(), "winbox", string.Format("winboxShowArgs('{0}',{1}, {2},{3},'{4}');", window, isModal.ToString().ToLower(), isDrag.ToString().ToLower(), func, args), true);
        }
        public static void Draggable(Page p, string window)
        {
            ScriptManager.RegisterClientScriptBlock(p, p.GetType(), "winbox", string.Format("jQuery.windrag('{0}','winbox-caption');", window), true);
        }

        #endregion
        
        #region window helper

        static string CleanMessage(string text)
        {
            return text.Replace("\r\n", "\\n").Replace("'", " ");
        }
        static string GetDir(bool isRtl)
        {
            return isRtl ? "rtl" : "ltr";
        }
        static string GetWidth(int width)
        {
            return width == 0 ? null : width.ToString();
        }
        static string BoolToString(bool value)
        {
            return value.ToString().ToLower();
        }

        public static void WindowDisplay(HtmlGenericControl c, bool display)
        {
            c.Style["display"] = display ? "block" : "none";
        }

        public static void ToogleWindow(HtmlGenericControl c1, HtmlGenericControl c2, bool display1)
        {
            c1.Style["display"] = display1 ? "block" : "none";
            c2.Style["display"] = display1 ? "none" : "block";
        }

        public static void OpenWindow(Page p, string control, string body, string msg)
        {
            HtmlGenericControl c = (HtmlGenericControl)p.Master.FindControl(control);
            HtmlGenericControl b = (HtmlGenericControl)p.Master.FindControl(body);
            b.InnerHtml = msg.Replace("\r\n", "<br/>");
            c.Style["display"] = "block";
        }
        //public static void OpenWindowAlert(Page p, string msg)
        //{
        //    OpenWindow(p, "windowAlert", "windowAlert_body", msg);
        //}
        public static void CloseWindow(HtmlGenericControl c)
        {
            c.Style["display"] = "none";
        }

        public static void Refresh(Page p)
        {
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "refresh", "javascript:setTimeout('window.location.reload(true);',600);", true);
            //ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "refresh", "javascript:window.location.reload( false );", true);
        }

        public static void Reload(Page p)
        {
            p.Response.Write("<script>setTimeout(\"location.reload(true);\", 600);</script>");
        }
 

        public static void RegisterClientScriptBlock(Page p, string script)
        {
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "script", script, true);
        }
        public static void RegisterStartupScript(Page p, string script)
        {
            ScriptManager.RegisterStartupScript(p, p.ClientScript.GetType(), "script", script, true);
        }
        public static void LoadHtml(Page p, string div, string filename)
        {
            string script = string.Format("$('#{0}').load('{1}');", div, filename);
            ScriptManager.RegisterStartupScript(p, p.ClientScript.GetType(), "script", script, true);
        }

        public static string DownloadHtml(Page p, string filename)
        {
            try
            {
                string url = p.Server.MapPath(p.ResolveClientUrl(filename));
                WebClient client = new WebClient();
                String htmlCode = client.DownloadString(url);
                return htmlCode;
                // Replace all html breaks for line seperators.
                //htmlCode = htmlCode.Replace("<br>", "\r\n");

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string ResolveClientUrl(Page p,string url)
        {
            if (url.StartsWith("~"))
                return p.ResolveClientUrl(url);
            return url;
        }

        #endregion

        #region window alerts functions

        public static void ToogleWindow(Page p, string window, bool show)
        {
            string msg = string.Format("{0}.style.display ='{1}';", window, show ? "block" : "none");
            ScriptManager.RegisterClientScriptBlock(p, p.GetType(), "window", msg, true);
        }

        public static void Alert(Page p, string text)
        {
            //HttpContext.Current
            string alert = string.Format("alert('{0}');", CleanMessage(text));
            ScriptManager.RegisterClientScriptBlock(p, p.GetType(), "Alert", alert, true);
        }
       
        public static void Redirect(Page p, string redirectTo)
        {
            redirectTo = ResolveClientUrl(p,redirectTo);
            string msg = string.Format("window.location.href=('{0}');", redirectTo);
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "redirect", msg, true);
        }
        public static void RedirectTo(Page p, string redirectTo, string args)
        {
            redirectTo = ResolveClientUrl(p, redirectTo);
            string msg = string.Format("window.location.href=('" + redirectTo + "?&arg={0}');", args);
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "redirect", msg, true);
        }
        public static void RedirectToErr(Page p, string message, string args=null)
        {
            string url = Nistec.Generic.NetConfig.AppSettings["ErrUrl"];
            if (string.IsNullOrEmpty(url))
                return;
            url = ResolveClientUrl(p, url);
            string msg = string.Format("window.location.href=('" + url + "?m={0}&arg={1}');", message, args);
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "redirect", msg, true);
        }
        
        public static void WindowOpen(Page p, string redirectTo, string name, int height, int width)
        {
            redirectTo = ResolveClientUrl(p, redirectTo);
            string Args = "height: " + height + "px; width: " + width + "px; edge: Raised; center: Yes; help: No; resizable: No; status: No";
            string msg = string.Format("window.open('{0}','{1}','{2}');", redirectTo, name, Args);
            ScriptManager.RegisterClientScriptBlock(p, p.ClientScript.GetType(), "redirect", msg, true);
        }
        #endregion
  
             
    }
}
