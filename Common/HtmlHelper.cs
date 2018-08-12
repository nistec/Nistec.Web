using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.Data;
using System.Linq;
using System.Collections;
//using Nistec.Web.Formatter;
using Nistec.Drawing;

namespace Nistec.Web
{
    [Serializable]
    public struct GenericLinkItem
    {
        public string Href{get;set;}
        public string Text { get; set; }
        public string Title { get; set; }
        public string CssClass { get; set; }
        public string Id { get; set; }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Href) || string.IsNullOrEmpty(Text); }
        }

        public string FormatLink()
        {
            return HtmlHelper.FormatLink(Href, Text, Title);
        }
    }

    [Serializable]
    public struct GenericElementItem
    {
        public string Value { get; set; }
        public string Title { get; set; }
        public string CssClass { get; set; }
        public string Id { get; set; }
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Value); }
        }

        public string FormatTag(string tagName)
        {
            return HtmlHelper.FormatTag(tagName,Value,CssClass,"");
        }
    }

    /// <summary>
    /// Represents a HTML helper
    /// </summary>
    public partial class HtmlHelper
    {
        #region Fields
        private static Regex paragraphStartRegex = new Regex("<p>", RegexOptions.IgnoreCase);
        private static Regex paragraphEndRegex = new Regex("</p>", RegexOptions.IgnoreCase);
        //private static Regex ampRegex = new Regex("&(?!(?:#[0-9]{2,4};|[a-z0-9]+;))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex regexBold = new Regex(@"\[b\](.+?)\[/b\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexItalic = new Regex(@"\[i\](.+?)\[/i\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexUnderLine = new Regex(@"\[u\](.+?)\[/u\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexUrl1 = new Regex(@"\[url\=([^\]]+)\]([^\]]+)\[/url\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexUrl2 = new Regex(@"\[url\](.+?)\[/url\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexQuote = new Regex(@"\[quote=(.+?)\](.+?)\[/quote\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        #endregion

        #region Utilities

        private static string EnsureOnlyAllowedHtml(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            string allowedTags = "br,hr,b,i,u,a,div,ol,ul,li,blockquote,img,span,p,em,strong,font,pre,h1,h2,h3,h4,h5,h6,address,ciate";

            var options = RegexOptions.IgnoreCase;
            var m = Regex.Matches(text, "<.*?>", options);
            for (int i = m.Count - 1; i >= 0; i--)
            {
                string tag = text.Substring(m[i].Index + 1, m[i].Length - 1).Trim().ToLower();

                if (!IsValidTag(tag, allowedTags))
                {
                    text = text.Remove(m[i].Index, m[i].Length);
                }
            }

            return text;
        }

        private static bool IsValidTag(string tag, string tags)
        {
            string[] allowedTags = tags.Split(',');
            if (tag.IndexOf("javascript") >= 0) return false;
            if (tag.IndexOf("vbscript") >= 0) return false;
            if (tag.IndexOf("onclick") >= 0) return false;

            char[] endchars = new char[] { ' ', '>', '/', '\t' };

            int pos = tag.IndexOfAny(endchars, 1);
            if (pos > 0) tag = tag.Substring(0, pos);
            if (tag[0] == '/') tag = tag.Substring(1);

            foreach (string aTag in allowedTags)
            {
                if (tag == aTag) return true;
            }

            return false;
        }


        public static bool IsHtml(string body)
        {
            //<([A-Z][A-Z0-9]*)\b[^>]*>(.*?)</\1>

            string pattern = @"<([A-Z][A-Z0-9]*)\b[^>]*>(.*?)</\1>";

            return Nistec.Regx.RegexValidateIgnoreCase(pattern,body);
        }

        public static string CaptureHead(string html, bool removeOuterTags)
        {
            //string pattern = @"<head.*?>(.|\n)*?</head>";
            return CaptureElement("head", html, removeOuterTags);
        }

        public static string CaptureStyleSheet(string html, bool removeOuterTags)
        {
            //string pattern = @"<style.*?>(.|\n)*?</style>";
            return CaptureElement("style", html, removeOuterTags);
        }

        public static string CaptureBody(string html, bool removeOuterTags)
        {
            //string pattern = @"<body.*?>(.|\n)*?</body>";
            return CaptureElement("body", html, removeOuterTags);
        }

        public static string CaptureElement(string tag, string html, bool removeOuterTags, bool isMulti = false)
        {
            string pattern = @"<" + tag + ".*?>(.|\n)*?</" + tag + ">";
            Regex rg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match m = rg.Match(html);
            if (m.Success)
            {
                string value = m.Groups[0].Value;
                if (removeOuterTags)
                {
                    value = Nistec.Regx.RegexReplace("<" + tag + ".*?>", value, "");
                    value = Nistec.Regx.RegexReplace("</" + tag + ">", value, "");
                }
                if (isMulti)
                {
                    html = Regex.Replace(html, value, "");

                    return value + CaptureElement(tag, html, removeOuterTags, isMulti);
                }
                return value;
            }
            return null;
        }

      

       
        #endregion

        #region Methods
        /// <summary>
        /// Formats the text
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="stripTags">A value indicating whether to strip tags</param>
        /// <param name="convertPlainTextToHtml">A value indicating whether HTML is allowed</param>
        /// <param name="allowHtml">A value indicating whether HTML is allowed</param>
        /// <param name="allowBBCode">A value indicating whether BBCode is allowed</param>
        /// <param name="resolveLinks">A value indicating whether to resolve links</param>
        /// <param name="addNoFollowTag">A value indicating whether to add "noFollow" tag</param>
        /// <returns>Formatted text</returns>
        public static string FormatText(string text, bool stripTags,
            bool convertPlainTextToHtml, bool allowHtml, 
            bool allowBBCode, bool resolveLinks, bool addNoFollowTag)
        {

            if (String.IsNullOrEmpty(text))
                return string.Empty;

            try
            {
                if (stripTags)
                {
                    text = HtmlHelper.StripTags(text);
                }

                if (allowHtml)
                {
                    text = HtmlHelper.EnsureOnlyAllowedHtml(text);
                }
                else
                {
                    text = HttpUtility.HtmlEncode(text);
                }

                if (convertPlainTextToHtml)
                {
                    text = HtmlHelper.ConvertPlainTextToHtml(text);
                }

                if (allowBBCode)
                {
                    text = HtmlHelper.FormatText(text, true, true, true, true, true, true);
                }

                if (resolveLinks)
                {
                    text = LinkResolver.FormatText(text);
                }

                if (addNoFollowTag)
                {
                    //add noFollow tag. not implemented
                }
            }
            catch (Exception exc)
            {
                text = string.Format("Text cannot be formatted. Error: {0}", exc.Message);
            }
            return text;
        }
        
        /// <summary>
        /// Strips tags
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string StripTags(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = Regex.Replace(text, @"(>)(\r|\n)*(<)", "><");
            text = Regex.Replace(text, "(<[^>]*>)([^<]*)", "$2");
            text = Regex.Replace(text, "(&#x?[0-9]{2,4};|&quot;|&amp;|&nbsp;|&lt;|&gt;|&euro;|&copy;|&reg;|&permil;|&Dagger;|&dagger;|&lsaquo;|&rsaquo;|&bdquo;|&rdquo;|&ldquo;|&sbquo;|&rsquo;|&lsquo;|&mdash;|&ndash;|&rlm;|&lrm;|&zwj;|&zwnj;|&thinsp;|&emsp;|&ensp;|&tilde;|&circ;|&Yuml;|&scaron;|&Scaron;)", "@");

            return text;
        }

        /// <summary>
        /// Converts plain text to HTML
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string ConvertPlainTextToHtml(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = text.Replace("\r\n", "<br />");
            text = text.Replace("\r", "<br />");
            text = text.Replace("\n", "<br />");
            text = text.Replace("\t", "&nbsp;&nbsp;");
            //text = text.Replace("  ", "&nbsp;&nbsp;");

            return text;
        }

        /// <summary>
        /// Converts HTML to plain text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string ConvertHtmlToPlainText(string text)
        {
            return ConvertHtmlToPlainText(text, false);
        }

        /// <summary>
        /// Converts HTML to plain text
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="decode">A value indicating whether to decode text</param>
        /// <returns>Formatted text</returns>
        public static string ConvertHtmlToPlainText(string text, bool decode)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            if (decode)
                text = HttpUtility.HtmlDecode(text);

            text = text.Replace("<br>", "\n");
            text = text.Replace("<br >", "\n");
            text = text.Replace("<br />", "\n");
            text = text.Replace("&nbsp;&nbsp;", "\t");
            text = text.Replace("&nbsp;&nbsp;", "  ");

            return text;
        }

        /// <summary>
        /// Converts text to paragraph
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string ConvertPlainTextToParagraph(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = paragraphStartRegex.Replace(text, string.Empty);
            text = paragraphEndRegex.Replace(text, "\n");
            text = text.Replace("\r\n", "\n").Replace("\r", "\n");
            text = text + "\n\n";
            text = text.Replace("\n\n", "\n");
            var strArray = text.Split(new char[] { '\n' });
            var builder = new StringBuilder();
            foreach (string str in strArray)
            {
                if ((str != null) && (str.Trim().Length > 0))
                {
                    builder.AppendFormat("<p>{0}</p>\n", str);
                }
            }
            return builder.ToString();
        }

        ///// <summary>
        ///// Formats the text
        ///// </summary>
        ///// <param name="text">Text</param>
        ///// <param name="replaceBold">A value indicating whether to replace Bold</param>
        ///// <param name="replaceItalic">A value indicating whether to replace Italic</param>
        ///// <param name="replaceUnderline">A value indicating whether to replace Underline</param>
        ///// <param name="replaceUrl">A value indicating whether to replace URL</param>
        ///// <param name="replaceCode">A value indicating whether to replace Code</param>
        ///// <param name="replaceQuote">A value indicating whether to replace Quote</param>
        ///// <returns>Formatted text</returns>
        //public static string FormatBoldText(string text, bool replaceBold, bool replaceItalic,
        //    bool replaceUnderline, bool replaceUrl, bool replaceCode, bool replaceQuote)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return string.Empty;

        //    if (replaceBold)
        //    {
        //        // format the bold tags: [b][/b]
        //        // becomes: <strong></strong>
        //        text = regexBold.Replace(text, "<strong>$1</strong>");
        //    }

        //    if (replaceItalic)
        //    {
        //        // format the italic tags: [i][/i]
        //        // becomes: <em></em>
        //        text = regexItalic.Replace(text, "<em>$1</em>");
        //    }

        //    if (replaceUnderline)
        //    {
        //        // format the underline tags: [u][/u]
        //        // becomes: <u></u>
        //        text = regexUnderLine.Replace(text, "<u>$1</u>");
        //    }

        //    if (replaceUrl)
        //    {
        //        // format the url tags: [url=http://www.net.com]my site[/url]
        //        // becomes: <a href="http://www.net.com">my site</a>
        //        text = regexUrl1.Replace(text, "<a href=\"$1\" rel=\"nofollow\">$2</a>");

        //        // format the url tags: [url]http://www.net.com[/url]
        //        // becomes: <a href="http://www.net.com">http://www.net.com</a>
        //        text = regexUrl2.Replace(text, "<a href=\"$1\" rel=\"nofollow\">$1</a>");
        //    }

        //    if (replaceQuote)
        //    {
        //        while (regexQuote.IsMatch(text))
        //            text = regexQuote.Replace(text, "<b>$1 wrote:</b><div class=\"quote\">$2</div>");
        //    }

        //    if (replaceCode)
        //    {
        //        text = CodeFormatHelper.FormatTextSimple(text);
        //    }

        //    return text;
        //}

        /// <summary>
        /// Removes all quotes from string
        /// </summary>
        /// <param name="str">Source string</param>
        /// <returns>string</returns>
        public static string RemoveQuotes(string str)
        {
            str = Regex.Replace(str, @"\[quote=(.+?)\]", String.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\[/quote\]", String.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return str;
        }
        #endregion

        #region Format


        public static string FormatLinkWrapper(string html)
        {
            string href = "";
            string pattern = "href=\"(?<attr>.*?)\"";
            Match m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (m.Success && m.Groups.Count > 1)
            {
                href = m.Groups[1].Value;//.ToString();
                string jref = "href=\"javascript:linkWrapper('" + href + "')\"";
                html = Regex.Replace(html, pattern, jref, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            return html;
        }

        public static string FormatLinkWrapperToHref(string html)
        {
            //string href = "";
            //string pattern = "href=\"javascript:linkWrapper\\('(?<attr>.*?)'\\)\"";
            string pattern = "javascript:linkWrapper\\('(?<attr>.*?)'\\)";

            Match m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (m.Success && m.Groups.Count > 1)
            {
                string jref=m.Groups[0].Value;
                //href = "href=\""+m.Groups[1].Value+"\"";
                string replacment = m.Groups[1].Value;
                if (replacment == "http://")
                    replacment = "#";
                html = html.Replace(jref, replacment);

                //string jref = "href=\"javascript:linkWrapper('" + href + "')\"";

                //html = Regex.Replace(html, jref, href, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            return html;
        }


        public static string FormatHtmlPreview(string html)
        {
            string pattern = "href=\"(?<attr>.*?)\"";
            html = Regex.Replace(html, pattern, "href=\"#\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            pattern = "target=\"(?<attr>.*?)\"";
            html = Regex.Replace(html, pattern, "target=\"_self\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            pattern = "onsubmit=\"(?<attr>.*?)\"";
            html = Regex.Replace(html, pattern, "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            pattern = "onclick=\"(?<attr>.*?)\"";
            html = Regex.Replace(html, pattern, "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            
            //pattern = "<form.*?>";
            //html = Regex.Replace(html, pattern, "<form>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return html;
        }

        public static string FormatHref(string href)
        {
            return Regex.Replace(href, "&(?!amp;|nbsp;)", "&amp;", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }
        public static string CleanHref(string href)
        {
            return Regex.Replace(href, "&amp;", "&", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }
        public static string FormatIframe(string src, string width, string height, bool scroling)
        {
            return string.Format("<iframe src=\"{0}\" width=\"{1}\" height=\"{2}\" scrolling=\"{3}\" frameborder=\"0\"></iframe>", src, width, height, scroling ? "yes" : "no");
        }
        public static string FormatImg(string src, string css, int Width, int Height)
        {
            //return string.Format("<center><img style=\"{1}\" src=\"{0}\" alt=\"{4}\" width=\"{2}px\" height=\"{3}px\" /></center>", src, css, Width, Height, "");
            return string.Format("<img style=\"{1}\" src=\"{0}\" alt=\"{4}\" width=\"{2}px\" height=\"{3}px\" />", src, css, Width, Height, "");
        }
        public static string FormatImg(string src, string alt)
        {
            return string.Format("<img src=\"{0}\" alt=\"{1}\" />", src, alt);
        }

        public static string FormatTag(string tag, string value, string style)
        {
            return string.Format("<{0}{2}>{1}</{0}>", tag, value, FormatStyle(style));
        }
        public static string FormatTag(string tag, string value, string cssClass, string style)
        {
            return string.Format("<{0}{2}{3}>{1}</{0}>", tag, value, FormatCssClass(cssClass) ,FormatStyle(style));
        }
        public static string FormatLink(string url, string caption, string title=null)
        {
            if (title == null) title = caption;
            return string.Format("<a href=\"{0}\" title=\"{1}\">{1}</a>", url, caption,title);
        }
        public static string FormatLink(string url, string caption, string title,string style)
        {
            return string.Format("<a href=\"{0}\" title=\"{2}\" {3}>{1}</a>", url, caption, title, FormatStyle(style));
        }
        public static string FormatP(string content, string style)
        {
            return string.Format("<p{1}>{0}</p>", content, FormatStyle(style));
        }
       
        public static string FormatDIV(string content, string style)
        {
            return string.Format("<div{1}>{0}</div>", content, FormatStyle(style));
        }
        public static string FormatSPAN(string content, string style)
        {
            return string.Format("<span{1}>{0}</span>", content, FormatStyle(style));
        }
        public static string FormatLI(string content, string style)
        {
            return string.Format("<li{1}>{0}</li>", content, FormatStyle(style));
        }
        
        public static string FormatLILink(string url, string caption, string title, string style)
        {
            return string.Format("<li{1}>{0}</li>", FormatLink(url, caption, title), FormatStyle(style));
        }
        public static string FormatRadioButton(string name, string value, string style, bool isChecked)
        {
            return string.Format("<input type=\"radio\" {3} name=\"{0}\" value=\"{1}\" {2} />", name, value, FormatStyle(style), FormatChecked(isChecked));
        }
        public static string FormatRadioButton(string name, string value, string style, bool isChecked, string text)
        {
            return string.Format("<input type=\"radio\" {3} name=\"{0}\" value=\"{1}\" /><span{2}>{4}</span>", name, value, FormatStyle(style), FormatChecked(isChecked), text);
        }
        public static string FormatRadioButtonServer(string id, string name, string value, string style, bool isChecked, string text)
        {
            return string.Format("<input type=\"radio\" runat=\"server\" id=\"{0}\" {4} name=\"{1}\" value=\"{2}\" /><span{3}>{5}</span>", id, name, value, FormatStyle(style), FormatChecked(isChecked), text);
        }
        public static string FormatButton(string id, string name, string value, string style, string serverClick)
        {
            return string.Format("<input type=\"button\" runat=\"server\" {4} id=\"{0}\" name=\"{1}\" value=\"{2}\" {3} />", id, name, value, FormatStyle(style), FormatServerClick(serverClick));
        }
        public static string FormatButtonSubmit(string id, string name, string value, string style)
        {
            return string.Format("<input type=\"submit\"  id=\"{0}\" name=\"{1}\" value=\"{2}\" {3} />", id, name, value, FormatStyle(style));
        }
        public static string FormatInput(string id, string value, string style)
        {
            return string.Format("<input type=\"text\" {2} id=\"{0}\" value=\"{1}\" />", id, value, FormatStyle(style));
        }
        public static string FormatHidden(string id, string value, bool isServer)
        {
            return string.Format("<input type=\"hidden\" {2} id=\"{0}\" value=\"{1}\" />", id, value, isServer ? "runat=\"server\"" : "");
        }
        public static string FormatSelectList(string id, string name, string style, string[] items)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<select id=\"{0}\" name=\"{1}\" {2}>", id, name, FormatStyle(style));
            for (int i = 0; i < items.Length; i++)
            {
                string[] arg = items[i].Split('|');
                string oname = arg.Length > 1 ? arg[1] : arg[0];
                string oval = arg.Length > 1 ? arg[0] : i.ToString();

                sb.AppendFormat("<option value=\"{0}\" >{1}</option>", oval, oname, i == 0 ? "selected" : "");
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        public static string FormatCssClass(string css)
        {
            if (string.IsNullOrEmpty(css))
                return "";
            if (css.ToLower().Contains("class="))
                return " " + css.TrimStart();
            return string.Format(" class=\"{0}\" ", css);
        }

        public static string FormatDir(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return "";
            if (dir.ToLower().Contains("dir="))
                return " " + dir.TrimStart();
            return string.Format(" dir=\"{0}\" ", dir);
        }

        public static string FormatStyle(string style)
        {
            if (string.IsNullOrEmpty(style))
                return "";
            if (style.ToLower().Contains("style="))
                return " " + style.TrimStart();
            return string.Format(" style=\"{0}\" ", style);
        }
        public static string FormatStyle(string style, string add)
        {
            return string.IsNullOrEmpty(style) ? string.Format(" style=\"{0}\" ", add) : string.Format(" style=\"{0};{1}\" ", style.TrimEnd(';'), add);
        }
        public static string FormatStyle(int width, int height, bool enableScroll)
        {
            string style = "";
            if (width > 0)
                style += "width:" + width.ToString() + "px;";
            if (height > 0)
                style += "height:" + height.ToString() + "px;";
            if (enableScroll)
                style += "overflow:auto;";
            return style;
        }
        public static string FormatServerClick(string onserverclick)
        {
            return string.IsNullOrEmpty(onserverclick) ? "" : string.Format(" onserverclick=\"{0}\" ", onserverclick);
        }
        public static string FormatChecked(bool isChecked)
        {
            return isChecked ? "checked=checked" : "";
        }

        /*see wapUtil
                public static string ResolveImage(string filename, int maxWidth, ref int width, ref int height)
                {
                    if (string.IsNullOrEmpty(filename))
                    {
                        return filename;
                    }

                    bool resize = maxWidth > 0;
                    //int maxWidth = 310;
                    string src = filename;
                    int w = 0;
                    int h = 0;
                    float diff = 1f;

                    try
                    {
                        string[] filearg = filename.Split('?');
                        src = filearg[0];
                        if (filearg.Length > 1)
                        {
                            string[] dim = filearg[1].Split('&');

                            w = Types.ToInt(dim[0].Split('=')[1], 0);
                            h = Types.ToInt(dim[1].Split('=')[1], 0);
                            if (resize && w > maxWidth)
                            {
                                diff = (float)maxWidth / (float)w;
                                w = (int)((float)w * diff);
                                h = (int)((float)h * diff);
                            }
                            width = w;
                            height = h;
                        }
                    }
                    catch
                    {

                    }
                    return src;
                }

                public static string LinkTagFormat(string warpTag, string url,string qs, string caption, string css)
                {
                    string u=url == "#" ? url : url + "?=" + qs;
                    return LinkTagFormat(warpTag, u, caption, css);
                }

                public static string LinkTagFormat(string warpTag, string url,string caption, string css)
                {
                    const string BaseLingTag = "<{0} {1}><a href='{3}'>{2}</a>{4}</{0}>";
                    string tagName = warpTag;
                    if (string.IsNullOrEmpty(tagName))
                        tagName = "p";
                    tagName = tagName.ToLower();
                    string space = tagName == "span" ? "&nbsp;" : "";
                    return string.Format(BaseLingTag, tagName, css, caption, url , space);
                }

                public static TagType ItemTagType(string tag)
                {
                         switch (tag.ToUpper())
                        {
                            case "P":
                                return TagType.P;
                            case "DIV":
                                return TagType.Div;
                            case "SPAN":
                                return TagType.Span;
                            case "H1":
                                return TagType.H1;
                            case "H2":
                                return TagType.H2;
                            case "H3":
                                return TagType.H3;
                            case "H4":
                                return TagType.H4;
                            default:
                                return TagType.P;
                        }
                }

 

                public static string ImgTagFormat(string FileName, int Width, int Height, int Zoom, int maxWidth, string upload_path, string vitual_path)
                {
                    const string imgTag = "<img src='{0}' alt='' {1} />";
                    string filename = FileName;

                    string img = "";
                    if (maxWidth <= 0) maxWidth = WapItem.DefaultScreenWidth;

                    if (string.IsNullOrEmpty(filename))
                        return null;// Contents.ImgTagFormat(filename, maxWidth, Zoom, upload_path, vitual_path);

                    if (!filename.ToLower().StartsWith("http://"))
                    {
                        string file = System.IO.Path.GetFileName(filename);
                        filename = vitual_path + file;// item.Filename.ToLower().Replace("~", "..");
                    }

                    int w = 0;
                    int h = 0;
                    float z = 1f;
                    float diff = 1f;
                    string src = filename;

                    try
                    {
                        if (Width > 0 && Height > 0)
                        {

                            w = Width;
                            h = Height;
                            z = Zoom;
                            if (w > maxWidth)
                            {
                                if (Zoom <= 0) Zoom = 100;
                                z = (float)Zoom / 100;

                                diff = (float)maxWidth / (float)(w * z);
                                w = (int)((float)w * diff);
                                h = (int)((float)h * diff);
                                img = string.Format(imgTag, src, "width=" + w.ToString() + "px height=" + h + "px");
                            }
                            else
                            {
                                img = string.Format(imgTag, src, "");
                            }


                        }
                        else
                        {
                            img = Contents.ImgTagFormat(filename, maxWidth, Zoom, upload_path, vitual_path);
                            //img = string.Format(imgTag, src, "");
                        }

                        return img;
                    }
                    catch
                    {
                        return string.Format(imgTag, src, "");
                    }
                }
        */
        #endregion

        #region css

        public static string AddToStyle(string css, string style)
        {
            if (string.IsNullOrEmpty(css))
                return FormatStyle(style);// string.Format("style=\"{0}\"", style);
            if (string.IsNullOrEmpty(style))
                return css;

            string[] arg = style.Trim().Split(':');
            if (css.Contains(arg[0]))
            {
                return MergCss(css, style);
            }

            string sep = css.TrimEnd().EndsWith(";") ? ";" : "";

            return css.Trim('"').Trim() + sep + style + "\"";
        }

        public static string MergCss(string css1, string css2)
        {
            Dictionary<string, string> css = SplitCss(new string[] { css1, css2 });
            return CreateCss(true, css);
        }

        public static Dictionary<string, string> SplitCss(string[] css)
        {
            Dictionary<string, string> style = new Dictionary<string, string>();
            if (css == null)
                return style;

            foreach (string cs in css)
            {
                string c = cs;
                c = Regex.Replace(cs, "style=\"", "", RegexOptions.IgnoreCase);
                c = c.Trim('"').Trim();
                string[] args = c.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in args)
                {
                    string[] arg = s.Split(':');
                    if (arg.Length > 1)
                    {
                        string key = arg[0].Trim(';');
                        key = key.Trim();
                        string value = arg[1].Trim(';');
                        value = value.Trim();
                        style[key] = value;
                    }
                }
            }
            return style;
        }
        public static string CreateCss(bool addTag, Dictionary<string, string> css)
        {
            if (css == null)
                return "";
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> entry in css)
            {
                sb.AppendFormat("{0}:{1};", entry.Key, entry.Value);

            }
            if (addTag && sb.Length > 0)
            {
                return string.Format("style=\"{0}\"", sb.ToString());
            }

            return sb.ToString();
        }


        public static bool IsKnownColor(string color)
        {
            return ColorUtils.KnownColorNames.Contains(color);
        }


        #endregion

        #region Encode/Decode

        public static string GetPlainText(string s)
        {
            s = Regex.Replace(s, "[ \n]+", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "<script[^>]*>.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "<br[^>]*>", "\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "<p [^>]*>", "\n\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "</p [^>]*>", "\n\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "<p>", "\n\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "</p>", "\n\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "<hr>", "\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "<!--.*?-->", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "</h2>", "\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "</h3>", "\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "</h4>", "\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "<[^>]*>", "");
            s = Regex.Replace(s, "\n ", "\n");
            return HttpContext.Current.Server.HtmlDecode(s);
        }
        
        public static string HtmlDecode(string s)
        {
            return s.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"");
        }
        public static string HtmlEncode(string s)
        {
            return s.Replace("\r\n", "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&quot;");
        }
        public static string CleanHtml(string html)
        {
            return html.Replace("\r\n", "<br/>").Replace("\n", "<br/>");
        }

        public static char[] HtmlEncodeArray = new char[]{'%', '<', '>', '!', '\'', '#', '$', '&', '"', '(', ')', ',', ':', ';', '=', '?', '[', '\\', ']', '^', '`', '{', '|', '}', '~', '+'};

        public static string HtmlEncodeData(string html)
        {
            if (html != null && html.Length > 0)
            {
                html=html.Replace("\r\n", "").Replace("\n", "");

                for (int i = 0, len = HtmlEncodeArray.Length; i < len; i++)
                {
                    html = html.Replace(HtmlEncodeArray[i].ToString(), "%" + Convert.ToInt32(HtmlEncodeArray[i]).ToString("x"));
                }
            }
            return html;
        }

        public static string HtmlDecodeData(string html)
        {
            if (html != null && html.Length > 0)
            {
                for (int i = HtmlEncodeArray.Length - 1; i >= 0; i--)
                {
                    html = html.Replace("%" + Convert.ToInt32(HtmlEncodeArray[i]).ToString("x"), HtmlEncodeArray[i].ToString());
                }
            }
            return html;
        }

        #endregion

        #region parse

        //<div.*?>(.*?)<\/div>
        public static string ParseElementInner(string tagName, string content)
        {
            string pattern = string.Format("<{0}.*?>(.*?)<\\/{0}>", tagName);// "<div.*?>(.*?)<\/div>";
            Match m = System.Text.RegularExpressions.Regex.Match(content, pattern);
            if (m.Success && m.Groups.Count > 0)
            {
                return m.Groups[1].ToString();
            }
            return "";
        }


        public static GenericLinkItem ParseLink(string link)
        {
            string caption = ParseElementInner("a", link);
            string url = ParseAttribute(link, "href");
            string title = ParseAttribute(link, "title");
            if(ParseLink(link, ref url, ref caption, ref title))
            {
                return new GenericLinkItem() {Href=url, Title=title, Text=caption };
            }
            return new GenericLinkItem();
        }

        public static bool ParseLink(string link, ref string url, ref string caption, ref string title)
        {
            caption = ParseElementInner("a", link);
            url = ParseAttribute(link, "href");
            title = ParseAttribute(link, "title");

            return !string.IsNullOrEmpty(caption) && !string.IsNullOrEmpty(url);
        }

        public static GenericElementItem ParseGenericElement(string element,string tagName)
        {
            string innerHtml = null;
            string title = null;
            if (ParseGenericElelemnt(element, tagName, ref innerHtml, ref title))
            {
                return new GenericElementItem() { Value = innerHtml, Title = title };
            }
            return new GenericElementItem();
        }

        public static bool ParseGenericElelemnt(string element, string tagName, ref string innerHtml, ref string title)
        {
            innerHtml = ParseElementInner(tagName, element);
            title = ParseAttribute(element, "title");

            return !string.IsNullOrEmpty(innerHtml);
        }
        public static GenericElementItem ParseGenericInput(string element, string tagName)
        {
            string id = null;
            string value = null;
            string title = null;
            if (ParseGenericInput(element, tagName, ref value, ref id, ref title))
            {
                return new GenericElementItem() { Value = value, Title = title, Id = id };
            }
            return new GenericElementItem();
        }

        public static bool ParseGenericInput(string element, string tagName, ref string value, ref string id, ref string title)
        {
            id = ParseAttribute(element, "id");
            value = ParseAttribute(element, "value");
            title = ParseAttribute(element, "title");

            return !string.IsNullOrEmpty(id);
        }
        public static string ParseAttribute(string element, string attributeName)
        {

            string attrValue = "";
            string pattern = attributeName + "=\"(?<attr>.*?)\"";
            Match m = Regex.Match(element, pattern, RegexOptions.IgnoreCase);
            if (m.Success && m.Groups.Count > 1)
            {
                attrValue = m.Groups["attr"].Value; ;// m.Groups[1].Value;//.ToString();
            }
            return attrValue;
        }

        public static string ParseElementInner(string tagName, string html, string id, string separator)
        {
            string result = html;
            string pattern = "<"+tagName+".*?id=\"" + id + "\".*?>(?<body>.*?)</"+tagName+">" + separator;
            Match m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {
                result = m.Groups["body"].Value;
            }
            return result;
        }

        public static string ParseElementClassInner(string tagName, string html, string cssClass)
        {
            html = html + "<end>";
            string result = html;
            string pattern = "<"+tagName+".*?class=\"" + cssClass + "\".*?>(?<body>.*?)</"+tagName+">" + "<end>";// separator;
            Match m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {
                result = m.Groups["body"].Value;
                if (!result.EndsWith("</"+tagName+">"))
                {
                    result = result + "</"+tagName+">";
                }
            }
            return result;
        }

        public static string ParseDivSection(string html, string id)
        {
            string result = html;
            string pattern = "<div.*?id=\"" + id + "\".*?(?<attr>.*?).*?>";
            Match m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {
                result = "<div.*?id=\"" + id + "\" " + m.Groups["attr"].Value.Trim() + ">";
            }
            return result;
        }

        public static string ParseElementAttribute(string tagName, string html, string attribute)
        {
            string result = null;
            string pattern = "<" + tagName + ".*?" + attribute + "=\"(?<attr>.*?)\".*?>";
            Match m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {
                result = m.Groups["attr"].Value;
            }
            return result;
        }

        public static string ParseElementClass(string tagName, string html, string wclass)
        {

            string result = null;
            string pattern = "<" + tagName + ".*?class=\"" + wclass + "\".*?(?<attr>.*?).*?>";
            Match m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {
                result = m.Groups["attr"].Value;//group 1
            }
            return result;
        }

        public static string ParseElementClass(string tagName, string html)
        {
            string result = null;
            string pattern = "<" + tagName + ".*?class=\"(?<attr>.*?)\".*?>";
            Match m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {
                result = m.Groups["attr"].Value;//group 1
            }
            return result;
        }
        
        public static string ParseInnerItem(string tagName, string html, string css_class)
        {
            //string pattern = "<div\\stitle=\"Html\"*>(?<attr>.*?)</div>";
            html = html.Trim();
            string pattern = "<" + tagName + ".*?class=\"" + css_class + "\"*>";
            Match m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {

                //result = m.Groups["attr"].Value;
                html = Regex.Replace(html, pattern, "", RegexOptions.IgnoreCase);
                pattern = "</" + tagName + ">$";
                html = Regex.Replace(html, pattern, "", RegexOptions.IgnoreCase);
                //if (html.TrimEnd().ToLower().EndsWith("</div>"))
                //{
                //    html = html.Remove(html.Length - 6, 6);
                //}
            }

            return html;
        }
        
        #endregion

        #region load html

        public static string LoadHtml(Stream stream)
        {
            string content = "";
            using (StreamReader reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }
            return content;
        }

        public static string LoadHtml(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return LoadHtml(stream);
            }
        }

        public static string ToHtmlTable(DataTable table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"styledTable\" cellspacing=\"0\">");
            sb.AppendLine("<tr>");
            for (int i = 0; i <= table.Columns.Count - 1; i++)
            {
                sb.AppendLine("<td class=\"styledTableHeader\">" +
                table.Columns[i].ColumnName + "</td>");
            }
            sb.AppendLine("</tr>");
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                sb.AppendLine("<tr>");
                for (int j = 0; j <= table.Columns.Count - 1; j++)
                {
                    if (i % 2 == 0)
                        sb.AppendLine("<td class=\"styledTableRow\">" +
                        table.Rows[i][j].ToString() + "</td>");
                    else
                        sb.AppendLine("<td class=\"styledTableAltRow\">" +
                        table.Rows[i][j].ToString() + "</td>");
                }
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        public static string ToHtmlTable(DataTable table, string[] prefix, string[] controls)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"styledTable\" cellspacing=\"0\">");
            sb.AppendLine("<tr>");
            for (int i = 0; i <= table.Columns.Count - 1; i++)
            {
                sb.AppendLine("<td class=\"styledTableHeader\">" +
                table.Columns[i].ColumnName + "</td>");
            }
            sb.AppendLine("</tr>");
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                sb.AppendLine("<tr>");
                for (int j = 0; j <= table.Columns.Count - 1; j++)
                {
                    if (i % 2 == 0)
                        sb.AppendLine("<td class=\"styledTableRow\">" +
                       string.Format(controls[j], prefix[j] + j.ToString(), table.Rows[i][j].ToString()) + "</td>");
                    else
                        sb.AppendLine("<td class=\"styledTableAltRow\">" +
                        string.Format(controls[j], prefix[j] + j.ToString(), table.Rows[i][j].ToString()) + "</td>");
                }
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        public static string ToHtmlVerticalTable(DataRow row, string style)
        {
            DataTable table = row.Table;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table class=\"styledTable\" cellspacing=\"0\"{0}>", FormatStyle(style));
            sb.AppendLine();
            for (int i = 0; i <= table.Columns.Count - 1; i++)
            {
                DataColumn col = table.Columns[i];
                sb.AppendLine("<tr>");
                sb.AppendFormat("<td class=\"styledRowKey\">{0}</td>", string.IsNullOrEmpty(col.Caption) ? col.ColumnName : col.Caption);
                sb.AppendFormat("<td class=\"styledRowValue\">{0}</td>", row[col]);
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }

        public static string ToHtmlVerticalTable(IDictionary row, string style)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table class=\"styledTable\" cellspacing=\"0\"{0}>", FormatStyle(style));
            sb.AppendLine();
            foreach(DictionaryEntry entry in row)// (int i = 0; i <= table.Columns.Count - 1; i++)
            {
                sb.AppendLine("<tr>");
                sb.AppendFormat("<td class=\"styledRowKey\">{0}</td>", entry.Key);
                sb.AppendFormat("<td class=\"styledRowValue\">{0}</td>", entry.Value);
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }
        #endregion
    }
}
