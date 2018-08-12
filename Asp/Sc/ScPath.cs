using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Nistec;
using Nistec.Generic;
using Nistec.Channels;

namespace Nistec.Web.Asp
{
    public class ScPath
    {
        public static NetProtocol CacheProtocol = NetProtocol.Tcp;

        public const string InfoPath = "~/Info.aspx";
        public const string ErrPath = "~/Err.aspx";

        public const string Err401="unauthorized";

        public static string EnvName { get { return NetConfig.AppSettings["EnvName"]; } }

        public static string SiteName { get { return NetConfig.AppSettings["SiteName"]; } }

        public static string LoginPath { get { return NetConfig.AppSettings["LoginPath"]; } }

        public static string SigninPage { get { return NetConfig.AppSettings["SigninPage"]; } }

        public static string ViewPath { get { return NetConfig.AppSettings["ViewPath"]; } }

        public static string WlPath { get { return NetConfig.AppSettings["WlPath"]; } }

        public static string RootPath { get { return "~/"; } }


        public static bool IsWl(string env)
        {
            return !(Types.NzOr(env, EnvName) == EnvName);
        }

        public static string GetEnv(string env)
        {
            return Types.NzOr(env, EnvName) == EnvName ? ViewPath : WlPath;
        }

        public static string GetEnvArgs(string env)
        {
            return Types.NzOr(env, EnvName) == EnvName ? "" : "?wl=" + env;
        }
        public static string GetEnvArgs(string env, string m)
        {
            return Types.NzOr(env, EnvName) == EnvName ? "?m=" + m : "?wl=" + env + "&m=" + m;
        }
        public static string LoginUrl(bool IsWL)
        {
            return (IsWL ? WlPath : RootPath) + "Login.aspx";
        }
        public static string SigninUrl(bool IsWL)
        {
            return (IsWL ? WlPath : RootPath) + SigninPage;
        }
        public static string ErrUrl(bool IsWL)
        {
            return (IsWL ? WlPath : RootPath) + "Err.aspx";
        }

        public static string IndexUrl(bool IsWL)
        {
            return (IsWL ? WlPath : ViewPath) + "Index.aspx";
        }


        /*
        public static string ConfirmArticleUrl(bool IsWL)
        {
            return IsWL ? "~/WL/ArticleInfo.aspx" : "~/ArticleInfo.aspx";
        }
        public static string IndexUrl(bool IsWL)
        {
            return IsWL ? "~/WL/Index.aspx" : "~/Index.aspx";
        }
        public static string LoginUrl(bool IsWL)
        {
            return IsWL ? "~/WL/Login.aspx" : "~/Login.aspx";
        }
        public static string InfoUrl(bool IsWL, bool isView)
        {
            return IsWL ? "~/WLInfo.aspx" : "~/View/Info.aspx";
        }
        public static string ErrUrl(bool IsWL, bool isView)
        {
            return IsWL ? "~/WL/Err.aspx" : "~/View/Err.aspx";
        }
        public static string SigninUrl(bool isWl)
        {
            return isWl ? "~/WL/Login.aspx" : "~/Login.aspx";
        }
        */

        #region WL

        public static string ConfirmArticleUrl(string env, int userId)
        {
            string args= Types.NzOr(env, EnvName) == EnvName ? "?u=" + userId.ToString() : "?wl=" + env + "&u=" + userId.ToString();
 
            return GetEnv(env) + "ArticleInfo.aspx" + args;
        }
        public static string IndexUrl(string env)
        {
            return GetEnv(env) + "Index.aspx" + GetEnvArgs(env);
        }
        public static string LoginUrl(string env)
        {
            return GetEnv(env) + "Login.aspx" + GetEnvArgs(env);
        }
        public static string SigninUrl(string env)
        {
            return GetEnv(env) + SigninPage + GetEnvArgs(env);
        }
        public static string InfoUrl(string env)
        {
            return GetEnv(env) + "Info.aspx" + GetEnvArgs(env); 
        }

        public static string ErrUrl(string env, bool isView)
        {
            return GetEnv(env) + "Err.aspx" + GetEnvArgs(env); 
        }

        public static string ErrUrl(string env, bool isView, string m)
        {
            string args= Types.NzOr(env, EnvName) == EnvName ? "?m=" + m : "?wl=" + env + "&m=" + m;

            return GetEnv(env) + "Err.aspx" + args; 
        }

        //public static string SigninUrl(string Wl)
        //{
        //    return string.IsNullOrEmpty(Wl) ? "~/Login.aspx" : "~/WL/Login.aspx?wl=" + Wl;
        //}

        #endregion

        #region Redirect
        /*
        public static void RedirectToIndex(Page p, bool isWl)
        {
            p.Response.Redirect(IndexUrl(isWl));
        }
        public static void RedirectToLogin(Page p, bool isWl)
        {
            p.Response.Redirect(LoginUrl(isWl));
        }
        public static void RedirectToSignIn(Page p, bool isWl)
        {
            p.Response.Redirect(ScPath.SigninUrl(isWl));
        }
        public static void RedirectToConfirmArticleUrl(Page p, bool isWl, int userId)
        {
            p.Response.Redirect(ScPath.ConfirmArticleUrl(isWl) + "?u=" + userId.ToString());
        }
        public static void RedirectToErr(Page p, bool isWl, bool isView, string m)
        {
            p.Response.Redirect(ErrUrl(isWl, isView) + "?m=" + m);
        }
        public static void RedirectToErrCookie(Page p, bool isWl)
        {
            p.Response.Redirect(ErrUrl(isWl, false) + "?m=cookie");
        }
        public static void RedirectToErr401(Page p, bool isWl)
        {
            p.Response.Redirect(ErrUrl(isWl, false) + "?m=" + Err401);
        }
        */
        #endregion

        #region Redirect WL

        public static void RedirectToIndex(Page p, string env, bool endResponse = true)
        {
            string url = IndexUrl(env);
            p.Response.Redirect(url, endResponse);
        }

        public static void RedirectToLogin(Page p, string env, bool endResponse = true)
        {
            //if (p.Request.RawUrl.ToLower().Contains("login.aspx"))
            //    return;
            p.Response.Redirect(LoginUrl(env), endResponse);
        }

        public static void RedirectToLogin(Page p, bool endResponse = true)
        {
            string env = p.Request.QueryString["wl"];
            //if (p.Request.RawUrl.ToLower().Contains("login.aspx"))
            //    return;
            p.Response.Redirect(LoginUrl(env), endResponse);
        }

        public static void RedirectToSignIn(Page p, string Wl, bool endResponse = true)
        {
            // if (p.Request.RawUrl.ToLower().Contains("login.aspx"))
            //     return;
            p.Response.Redirect(ScPath.SigninUrl(Wl), endResponse);
        }

        public static void RedirectToConfirmArticleUrl(Page p, string env, int userId)
        {
            p.Response.Redirect(ScPath.ConfirmArticleUrl(env, userId));
        }

        public static void RedirectToErr(Page p, string env, bool isView, string m, bool endResponse = true)
        {
            //if (p.Request.RawUrl.ToLower().Contains("err.aspx"))
            //    return;
            p.Response.Redirect(ErrUrl(env, isView, m), endResponse);
        }

        public static void RedirectToErrCookie(Page p, string env, bool endResponse = true)
        {
           // if (p.Request.RawUrl.ToLower().Contains("err.aspx"))
            p.Response.Redirect(ErrUrl(env, false, "cookie"), endResponse);
        }

        public static void RedirectToErr401(Page p, string env, bool endResponse = true)
        {
           // if (p.Request.RawUrl.ToLower().Contains("err.aspx"))
            p.Response.Redirect(ErrUrl(env, false, Err401), endResponse);
        }
        #endregion

        /*
        public static void JsToLogin(Page p, bool isWl)
        {
            JS.Redirect(p, p.ResolveClientUrl(LoginUrl(isWl)));
        }
        public static void JsToErr401(Page p, bool isWl)
        {
            JS.Redirect(p, p.ResolveClientUrl(ErrUrl(isWl, false)) + "?m=" + Err401);
        }
        */

        public static void JsToLogin(Page p, string env)
        {
            p.Response.Redirect(LoginUrl(env));
        }

        public static void JsToErr401(Page p, string env)
        {
            p.Response.Redirect(ErrUrl(env, false, Err401));
        }

        public static void JsToLogin(Page p)
        {
            string Wl = p.Request.QueryString["wl"];
            p.Response.Redirect(LoginUrl(Wl));
        }

        public static void JsToErr401(Page p)
        {
            string Wl = p.Request.QueryString["wl"];
            p.Response.Redirect(ErrUrl(Wl, false, Err401));
        }
    }
}
