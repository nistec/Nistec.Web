//#define TEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

using Nistec.Web;

namespace Nistec.Web
{

    public enum BrowserType
    {
        Default=0,
        Mobile=1,
        Tablet=2
    }

    /// <summary>
    /// Summary description for PF
    /// </summary>
    public class BrowserHelper
    {

        public static string GetBrowserVersion(Page p)
        {
            return GetBrowserVersion(p.Request);
        }

        public static string GetBrowserVersion(HttpRequest request)
        {
            if (request.Browser.IsMobileDevice)
            {
                string version = "";
                if (GetMobileBrowserVersion(request.UserAgent, out version))
                    return version;
                return GetBrowserVersion(request.UserAgent);

            }
            return GetBrowserVersion(request.UserAgent);
        }

/*
0	NA	NA
1	Nokia	Nokia
2	BlackBerry	BlackBerry
3	iPhone	iPhone
4	SonyEricsson	SonyEricsson
5	LG	LG
6	Android	Android
7	MOT	Motorola
8	SAMSUNG	Samsung
9	iPod	iPod
10	iPad	iPad
11	MSIE	IE
12	Chrome	Chrome
13	Opera	Opera
14	Firefox	Firefox
15	Safari	Safari
16	DoCoMo	DoCoMo
17	IEMobile	IEMobile
18	Toshiba	Toshiba
19	Palm	Palm
20	HTC	HTC
NULL	NULL	NULL
*/
        
        public static bool IsMobileBrowser(HttpRequest request)
        {

            //if (request.Browser.IsMobileDevice)
            //    return true;
            string browser = "";
            return GetBrowser(request.UserAgent, out browser) == BrowserType.Mobile;
        }

        public static BrowserType GetBrowser(HttpRequest request, out string browser)
        {
            return GetBrowser(request.UserAgent,out browser);
        }

        public static BrowserType GetBrowser(string UA, out string browser)
        {
            string lUA = UA.ToLower();

            int i = 0;
            i = lUA.IndexOf("nokia");
            if (i != -1) { browser = "Nokia"; return BrowserType.Mobile; }
            i = lUA.IndexOf("blackberry");
            if (i != -1) { browser = "BlackBerry"; return BrowserType.Mobile;}
            i = lUA.IndexOf("iphone");
            if (i != -1) { browser = "iPhone"; return BrowserType.Mobile;}
            i = lUA.IndexOf("sonyericsson");
            if (i != -1) { browser = "SonyEricsson"; return BrowserType.Mobile;}
            i = UA.IndexOf("LG");
            if (i != -1) { browser = "LG"; return BrowserType.Mobile;}
            i = UA.IndexOf("MOT-");
            if (i != -1) { browser = "MOT"; return BrowserType.Mobile;}
            i = UA.IndexOf("SAMSUNG");
            if (i != -1) { browser = "SAMSUNG"; return BrowserType.Mobile;}
            i = lUA.IndexOf("ipod");
            if (i != -1) { browser = "iPod"; return BrowserType.Mobile;}
            i = lUA.IndexOf("ipad");
            if (i != -1) { browser = "iPad"; return BrowserType.Tablet;}
            i = UA.IndexOf("DoCoMo");
            if (i != -1) { browser = "DoCoMo"; return BrowserType.Mobile;}
            i = UA.IndexOf("Toshiba");
            if (i != -1) { browser = "Toshiba"; return BrowserType.Mobile;}
            i = UA.IndexOf("HTC");
            if (i != -1) { browser = "HTC"; return BrowserType.Mobile;}
            i = lUA.IndexOf("toshiba");
            if (i != -1) { browser = "Toshiba"; return BrowserType.Mobile;}
            i = lUA.IndexOf("palm");
            if (i != -1) { browser = "Palm"; return BrowserType.Mobile;}

            i = lUA.IndexOf("iemobile");
            if (i != -1) { browser = "IEMobile"; return BrowserType.Mobile;}
            i = lUA.IndexOf("android");
            if (i != -1) { browser = "Android"; return BrowserType.Mobile;}


            i = lUA.IndexOf("opera");
            if (i != -1) {  browser = "Opera"; return BrowserType.Default;}
            i = lUA.IndexOf("firefox");
            if (i != -1) {  browser = "Firefox"; return BrowserType.Default;}
            i = lUA.IndexOf("chrome");
            if (i != -1) {  browser = "Chrome"; return BrowserType.Default; }

            i = lUA.IndexOf("safari");
            if (i != -1) {   browser ="Safari"; return BrowserType.Default;}
            i = lUA.IndexOf("msie");
            if (i != -1) {   browser ="MSIE"; return BrowserType.Default;}
            
            browser= "NA";
                return BrowserType.Default;

            }





        public static string GetBrowserVersion(string UA)
        {

            string version = "";
            //string UA = request.UserAgent;
            string ua = UA.ToLower();

            bool isOpera = UA.ToLower().IndexOf("opera") != -1;
            bool isIE = !isOpera && (UA.ToLower().IndexOf("msie") != -1);

            if (isIE)
            {
                //Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)

                if (ua.IndexOf("msie 8") != -1)
                    version = "IE-8.0";
                else if (ua.IndexOf("msie 7") != -1)
                    version = "IE-7.0";
                else if (ua.IndexOf("msie 6") != -1)
                    version = "IE-6.0";
                else if (ua.IndexOf("msie 5") != -1)
                    version = "IE-5.0";
                else if (ua.IndexOf("msie 4") != -1)
                    version = "IE-4.0";
                //nav = "IE";
            }
            else
            {
                int i = -1;


                //Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.32 Safari/532.0
                i = ua.IndexOf("chrome");
                if (i != -1)
                {
                    //nav = "Chrome";
                    version = UA.Substring(i);
                    return version;
                }
                i = ua.IndexOf("safari");
                if (i != -1)
                {
                    //nav = "Safari";
                    version = UA.Substring(i);
                    return version;
                }
                //Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.12) Gecko/2009070611 Firefox/3.0.12 (.NET CLR 3.5.30729)
                i = ua.IndexOf("firefox");
                if (i != -1)
                {
                    //nav = "Firefox";
                    version = UA.Substring(i);
                    return version;
                }
                //Opera/9.64 (Windows NT 5.1; U; en) Presto/2.1.1
                i = ua.IndexOf("opera");
                if (i != -1)
                {
                    //nav = "Opera";
                    version = UA.Substring(i);
                    return version;
                }
                i = ua.IndexOf("netscape");
                if (i != -1)
                {
                    //nav = "Netscape";
                    version = UA.Substring(i);
                    return version;
                }
            }

            return version;
        }

        

        public static bool GetMobileBrowserVersion(string UA,out string version)
        {

            //string version = "";
            //string UA = request.UserAgent;

            int i = -1;
            string ua = UA.ToLower();

            i = ua.IndexOf("iphone");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = ua.IndexOf("ipod");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = ua.IndexOf("android");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = ua.IndexOf("blackberry");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = ua.IndexOf("samsung");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = ua.IndexOf("docomo");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = UA.IndexOf("LG");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = UA.IndexOf("HTC");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = ua.IndexOf("sonyericsson");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = UA.IndexOf("SIE-");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = ua.IndexOf("toshiba");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = ua.IndexOf("motorola");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = ua.IndexOf("philips");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            i = ua.IndexOf("mozilla");
            if (i != -1)
            {
                version = UA.Substring(i);
                return true;
            }
            version = "";
            return false;
        }

    }
}
