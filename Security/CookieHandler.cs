using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Nistec.Web.Security
{
    public class CookieHandler
    {
        public static string Encode(string value)
        {

            if (string.IsNullOrEmpty(value))
                return value;
            return MachineKey.Encode(Encoding.UTF8.GetBytes(value), MachineKeyProtection.All);
            //return Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(value)));

        }

        public static string Decode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return Encoding.UTF8.GetString(MachineKey.Decode(value, MachineKeyProtection.All));
            //return Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(value)));
        }

        //some action method
        //Response.Cookies.Add(CreateCookie("","",60));
        public static HttpCookie CreateCookie(HttpContextBase context, string name, string value, int addMinutes, bool encrypt = true)
        {
            HttpCookie cookies = null;
            if (context.Response.Cookies[name] != null)
            {
                cookies = context.Response.Cookies[name];
                cookies.Value = encrypt ? Encode(value) : value;
                cookies.Expires = DateTime.Now.AddMinutes(addMinutes);
            }
            else
            {
                cookies = new HttpCookie(name);
                //cookies.Secure = true;
                cookies.Value = encrypt ? Encode(value) : value;
                cookies.Expires = DateTime.Now.AddMinutes(addMinutes);
                cookies.Domain = context.Request.Url.Host;
                context.Response.Cookies.Add(cookies);
            }
            return cookies;
        }

        public static void CreateCookies(HttpContextBase context, int addMinutes, bool encrypt, params string[] keyValueArgs)
        {

            int count = keyValueArgs.Length;
            if (count % 2 != 0)
            {
                throw new ArgumentException("values parameter not correct, Not match key value arguments");
            }
            for (int i = 0; i < count; i++)
            {
                string key = keyValueArgs[i].ToString();
                string value = keyValueArgs[++i];

                CreateCookie(context, key, value, addMinutes, encrypt);
            }
        }

        public static string GetCookieValue(HttpContextBase context, string name, bool encrypt = true)
        {
            string cookievalue = null;

            if (context.Request.Cookies[name] != null)
            {
                cookievalue = context.Request.Cookies[name].Value.ToString();
            }
            return encrypt ? Decode(cookievalue) : cookievalue;
        }

        public static string[] GetCookieValueSplited(HttpContextBase context, string name, int length, bool encrypt = true)
        {
            string[] cookievalues = null;

            if (context.Request.Cookies[name] != null)
            {
                string cookievalue = context.Request.Cookies[name].Value.ToString();
                if (encrypt)
                    cookievalue = Decode(cookievalue);

                if (cookievalue != null)
                {
                    cookievalues = cookievalue.Split('|');
                    if (cookievalues == null || cookievalues.Length != length)
                        return null;
                }
            }
            return cookievalues;
        }

        public static string RemoveCookieValue(HttpContextBase context, string name)
        {
            string cookievalue = null;

            if (context.Request.Cookies[name] != null)
            {
                context.Request.Cookies[name].Expires = DateTime.Now.AddDays(-1);
            }
            return cookievalue;
        }

    }
}
