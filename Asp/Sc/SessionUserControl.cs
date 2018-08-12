using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Nistec;
using Nistec.Runtime;
using Nistec.Channels.RemoteCache;
using Nistec.Channels;
//using Nistec.Caching.Remote;

namespace Nistec.Web.Asp
{
  
    public abstract class SessionUserControl : System.Web.UI.UserControl//, ISc
    {
        #region const
       
        public const string Version = "v=4";
        public const int CacheExpiration = 0;
        
        #endregion

        #region override properties

        public override bool EnableViewState
        {
            get { return false; }
            set { }
        }
 
        public abstract bool SecureLogin { get; }
        public abstract bool CheckConfirmArticle { get; set; }
        protected virtual bool AutoReSign { get { return true; } }

        #endregion

        #region view state properties

       
        public bool IsSignIn
        {
            get
            {
                string s = (string)ViewState[SessionControl.TagUAID];
                return (s != null);
            }
        }
        public bool IsWL
        {
            get { return ScPath.IsWl(ENV); /*!string.IsNullOrEmpty(WL)*/; }
        }

        //public bool IsWl()
        //{
        //    string s = WL;
        //    return !string.IsNullOrEmpty(s);
        //}
        string GetUAID()
        {
            if (_UAID == null)
            {
                string s = (string)ViewState["UAID"];
                _UAID = ((s == null) ? null : s);
            }
            return _UAID;
        }

        string _UAID;
        public string UAID
        {
            get
            {
                if (_UAID == null)
                {
                    ValidateSc();
                    string s = (string)ViewState["UAID"];
                    _UAID = ((s == null) ? null : s);
                }
                return _UAID;
            }
            set
            {
                ViewState["UAID"] = value;
                if (value != null)
                {
                    //int uid = 0;
                    //int aid = 0;
                    //int userType = 0;
                    //int accType = 0;
                    ////int iparent = 0;
                    //int env = 0;

                    //Nistec.Generic.GenericArgs.SplitArgs<int, int, int, int, int>(value, '-', ref uid, ref aid, ref userType, ref accType, ref env);
                    //ViewState["UID"] = uid;
                    //ViewState["AID"] = aid;
                    //ViewState["UTYPE"] = userType;
                    //ViewState["ATYPE"] = accType;
                    ////ViewState["PID"] = iparent;
                    //ViewState["ENV"] = env;

                    string[] args = value.Split('-');
                    if (args.Length > 6)
                    {

                        ViewState["UID"] = args[0];
                        ViewState["AID"] = args[1];
                        ViewState["UTYPE"] = args[2];
                        ViewState["ATYPE"] = args[3];
                        ViewState["PID"] = args[4];
                        ViewState["ENVID"] = args[5];
                        ViewState["ENV"] = args[6];
                    }

                }
            }
        }

        int _UID;
        public int UserId
        {
            get
            {
                if (_UID == 0)
                {
                    ValidateSc();
                    _UID = Types.ToInt(ViewState["UID"], 0);
                }
                return _UID;
            }
        }
        int _AID;
        public int AccountId
        {
            get
            {
                if (_AID == 0)
                {
                    ValidateSc(); ;
                    _AID = Types.ToInt(ViewState["AID"], 0);
                }
                return _AID;
            }
        }
        int _UTYPE;
        public UserType UserType
        {
            get
            {
                if (_UTYPE == 0)
                {
                    ValidateSc(); ;
                    _UTYPE = Types.ToInt(ViewState["UTYPE"], 0);
                }
                return (UserType)_UTYPE;
            }
        }
        int _ATYPE;
        public AccountType AccType
        {
            get
            {
                if (_ATYPE == 0)
                {
                    ValidateSc(); ;
                    _ATYPE = Types.ToInt(ViewState["ATYPE"], 0);
                }
                return (AccountType)_ATYPE;
            }
        }
        int _PID;
        public int ParentId
        {
            get
            {
                if (_PID == 0)
                {
                    ValidateSc(); ;
                    _PID = Types.ToInt(ViewState["PID"], 0);
                }
                return _PID;
            }
        }
        int _ENVID;
        public int EnvId
        {
            get
            {
                if (_ENVID == 0)
                {
                    ValidateSc(); ;
                    _ENVID = Types.ToInt(ViewState["ENVID"], 0);
                }
                return _ENVID;
            }
        }
        string _ENV;
        public string ENV
        {
            get
            {
                if (string.IsNullOrEmpty(_ENV))
                {
                    ValidateSc(); ;
                    _ENV = Types.NZ(ViewState["ENV"], ScPath.EnvName);
                }
                return _ENV;
            }
            private set
            {
                ViewState["ENV"] = value;
            }
        }

        #endregion

        #region cache properties

        string _CultureName;
        public string CultureName
        {
            get
            {
                if (string.IsNullOrEmpty(_CultureName))
                {
                    //ValidateSc(); ;
                    _CultureName = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<string>(UAID, "CULTURE", "he");
                }
                return _CultureName;
            }
            set
            {
                //ViewState["Culture"] = value;

                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "CULTURE", value, CacheExpiration);
            }
        }
        string _AccountName;
        public string AccountName
        {
            get
            {
                if (_AccountName == null)
                {
                    //ValidateSc();
                    //return UserName + ", " + AccountId.ToString() + ", " + AccountName+", "+ LastLoggedIn;
                    _AccountName = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<string>(UAID, "ANAME",null);

                    //string s = (string)ViewState["ANAME"];
                    //_AccountName = ((s == null) ? "" : s);
                }
                return _AccountName;
            }
            internal set
            {
                _AccountName = value;
                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "ANAME", value, CacheExpiration);
            }
        }

        string _UserName;
        public string UserName
        {
            get
            {
                if (_UserName == null)
                {
                    _UserName = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<string>(UAID, "UNAME", null);
                }
                return _UserName;
            }
            internal set
            {
                _UserName = value;
                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "UNAME", value,CacheExpiration);
            }
        }
        int _ActiveAccId;
        public int ActiveAccId
        {
            get
            {
                if (_ActiveAccId == 0)
                {
                    //ValidateSc(); ;
                    _ActiveAccId = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<int>(UAID, "AAID",0);
                }
                return _ActiveAccId;
            }
            set
            {
                _ActiveAccId = value;
                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "AAID", value,CacheExpiration);
            }
        }
        int _ActiveUserId;
        public int ActiveUserId
        {
            get
            {
                if (_ActiveUserId == 0)
                {
                    //ValidateSc(); 
                    _ActiveUserId = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<int>(UAID, "AUID",0);
                }
                return _ActiveUserId;
            }
            set
            {
                _ActiveUserId = value;
                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "AUID", value, CacheExpiration);
            }
        }
        string _ActiveUAID;
        public string ActiveUAID
        {
            get
            {
                if (_ActiveUAID == null)
                {
                    _ActiveUAID = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<string>(UAID, "AUAID",null);
                }
                return _ActiveUAID;
            }
            internal set
            {
                _ActiveUAID = value;
                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "AUAID", value, CacheExpiration);
            }
        }

        int _FilesCapacity;
        public int FilesCapacity
        {
            get
            {
                if (_FilesCapacity == 0)
                {
                    _FilesCapacity = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<int>(UAID, "FilesCapacity",0);
                }
                return _FilesCapacity;
            }
            internal set
            {
                _FilesCapacity = value;
                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "FilesCapacity", value, CacheExpiration);
            }
        }

        int _ContactCapacity;
        public int ContactCapacity
        {
            get
            {
                if (_ContactCapacity == 0)
                {
                    _ContactCapacity = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<int>(UAID, "ContactCapacity",0);
                }
                return _ContactCapacity;
            }
            internal set
            {
                _ContactCapacity = value;
                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "ContactCapacity", value, CacheExpiration);
            }
        }
        string _RootFolder;
        public string RootFolder
        {
            get
            {
                if (_RootFolder == null)
                {
                    _RootFolder = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<string>(UAID, "RootFolder", null);
                }
                return _RootFolder;
            }
            internal set
            {
                _RootFolder = value;
                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "RootFolder", value, CacheExpiration);
            }
        }

        //string _WL;
        //public virtual string WL
        //{
        //    get
        //    {
        //        if (_WL == null)
        //        {
        //            if (IsSignIn)
        //                return this.Page.Request.QueryString["wl"];
        //            _WL = CacheApi.Session.Get(UAID,"WL");
        //        }
        //        return _WL;
        //    }
        //    set
        //    {
        //        _WL = value;
        //        CacheApi.Session.Set(UAID,"WL"value);
        //    }
        //}

        #endregion
                
        #region SignIn

        public static string GetUAIDKey(UserAuth au)
        {
            //int iParent = 0;
            //switch ((AccountType)au.AccType)
            //{
            //    case AccountType.Owner:
            //        iParent = au.EnvId; break;
            //    case AccountType.Parent:
            //        iParent = au.ParentId; break;
            //}

            string uaid = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", au.LoginId, au.AccountId, au.UserType, au.AccType, au.ParentId,au.OwnerId, au.OwnerFolder);
            return uaid;
        }

        //for login control
        protected bool SignIn(string loginName, string pass, bool rememberMeSet)
        {

            try
            {
                UserAuth au = UserAuth_Context.Get(loginName, pass);
                if (au.IsEmpty)
                {
                    //Winbox.Alert(this.Page, "au.IsEmpty");
                    //Sessions.SessionLog(SessionType.Logfailed, loginName + ":" + pass);

                    //RedirectToLogIn();
                    return false;
                }
                return SignIn(au, rememberMeSet);

            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return false;
            }
        }

        public bool SignIn(UserAuth au, bool rememberMeSet)
        {

            try
            {
                //UserInfo au = new UserInfo(loginName, pass);
                //if (au.IsEmpty)
                //{
                //    //Sessions.SessionLog(SessionType.Logfailed, loginName + ":" + pass);

                //    RedirectToLogIn();
                //    return false;
                //}

                string uaid = GetUAIDKey(au);

                UAID = uaid;
                
                SetCooki(au.LogInName, au.Pass, uaid, rememberMeSet);
                ClearActiveCache(true);
                RemoteCacheApi.Session(ScPath.CacheProtocol).CreateSession(uaid, CacheExpiration, au.CacheArgs());
                
                //AccountName = au.AccountName;
                //UserName = au.UserName;

                SetActiveAccount(au);

                SetWl(au.OwnerFolder);
                
                //FormsAuthentication.RedirectFromLoginPage("Index.aspx", false);

                //string dash = (!isWl && au.ShowDashboard) ? "?dash=1" : "";

                if (au.Evaluation < 0)
                {
                    RedirectToSignIn();
                    return false;
                }
                else if (CheckConfirmArticle && !au.ConfirmArticle)
                {

                    //Session[Sessions.ActiveUserInfo] = au;
                    //Response.Redirect(ConfirmArticleUrl + "?u=" + au.UserId.ToString(), false);
                    ScPath.RedirectToConfirmArticleUrl(this.Page, ENV, au.UserId);
                    return false;
                }
                //ScPath.RedirectToIndex(this.Page, ENV);

                string url= ScPath.IndexUrl(ENV);
                //Winbox.Redirect(this.Page, ResolveClientUrl(url));

                Response.Redirect(url);
                //JS.Redirect(this.Page, ResolveClientUrl(HomeUrl));
                return true;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return false;
            }
        }

        public void SetActiveAccount(UserAuth au)
        {
            try
            {
 
                //ClearActiveCache(false);

                AccountName = au.AccountName;
                UserName = au.UserName;
                ActiveAccId = au.AccountId;
                ActiveUserId = au.UserId;
                ActiveUAID = GetUAIDKey(au);
                RootFolder = au.RootFolder;
                FilesCapacity = au.FilesCapacity;
                ContactCapacity = au.ContactCapacity;

            }
            catch (Exception ex)
            {
                string s = ex.Message;
                //Log.ErrorFormat("SetActiveAccount error:{0}", ex.Message);

                //p.Session[Sessions.UType] = 0;

                throw ex;
            }

        }

        protected void SetCooki(string loginName, string pass, string uaid, bool rememberMeSet)
        {
            //if (rememberMeSet)
            //{
            HttpCookie cooki = new HttpCookie(ScPath.SiteName);

            if (SecureLogin)
            {
                string secure = loginName + ";" + pass;
                cooki[SessionControl.TagUNAME] = loginName;
                cooki[SessionControl.TagUPASS] = Encryption.EncryptPass(secure);
            }
            else
            {
                cooki[SessionControl.TagUNAME] = Encryption.EncryptPass(loginName);
                cooki[SessionControl.TagUPASS] = Encryption.EncryptPass(pass);
            }
            cooki[SessionControl.TagRSET] = rememberMeSet ? "1" : "0";
            cooki[SessionControl.TagUAID] = Encryption.EncryptPass(uaid);
            cooki[SessionControl.TagULOG] = DateTime.Now.ToString();
            cooki.Expires = DateTime.Now.AddDays(60);
            Response.Cookies.Add(cooki);

            //}
            //else
            //{
            //    Response.Cookies[SiteName].Value = "";
            //}
        }


        private void SetWl(string wl)
        {
            ENV = null;
            bool isWl = !Types.IsEmpty(wl) && wl != "Default";
            if (isWl)
            {
                ENV = wl;
                HtmlInputHidden ctl = (HtmlInputHidden)this.Page.Master.FindControl("wlmaster");
                if (ctl != null)
                {
                    ctl.Value = wl;
                }
            }
        }

        #endregion

        #region Resign

        public void ReSign()
        {
            HttpCookie cookie = GetCookie();
            if (cookie == null)
            {
                ScPath.RedirectToLogin(this.Page, ENV);
            }

            string uaid = Encryption.DecryptPass(string.Format("{0}", cookie[SessionControl.TagUAID]));

            UAID = uaid;
            //UserType ut = UserType;
            if (string.IsNullOrEmpty(uaid))
            {
                //Response.Redirect(LoginUrl);
                ScPath.RedirectToLogin(this.Page, ENV);
            }

            RemoteCacheApi.Session(ScPath.CacheProtocol).Refresh(uaid);
        }

        public void RedirectToHome()
        {
            //Response.Redirect(HomeUrl);
            ScPath.RedirectToIndex(this.Page, ENV);
        }

        public void RedirectToLogIn()
        {
            //Response.Redirect(LoginUrl);
            ScPath.RedirectToLogin(this.Page, ENV);
        }

        public void RedirectToSignIn()
        {
            //Response.Redirect(SigninUrl);
            ScPath.RedirectToSignIn(this.Page, ENV);
        }
        public void ValidateSc()
        {
            object o = ViewState[SessionControl.TagUAID];
            if (o == null)
            {
                ReSign();
            }
        }
        #endregion

        #region private methods

        private HttpCookie GetCookie()
        {
            HttpCookie cookies = null;
            if (Request.Browser.Cookies)
            {
                cookies = Request.Cookies[ScPath.SiteName];
                if (cookies == null || cookies.Values.Count == 0)
                {
                    //throw new Exception("Invalid Cooki");
                    //Response.Redirect(LoginUrl);
                    //JS.Redirect(this.Page, ResolveClientUrl(LoginUrl));

                    ScPath.JsToLogin(this.Page, ENV);
                }
            }
            else
            {
                throw new Exception("Cooki Is not supported");
            }
            return cookies;
        }

        void ClearActiveCache(bool removeSession)
        {

            string activeUAID = ActiveUAID;
            if (!string.IsNullOrEmpty(activeUAID))
            {
                if (removeSession)
                    RemoteCacheApi.Session(ScPath.CacheProtocol).RemoveSession(activeUAID);
                else
                    RemoteCacheApi.Session(ScPath.CacheProtocol).ClearItems(activeUAID);
            }
            //int activeUid = ActiveUserId;
            //if (activeUid > 0)
            //{
            //    string activeUAID = ActiveUAID;
            //    MCacheHelper.ClearAll(activeUAID, true, true);

            //    ActiveAccId = 0;
            //    ActiveUserId = 0;
            //    ActiveUAID = null;
            //}
            else
            {
                if (removeSession)
                {
                    RemoteCacheApi.Session(ScPath.CacheProtocol).RemoveSession(UAID);
                }
            }
        }
        #endregion
    }

}

