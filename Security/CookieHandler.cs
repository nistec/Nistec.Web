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

        public static HttpCookie UpsertCookieValues(string name, IDictionary<string, object> keyValueDictionary, int addMinutes, string cookieDomain = "host", bool encrypt = true, bool httpOnly = true)
        {

            var context = HttpContext.Current;

            HttpCookie cookie = Create(context, name);

            if (keyValueDictionary == null)
                cookie.Value = null;
            else
                foreach (var kvp in keyValueDictionary)
                {
                    string val = kvp.Value == null ? "" : kvp.Value.ToString();
                    if (encrypt)
                        cookie.Values.Set(kvp.Key, Encode(val));
                    else
                        cookie.Values.Set(kvp.Key, val);
                }

            return UpsertComplete(cookie, context, addMinutes, cookieDomain, httpOnly);
        }

        public static HttpCookie UpsertCookieValues(string name, string[] keyValueArgs, int addMinutes, string cookieDomain = "host", bool encrypt = true, bool httpOnly = true)
        {

            var context = HttpContext.Current;

            HttpCookie cookie = Create(context, name);

            if (keyValueArgs == null)
                cookie.Value = null;
            else
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
                    if (encrypt)
                        cookie.Values.Set(key, Encode(value));
                    else
                        cookie.Values.Set(key, value);
                }
            }

            return UpsertComplete(cookie, context, addMinutes, cookieDomain, httpOnly);
        }

        public static HttpCookie UpsertCookieValues(string name, string key, string value, int addMinutes, string cookieDomain = "host", bool encrypt = true, bool httpOnly = true)
        {

            var context = HttpContext.Current;

            HttpCookie cookie = Create(context, name);

            if (encrypt)
                cookie.Values.Set(key, Encode(value));
            else
                cookie.Values.Set(key, value);

            return UpsertComplete(cookie, context, addMinutes, cookieDomain, httpOnly);
        }

        public static HttpCookie UpsertCookieValues(string name, string value, int addMinutes, string cookieDomain = "host", bool encrypt = true, bool httpOnly = true)
        {

            var context = HttpContext.Current;

            HttpCookie cookie = Create(context, name);

            if (encrypt)
                cookie.Value= Encode(value);
            else
                cookie.Value=value;

            return UpsertComplete(cookie, context,  addMinutes,  cookieDomain , httpOnly);
           
        }

        static HttpCookie Create(HttpContext context, string name)
        {
            //var context = HttpContext.Current;

            // NOTE: we have to look first in the response, and then in the request.
            // This is required when we update multiple keys inside the cookie.
            HttpCookie cookie = context.Response.Cookies[name]
                ?? context.Request.Cookies[name];

            if (cookie == null)
            {
                cookie = new HttpCookie(name);

                if (context.Request.IsSecureConnection)
                    cookie.Secure = true;
            }
            return cookie;
        }

        static HttpCookie UpsertComplete(HttpCookie cookie, HttpContext context, int addMinutes, string cookieDomain = "host",  bool httpOnly = true) {

            cookie.Expires = DateTime.Now.AddMinutes(addMinutes);
            if (!String.IsNullOrEmpty(cookieDomain))
            {
                if (cookieDomain == "host")
                {
                    if (!context.Request.IsLocal)
                        cookie.Domain = context.Request.Url.Host;
                }
                else{
                    cookie.Domain = cookieDomain;
                }
            }
            if (httpOnly) cookie.HttpOnly = true;
            context.Response.Cookies.Add(cookie);
            return cookie;
        }

        public static string GetCookieValue(string cookieName, string keyName=null, bool encrypt = true)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                string val = (!String.IsNullOrEmpty(keyName)) ? cookie[keyName] : cookie.Value;
                if (!String.IsNullOrEmpty(val))
                {
                    string value = Uri.UnescapeDataString(val);
                    if (encrypt)
                        return Decode(value);
                    return value;
                }
            }
            return null;
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

        //=======================================
        /// <summary>
        /// Checks if a cookie / key exists in the current HttpContext.
        /// </summary>
        public static bool CookieExist(string cookieName, string keyName)
        {
            HttpCookieCollection cookies = HttpContext.Current.Request.Cookies;
            return (String.IsNullOrEmpty(keyName))
                ? cookies[cookieName] != null
                : cookies[cookieName] != null && cookies[cookieName][keyName] != null;
        }
        /// <summary>
        /// Removes a single value from a cookie or the whole cookie (if keyName is null)
        /// </summary>
        public static void RemoveCookie(string cookieName, string keyName, string domain = null)
        {
            if (String.IsNullOrEmpty(keyName))
            {
                if (HttpContext.Current.Request.Cookies[cookieName] != null)
                {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
                    cookie.Expires = DateTime.UtcNow.AddYears(-1);
                    if (!String.IsNullOrEmpty(domain)) cookie.Domain = domain;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    HttpContext.Current.Request.Cookies.Remove(cookieName);
                }
            }
            else
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
                cookie.Values.Remove(keyName);
                if (!String.IsNullOrEmpty(domain)) cookie.Domain = domain;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        /// <summary>
        /// Retrieves a single value from Request.Cookies
        /// </summary>
        public static string GetFromCookie(string cookieName, string keyName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                string val = (!String.IsNullOrEmpty(keyName)) ? cookie[keyName] : cookie.Value;
                if (!String.IsNullOrEmpty(val)) return Uri.UnescapeDataString(val);
            }
            return null;
        }
        /// <summary>
        /// Stores multiple values in a Cookie using a key-value dictionary, 
        ///  creating the cookie (and/or the key) if it doesn't exists yet.
        /// </summary>
        public static void StoreInCookie(
            string cookieName,
            string cookieDomain,
            Dictionary<string, string> keyValueDictionary,
            DateTime? expirationDate,
            bool httpOnly = false)
        {
            // NOTE: we have to look first in the response, and then in the request.
            // This is required when we update multiple keys inside the cookie.
            HttpCookie cookie = HttpContext.Current.Response.Cookies[cookieName]
                ?? HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null) cookie = new HttpCookie(cookieName);
            if (keyValueDictionary == null || keyValueDictionary.Count == 0)
                cookie.Value = null;
            else
                foreach (var kvp in keyValueDictionary)
                    cookie.Values.Set(kvp.Key, kvp.Value);
            if (expirationDate.HasValue) cookie.Expires = expirationDate.Value;
            if (!String.IsNullOrEmpty(cookieDomain)) cookie.Domain = cookieDomain;
            if (httpOnly) cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        /// <summary>
        /// Stores a value in a user Cookie, creating it if it doesn't exists yet.
        /// </summary>
        public static void StoreInCookie(
            string cookieName,
            string cookieDomain,
            string keyName,
            string value,
            DateTime? expirationDate,
            bool httpOnly = false)
        {
            // NOTE: we have to look first in the response, and then in the request.
            // This is required when we update multiple keys inside the cookie.
            HttpCookie cookie = HttpContext.Current.Response.Cookies[cookieName]
                ?? HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null) cookie = new HttpCookie(cookieName);
            if (!String.IsNullOrEmpty(keyName)) cookie.Values.Set(keyName, value);
            else cookie.Value = value;
            if (expirationDate.HasValue) cookie.Expires = expirationDate.Value;
            if (!String.IsNullOrEmpty(cookieDomain)) cookie.Domain = cookieDomain;
            if (httpOnly) cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Set(cookie);
        }
    }
}
