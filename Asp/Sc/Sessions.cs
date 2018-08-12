using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;

using Nistec;
using Nistec.Runtime;
//using Nistec.Caching.Remote;

namespace Nistec.Web.Asp
{
    
    public class SessionsAdmin
    {
        public const string AdminUAID = "AdminUAID";
        public const string AdminUserId = "AdminUserId";
        public const string AdminUserType = "AdminUserType";
        public const string AdminUserName = "AdminUserName";
        public const string AdminAccountId = "AdminAccountId";
        public const string AdminParentId = "AdminParentId";
        public const string AdminOwnerId = "AdminOwnerId";

        public static void SetActiveAdmin(Page p, UserAuth ua)
        {
            if (ua == null)
            {
                throw new ArgumentNullException("SessionsAdmin.UserAuth");
            }
            UserType ut = (UserType)ua.UserType;
            if (!(ut == UserType.Admin || ut == UserType.Manager))
            {
                throw new SessionException("Access denied!!!");
            }

            p.Session[AdminUserId] = ua.UserId;
            p.Session[AdminUserType] = (int)ut;
            p.Session[AdminUserName] = ua.UserName;
            p.Session[AdminAccountId] = ua.AccountId;
            p.Session[AdminParentId] = ua.ParentId;
            p.Session[AdminOwnerId] = ua.OwnerId;
            p.Session[AdminUAID] = string.Format("{0}-{1}-{2}-{3}", ua.UserId, ua.AccountId, (int)ut, p.Session.SessionID);
        }

        public static T Get<T>(Page p,string field, T defaultValue)
        {
            object o = p.Session[field];
            if (o == null)
            {
                return defaultValue;
            }
            return (T)o;
        }
        public static string Get(Page p, string field)
        {
            object o = p.Session[field];
            if (o == null)
            {
                return null;
            }
            return o.ToString();
        }

        //public static AccountRules GetAccountCapacity(Page p)
        //{
        //   // Page p =Sessions.CurrentPage;
        //    int accountId = Get<int>(p, SessionsAdmin.AdminAccountId, 0);
        //    return AccountRules_Context.Get(accountId);

        //    //AccountCapacity ac = new AccountCapacity()
        //    //{
        //    //    AccountId = Get<int>(p, SessionsAdmin.AdminAccountId, 0),
        //    //    ContactCapacity = 0,
        //    //    ContentCapacity = 0,
        //    //    EnvFolder = Get(p, SessionsAdmin.AdminEnvFolder)
        //    //};
        //    //return ac;
        //}

        public static void Quit(Page p)
        {
            p.Session[AdminUserId] = null;
            p.Session[AdminUserType] = null;
            p.Session[AdminUserId] = null;
            p.Session[AdminUserType] = null;
            p.Session[AdminUserType] = null;
            p.Session[AdminParentId] = null;
            p.Session[AdminOwnerId] = null;
            p.Session[AdminUAID] = null; 
            p.Session.Clear();
        }

        public static int GetAccountId(Page p)
        {
            return Get<int>(p,AdminAccountId,0);
        }
        public static int GetParentId(Page p)
        {
            return Get<int>(p, AdminParentId, 0);
        }
        public static int GetAdminOwnerId(Page p)
        {
            return Get<int>(p, AdminOwnerId, 0);
        }
        public static string GetUAID(Page p)
        {
            return Get(p, AdminUAID);
        }

        public static bool IsAdmin(Page p)
        {
            object o = p.Session[AdminUserType];
            if (o == null)
            {
                return false;
            }

            return Types.ToInt(o) ==(int) UserType.Admin;
        }

        public static bool IsManager(Page p)
        {
            object o = p.Session[AdminUserType];
            if (o == null)
            {
                return false;
            }

            return Types.ToInt(o) == (int)UserType.Manager;
        }

        public static bool IsManagerOrAdmin(Page p)
        {
            object o = p.Session[AdminUserType];
            if (o == null)
            {
                return false;
            }
            int i= Types.ToInt(o);
            return i == (int)UserType.Manager || i == (int)UserType.Admin;
        }

        public static void RedirectLogin()
        {
            //LogActiveError("RedirectLogin");
            HttpContext.Current.Response.Redirect("~/Admin/Login.aspx", true);
        }
        public static void RedirectLogin(Page p, bool endResponse)
        {
            //LogActiveError("RedirectLogin");
            HttpContext.Current.Response.Redirect("~/Admin/Login.aspx", endResponse);
        }

        public static void SessionAdminValidating(Page p)
        {
            if (!IsAdmin(p))
            {
                //LogActiveError("SessionAdminValidating");
                RedirectLogin(p,true);
            }
        }
        public static void SessionManagerValidating(Page p)
        {
            if (!IsManagerOrAdmin(p))
            {
                //LogActiveError("SessionAdminValidating");
                RedirectLogin(p, true);
            }
        }

    }
    
    /// <summary>
    /// Summary description for ActiveUser
    /// </summary>
    public class Sessions
    {

        public static Page GetCurrentPage()
        {
            Page p = CurrentPage;
            if (p == null)
            {
                throw new Exception("CurrentPage is invalid");
            }
            return p;
        }

        public static Page CurrentPage
        {
            get
            {
                return HttpContext.Current.Handler as Page;
            }
        }

        public static bool IsAdmin(Page p)
        {
            return p.GetSc().UserType == UserType.Admin;
        }

        public static bool IsManager(Page p)
        {
            ISc isc = p.GetSc();
            return isc.UserType == UserType.Manager || isc.UserType == UserType.Admin;
        }

        public static string CreateCa(Page p, string curAc)
        {
            int ca = Types.ToInt(curAc);

            return CreateCa(p, ca);
        }
        public static string CreateCa(Page p, int curAc)
        {
            ISc isc = p.GetSc();
            isc.ValidateScManager();
            //Sessions.SessionManagerValidating(p);
            int accountId = isc.AccountId;// p.GetSc(ScField.AID);
            if (curAc <= 0)
            {
                curAc = accountId;
            }
            return string.Format("{0}@{1}", accountId, curAc);
        }

#if (false)

        #region members

        public const string InnerMessage = "InnerMessage";

        #endregion

        #region CK

        public static Page GetCurrentPage()
        {
            Page p = CurrentPage;
            if (p == null)
            {
                throw new Exception("CurrentPage is invalid");
            }
            return p;
        }

        public static Page CurrentPage
        {
            get
            {
                return HttpContext.Current.Handler as Page;
            }
        }
       
     
        public static string CacheKey()
        {
            Page p = GetCurrentPage();

            if (p.Master == null)
                return p.GetSc().CacheKey;
            return p.Master.GetSc().CacheKey;
        }

      

        public static bool IsAdmin(Page p)
        {
            return p.GetSc().UserType == UserType.Admin;
        }

        public static bool IsManager(Page p)
        {
            ISc ick=p.GetSc();
            return ick.UserType == UserType.Manager || ick.UserType == UserType.Admin;
        }

        public static bool IsWL(Page p)
        {
            if (p == null)
            {
                p = CurrentPage;
                if (p == null)
                    return false;
            }
            return !string.IsNullOrEmpty(p.GetSc().WL);
        }

        //public static string GetLoginKey(Page p)
        //{
        //    return p.GetSc(ScField.UID).ToString();
        //}

        public static string GetLoginKeyManager(Page p)
        {
            SessionManagerValidating(p);
            return p.GetSc().UAID;
        }

        //public static int GetOwnerId(Page p)
        //{
        //    return p.GetSc().OwnerId;
        //}

        //public static int GetParentId(Page p)
        //{
        //    return p.GetSc().ParentId;
        //}

        public static AccountCapacity GetAccountCapacity(Page p)
        {
            int accountId = p.NcSession().Get(ScField.CurrentAccountId);
            //int accountId=GetAID(p);
            return new AccountCapacity(accountId);
        }

        public static string GetInnerMessage()
        {
            return string.Format("{0}", HttpContext.Current.Session[Sessions.InnerMessage]);
        }

        public static string GetAccountArg(Page p)
        {
            return Encryption.EncryptPass(p.GetSc( ScField.CA).ToString());
        }
        #endregion

        #region CA

        //public static int GetCa(Page p)
        //{
        //    return GetSc(p).GetCA();
        //}

        public static int GetCa(Page p, string ca)
        {
            int accountId = p.GetSc(ScField.AID);
            if (string.IsNullOrEmpty(ca))
            {

                return accountId;
            }
            int curac = 0;
            int accid = 0;
            if (Nistec.Generic.GenericArgs.SplitArgs<int, int>(ca, '@', ref accid, ref curac))
            {
                if (accid == accountId)
                {
                    return curac;
                }
            }
            return accountId;
        }

        public static string CreateCa(Page p, string curAc)
        {
            int ca = Types.ToInt(curAc);

            return CreateCa(p, ca);
        }
        public static string CreateCa(Page p, int curAc)
        {
            Sessions.SessionManagerValidating(p);
            int accountId = p.GetSc(ScField.AID);
            if (curAc <= 0)
            {
                curAc = accountId;
            }
            return string.Format("{0}@{1}", accountId, curAc);
        }

        public static bool TryGetCa(Page p, ref int curCa)
        {
            string ca = HttpContext.Current.Request.QueryString["ca"];
            int accountId = p.GetSc(ScField.AID);
            if (string.IsNullOrEmpty(ca))
            {
                curCa = accountId;
                return false;
            }
            int accid = 0;
            if (Nistec.Generic.GenericArgs.SplitArgs<int, int>(ca, '@', ref accid, ref curCa))
            {
                if (accid == accountId)
                {
                    return true;
                }
            }
            curCa = accountId;
            return false;
        }


        #endregion

        #region validating

        public static void RedirectLogin()
        {
            LogActiveError("RedirectLogin");
            HttpContext.Current.Response.Redirect("~/View/Login.aspx", true);

        }

        public static void SessionValidatingTesting(Page p)
        {
            LogActiveError("SessionValidatingTesting");

            //ClearAll(p);
            HtmlInputHidden ctl = (HtmlInputHidden)p.Master.FindControl("wlmaster");
            if (ctl == null)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            else if (ctl.Value == "" || ctl.Value == "view")
            {
                p.Response.Redirect(p.ResolveClientUrl("~/View/Login.aspx"), true);
            }
            else
            {
                p.Response.Redirect(p.ResolveClientUrl("~/WL/Login.aspx?wl=" + ctl.Value), true);
                //p.Response.Redirect(p.ResolveClientUrl("~/WL/" + ctl.Value + "/Login.aspx"), true);
            }
            //HttpContext.Current.Session[Sessions.AccountId] = null;
            //HttpContext.Current.Response.Redirect("~/View/Login.aspx", true);
        }

 
        public static void SessionValidating(Page p, string wl)
        {

            ISc ick = p.GetSc();
            if (ick == null)
            {
                if (!string.IsNullOrEmpty(wl))
                {
                    p.Response.Redirect("~/Wl/Login.aspx?wl=" + wl);
                }
                else
                {
                    HtmlInputHidden ctl = (HtmlInputHidden)p.Master.FindControl("wlmaster");
                    if (ctl == null)
                    {
                        FormsAuthentication.RedirectToLoginPage();
                    }
                    else if (ctl.Value == "" || ctl.Value == "view")
                    {
                        p.Response.Redirect(p.ResolveClientUrl("~/View/Login.aspx"), true);
                    }
                    else
                    {
                        p.Response.Redirect(p.ResolveClientUrl("~/WL/Login.aspx?wl=" + ctl.Value), true);
                    }
                }
            }
            ick.ValidateSc();
        }
        public static void SessionValidating(Page p)
        {
            ISc ick = p.GetSc();
            if (ick == null)
            {
                HtmlInputHidden ctl = (HtmlInputHidden)p.Master.FindControl("wlmaster");
                if (ctl == null)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
                else if (ctl.Value == "" || ctl.Value == "view")
                {
                    p.Response.Redirect(p.ResolveClientUrl("~/View/Login.aspx"), true);
                }
                else
                {
                    p.Response.Redirect(p.ResolveClientUrl("~/WL/Login.aspx?wl=" + ctl.Value), true);
                }
            }
            ick.ValidateSc();

        }

 
        public static void SessionAdminValidating(Page p)
        {
            if (!IsAdmin(p))
            {
                LogActiveError("SessionAdminValidating");

                p.Response.Redirect(p.ResolveClientUrl("~/View/Login.aspx"), true);
            }
        }
        public static void SessionManagerValidating(Page p)
        {
            if (p.GetSc().UserType < UserType.Manager)
            {
                LogActiveError("SessionManagerValidating");

                p.Response.Redirect(p.ResolveClientUrl("~/View/Login.aspx"), true);
            }
        }

        #endregion

        #region log

        public static string PrintActive()
        {
            Page p = CurrentPage;

            if (p == null)
            {
                return "CurrentSession is null";

            }
            ISc ick = p.GetSc();
            if (ick == null)
            {
                return "CurrentSc is null";
            }
            return ick.ToString();

        }

        public static void LogActiveError(string method)
        {

            //Log.ErrorFormat("{0} error:{1} ", method, PrintActive());
        }

        #endregion

        #region Redirect to

        public static void RedirectToError(string msg)
        {
            HttpContext.Current.Response.Redirect("~/Err.aspx?m=" + msg.Replace("\r\n", "<br/>"));
            //HttpContext.Current.Response.End();
        }
        public static void RedirectToErrorView(string msg)
        {
            if (IsWL(null))
                HttpContext.Current.Response.Redirect("~/WL/Err.aspx?m=" + msg.Replace("\r\n", "<br/>"), true);
            else
            {
                HttpContext.Current.Response.Redirect("~/View/Err.aspx?m=" + msg.Replace("\r\n", "<br/>"), true);
            }
            //HttpContext.Current.Response.Redirect("~/View/Err.aspx?m=" + msg.Replace("\r\n", "<br/>"));
            //HttpContext.Current.Response.End();
        }

        public static void RedirectToError()
        {
            RedirectToErrorView("Can not display the web page");
        }
        #endregion

        #region quit

        //public static void ClearAll(Page p)
        //{
        //    p.Session[Sessions.LoginId] = null;
        //    p.Session[Sessions.AccountId] = null;
        //    //p.Session[Sessions.ActiveUserPerms] = null;
        //    //RePerms(true);
        //}

        public static void Quit(Page p)
        {

            //string uid = GetUID(p);// Types.ToInt(p.Session[Sessions.LoginId], 0).ToString();
            string sessionId = p.GetSc().CacheKey;// p.Session.SessionID;

            //MCacheHelper.ClearAllSession(sessionId);
            //MCacheHelper.ClearAll(uid);

            //ClearAll(p);
            p.Session.Clear();
            RemoteSession.Instance(sessionId).RemoveSessionAsync();
        }

        public static void Quit()
        {
            Page p = GetCurrentPage();
            string sessionId = p.GetSc().CacheKey;
            p.Session.Clear();
            RemoteSession.Instance(sessionId).RemoveSessionAsync();

            //string uid = null;
            //string sessionId = null;
            //if (HttpContext.Current == null)
            //    return;
            //System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
            //if (session != null)
            //{
            //    sessionId = session.SessionID;
            //    uid = Types.ToInt(session[Sessions.LoginId], 0).ToString();
            //    session[Sessions.LoginId] = null;
            //    session[Sessions.AccountId] = null;

            //    //if (uid != null)
            //    //{
            //    //    MCacheHelper.ClearAll(uid);
            //    //}
            //    //MCacheHelper.ClearAllSession(sessionId);
            //    RemoteSession.Instance(sessionId).RemoveSessionAsync();
            //}
        }
        #endregion
#endif

    }

}