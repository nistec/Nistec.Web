using Nistec.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nistec.Web.Controls
{
    public class TraceHelper<Dbc> where Dbc: IDbContext
    {
       
        /*
        public static string GetReferrer(HttpRequestBase Request)
        {
            if (Request == null)
                return "";

            var uri=Request.UrlReferrer;
            if (uri != null)
            {
                return uri.AbsoluteUri;
            }
            else
            {
                if (Request.ServerVariables.Count > 0)
                    return Request.ServerVariables["HTTP_HOST"];
                else
                    return "";
                //return Request.UserHostName ?? Request.UserHostAddress;
            }
            //string referer = Request.ServerVariables["HTTP_REFERER"];
            //if (string.IsNullOrEmpty(referer))
            //    return Request.ServerVariables["HTTP_HOST"]; ;
            //return referer;
        }
        public static string GetReferrer(HttpRequest Request)
        {
            if (Request == null)
                return "";
            var uri = Request.UrlReferrer;
            if (uri != null)
            {
                return uri.AbsoluteUri;
            }
            else
            {
                if (Request.ServerVariables.Count > 0)
                    return Request.ServerVariables["HTTP_HOST"];
                else
                    return "";
                //return Request.UserHostName ?? Request.UserHostAddress;
            }
            //string referer = Request.ServerVariables["HTTP_REFERER"];
            //if (string.IsNullOrEmpty(referer))
            //    return Request.ServerVariables["HTTP_HOST"]; ;
            //return referer;
        }
        */
        //public async Task<int> LogAsync(string folder, string Action, string LogText, string clientIp, string referrer, int LogType = 0)
        //{
        //    return await Task.Run(() => TraceHelper.Log(folder, Action, LogText, clientIp, referrer, LogType));
        //}

        public Task<int> LogAsync(string folder, string Action, string LogText, string clientIp, string referrer, int LogType = 0)
        {
            return Task.Factory.StartNew(() => Log(folder, Action, LogText, clientIp, referrer, LogType));
        }
        public static int Log(string folder, string Action, string LogText, string clientIp, string referrer, int LogType = 0)
        {
            try
            {
                using (var db = DbContext.Create<Dbc>())
                    return db.ExecuteNonQuery("sp_Log", "Folder", folder, "Action", Action, "LogText", LogText, "Client", clientIp, "Referrer", referrer, "LogType", LogType);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return -1;
            }
        }

        //public async static Task<int> LogAsync(string folder, string Action, string LogText, HttpRequestBase request, int LogType = 0)
        //{
        //    return await Task.Run(() => TraceHelper.Log(folder, Action, LogText, request, LogType));
        //}
        public static Task<int> LogAsync(string folder, string Action, string LogText, HttpRequestBase request, int LogType = 0)
        {
            return Task.Factory.StartNew(() => Log(folder, Action, LogText, request, LogType));
        }

        public static int Log(string folder, string Action, string LogText, HttpRequestBase request, int LogType = 0)
        {
            try
            {
                string referrer = HttpHelper.GetReferrer(request);
                string clientIp = HttpHelper.GetClientIP(request);// request == null ? "" : request.UserHostAddress;
                                                                  //if (request != null)
                                                                  //{
                                                                  //    clientIp = request.UserHostAddress;
                                                                  //    if (request.UrlReferrer != null)
                                                                  //        referrer = request.UrlReferrer.AbsoluteUri;
                                                                  //}
                using (var db = DbContext.Create<Dbc>())
                    return db.ExecuteNonQuery("sp_Log", "Folder", folder, "Action", Action, "LogText", LogText, "Client", clientIp, "Referrer", referrer, "LogType", LogType);
            }
            catch(Exception ex)
            {
                string err = ex.Message;
                return -1;
            }
        }
        public static int Log(string folder, string Action, string LogText, HttpRequest request, int LogType = 0)
        {
            try
            {
                string referrer = HttpHelper.GetReferrer(request);
                string clientIp = HttpHelper.GetClientIP(request);//request == null ? "" : request.UserHostAddress;
                                                                  //if (request != null)
                                                                  //{
                                                                  //    clientIp = request.UserHostAddress;
                                                                  //    if (request.UrlReferrer != null)
                                                                  //        referrer = request.UrlReferrer.AbsoluteUri;
                                                                  //}
                using (var db = DbContext.Create<Dbc>())
                    return db.ExecuteNonQuery("sp_Log", "Folder", folder, "Action", Action, "LogText", LogText, "Client", clientIp, "Referrer", referrer, "LogType", LogType);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return -1;
            }
        }

        //public static int GetAccountId()
        //{
        //    return Nistec.Generic.NetConfig.Get<int>("account", 0);
        //}

        //public static void TraceInfo(string message)
        //{
        //    try
        //    {
        //        Nistec.Trace.Log.Info(message);
        //    }
        //    catch { }
        //}
        //public static void TraceDebug(string message)
        //{
        //    try
        //    {
        //        Nistec.Trace.Log.Debug(message);
        //    }
        //    catch { }
        //}
        //public static void TraceError(string message,Exception exception)
        //{
        //    try
        //    {
        //        //int accountId = GetAccountId();
        //        //Nistec.Trace.TraceException.Trace(Nistec.Trace.TraceStatus.ApplicationException, accountId, exception);
        //        Nistec.Trace.Log.Exception(message,exception);
        //    }
        //    catch { }
        //}
        //public static void TraceError(string message)
        //{
        //    try
        //    {
        //        //int accountId = GetAccountId();
        //        //Nistec.Trace.TraceException.Trace(Nistec.Trace.TraceStatus.ApplicationException, accountId, message);
        //        Nistec.Trace.Log.Error(message);
        //    }
        //    catch { }
        //}
    }
}
