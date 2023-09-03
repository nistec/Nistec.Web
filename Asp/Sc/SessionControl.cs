using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;

using Nistec;
//using Nistec.Caching.Remote;
using Nistec.Runtime;
using Nistec.Generic;
//using Nistec.Channels.RemoteCache;

namespace Nistec.Web.Asp
{

    public class SessionControl : WebControl, ISc
    {
        #region ctor
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!this.Page.IsPostBack)
            {
                if (AutoReSign)
                {
                    ReSign();

                    //if (IsSignIn())
                    //{

                    //    Page.ViewStateUserKey = UAID;
                    //}
                }
            }
        }
        #endregion

        #region const
        //public const string SiteName = "EPHONE";
        //public const bool SecureLogin = true;
        internal const string TagUNAME = "UNAME";
        internal const string TagUPASS = "UPASS";
        internal const string TagULOG = "ULOG";
        internal const string TagUAID = "UAID";
        internal const string TagRSET = "RSET";
        
        public const int CacheExpiration = 0;
        public const int CacheActiveTimeout = 30;

        #endregion

        #region override properties

        public override bool EnableViewState
        {
            get { return true; }
            set { }
        }
        //public virtual string SiteName { get { return NetConfig.AppSettings["SiteName"]; } }

        //public abstract string OwnerName { get; set; }
        //public abstract string RedirectTo { get; set; }
        public virtual bool SecureLogin { get { return true; } }
        public virtual bool CheckConfirmArticle { get { return true; } set { } }
        protected virtual bool AutoReSign 
        { 
            get 
            {
                if (this.Page.Request.RawUrl.ToLower().Contains("login"))
                    return false;
                return true; 
            } 
        }

        ////public virtual string ConfirmArticleUrl
        ////{
        ////    get { return IsWL ? "~/WL/Index.aspx" : "~/Index.aspx"; }
        ////    set { }
        ////}
        //public virtual string IndexUrl
        //{
        //    get { return IsWL ? "~/WL/Index.aspx" : "~/Index.aspx"; }
        //}
        //public virtual string LoginUrl
        //{
        //    get { return IsWL ? "~/WL/Login.aspx" : "~/Login.aspx"; }
        //}
        //public virtual string SigninUrl
        //{
        //    get { return IsWL ? "~/WL/Login.aspx" : "~/Login.aspx"; }
        //}
        #endregion

        #region properties

        public bool IsSignIn
        {
            get
            {
                string s = (string)ViewState[TagUAID];
                return (s != null);
            }
        }

        public bool IsWL
        {
            get { return ScPath.IsWl(ENV); /*!string.IsNullOrEmpty(WL)*/; }
        }

        public int Get(ScField field)
        {
            switch (field)
            {
                case ScField.AID: return AccountId;
                case ScField.ATYPE: return (int)AccType;
                case ScField.PID: return ParentId;
                case ScField.ENV: return EnvId;
                case ScField.UID: return UserId;
                case ScField.UTYPE: return (int)UserType;
                case ScField.CurrentAccountId: return CurrentAccountId;
                case ScField.CurrentUserId: return CurrentUserId;
                case ScField.CA: return GetCA();
                case ScField.Culture: return (int)CultureUtil.GetCultue(CultureName);
                default:
                    return 0;
            }
        }

        public string GetField(ScField field)
        {
            switch (field)
            {
                case ScField.AID: return AccountId.ToString();
                case ScField.ATYPE: return AccType.ToString();
                case ScField.PID: return ParentId.ToString();
                case ScField.ENV: return EnvId.ToString();
                case ScField.UID: return UserId.ToString();
                case ScField.UTYPE: return UserType.ToString();
                case ScField.CurrentAccountId: return CurrentAccountId.ToString();
                case ScField.CurrentUserId: return CurrentUserId.ToString();
                case ScField.CA: return GetCA().ToString();
                case ScField.Culture: return CultureName;
                default:
                    return string.Empty;
            }
        }

        //public UserType UserType
        //{
        //    get { return (UserType)UTYPE; }
        //}
        //public AccountType AccType
        //{
        //    get { return (AccountType)ATYPE; }
        //}
        public bool IsAdminOrManager
        {
            get { return UserType == UserType.Admin || UserType == UserType.Manager; }
        }

        public override string ToString()
        {
            return string.Format("LoginId:{0},AccountId:{1},Env:{2},GetUserType:{3}",
                UserId,
                AccountId,
                ENV,
                UserType);

        }
        public string CacheKeyDependency(string currentKey, bool validateManager = false)
        {
            if (validateManager)
            {
                SessionManagerValidating();
            }
            if (string.IsNullOrEmpty(currentKey))
                return CacheKey + "-0";
            int ikey = Types.ToInt(currentKey);
            ikey++;

            return CacheKey + "-" + ikey.ToString();

            //if (currentKey.EndsWith("-A"))
            //    return CacheKey + "-B";
            //if (currentKey.EndsWith("-B"))
            //    return CacheKey + "-C";
            //else
            //    return CacheKey + "-A";
        }

        public string AdminKeyDependency(string currentKey)
        {
           
            SessionAdminValidating();

            if (string.IsNullOrEmpty(currentKey))
                return CacheKey + "-0";
            int ikey = Types.ToInt(currentKey);
            ikey++;

            return CacheKey + "-" + ikey.ToString();

            //if (string.IsNullOrEmpty(currentKey))
            //    return CacheKey + "-A";
            //if (currentKey.EndsWith("-A"))
            //    return CacheKey + "-B";
            //if (currentKey.EndsWith("-B"))
            //    return CacheKey + "-C";
            //else
            //    return CacheKey + "-A";
        }

        public void SessionAdminValidating()
        {
            if (!(UserType == UserType.Admin))
            {
                ScPath.RedirectToLogin(this.Page, ENV);
            }
        }
        public void SessionManagerValidating()
        {
            if (!IsAdminOrManager)
            {
                ScPath.RedirectToLogin(this.Page, ENV);
            }
        }
        public int AccountManager
        {
            get { return AccountId; }
        }

        //public int CurrentAccountId
        //{
        //    get
        //    {
        //        int aid = ActiveAccId;
        //        return aid > 0 ? aid : AccountId;
        //    }
        //}
        //public int CurrentUserId
        //{
        //    get
        //    {
        //        int uid = ActiveUserId;
        //        return uid > 0 ? uid : UserId;
        //    }
        //}

        //public int ParentId
        //{
        //    get { return PID; }
        //}
        //public int OwnerId
        //{
        //    get { return (AccType == AccountType.Owner) ? PID : 0; }
        //}

        public string CacheKey
        {
            get
            {
                if (ActiveUserId > 0)
                {
                    return ActiveUAID;
                }
                return UAID;
            }
        }
        public int CurrentAccountId
        {
            get
            {
                int accId = ActiveAccId;
                return (accId > 0) ? accId : AccountId;
            }
        }
        public int CurrentUserId
        {
            get
            {
                int userId = ActiveUserId;
                return (userId > 0) ? userId : UserId;
            }
        }

        public int GetAccParentId()
        {
            if (UserType == UserType.Admin)
                return 100;
            if (AccType == AccountType.Parent)
                return GetCA();
            return ParentId;
        }

        #endregion

        #region view state properties


        string GetUAID()
        {
            if (_UAID == null)
            {
                string s = (string)ViewState[TagUAID];
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
                    string s = (string)ViewState[TagUAID];
                    _UAID = ((s == null) ? null : s);
                }
                return _UAID;
            }
            set
            {
                ViewState[TagUAID] = value;
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

        #region Cache properties

        string _CultureName;
        public string CultureName
        {
            get
            {
                if (string.IsNullOrEmpty(_CultureName))
                {
                    //ValidateSc(); ;
                    //_CultureName = Types.NZ(ViewState["Culture"], "he");

                    _CultureName = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<string>(UAID, "CULTURE", "he");

                }
                return _CultureName;
            }
            set
            {
                //ViewState["Culture"] = value;

                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "CULTURE", value,0);
            }
        }

        UserAuth _UserInfo;
        public UserAuth UserInfo
        {
            get
            {
                if (_UserInfo == null)
                {
                    ValidateSc();
                    _UserInfo = UserAuthId_Context.Get(UserId);
                }
                return _UserInfo;
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
                    _AccountName = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<string>(UAID, "ANAME");

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
                    _UserName = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<string>(UAID,"UNAME");
                }
                return _UserName;
            }
            internal set
            {
                _UserName = value;
                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "UNAME", value, CacheExpiration);
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
                    _ActiveAccId = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<int>(UAID, "AAID");
                }
                return _ActiveAccId;
            }
            set
            {
                _ActiveAccId = value;
                RemoteCacheApi.Session(ScPath.CacheProtocol).Set(UAID, "AAID", value, CacheExpiration);
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
                    _ActiveUserId = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<int>(UAID, "AUID");
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
                    _ActiveUAID = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<string>(UAID, "AUAID");
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
                    _FilesCapacity = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<int>(UAID, "FilesCapacity");
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
                    _ContactCapacity = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<int>(UAID, "ContactCapacity");
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
                    _RootFolder = RemoteCacheApi.Session(ScPath.CacheProtocol).Get<string>(UAID, "RootFolder");
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
        //            if (!IsSignIn)
        //                return this.Page.Request.QueryString["wl"];
        //            _WL = SessionCacheApi.Get(ScPath.CacheProtocol).Get(UAID,"WL");
        //        }
        //        return _WL;
        //    }
        //    set
        //    {
        //        _WL = value;
        //        SessionCacheApi.Get(ScPath.CacheProtocol).Set(UAID,"WL",value);
        //    }
        //}

        //public string GetWl()
        //{
        //    if (_WL == null)
        //    {
        //        if (!IsSignIn)
        //            return this.Page.Request.QueryString["wl"];
        //        _WL = SessionCacheApi.Get(ScPath.CacheProtocol).Get(UAID,"WL");
        //    }
        //    return _WL;
        //}
        #endregion
        
        #region Resign

        public void ValidateSc()
        {
            object o = ViewState[TagUAID];
            if (o == null)
            {
                ReSign();
            }
        }

        public void ValidateScManager()
        {
            object o = ViewState[TagUAID];
            if (o == null)
            {
                ReSign();
            }
            if (!(UserType == Asp.UserType.Manager))
            {
                ReSign();
            }
        }

        public void ReSign()
        {
            HttpCookie cookie =null;

            try
            {
                cookie = GetCookie();
                if (cookie == null)
                {

                    ScPath.RedirectToLogin(this.Page, ENV, false);
                    return;
                }
            }
            catch (NotSupportedException)
            {
                ScPath.RedirectToErrCookie(this.Page, ENV, false);
                return;
            }
            catch (Exception)
            {
                ScPath.RedirectToErr(this.Page, ENV, false, null);
                return;
            }

            try
            {
               
                string uaid = Encryption.DecryptPass(string.Format("{0}", cookie[TagUAID]));

                UAID = uaid;
                //UserType ut = UserType;
                if (string.IsNullOrEmpty(uaid))
                {
                    ScPath.RedirectToLogin(this.Page, ENV);
                }

                RemoteCacheApi.Session(ScPath.CacheProtocol).Refresh(uaid);
            }
            catch (Exception)
            {
                ScPath.RedirectToErr401(this.Page, ENV, false);
            }
        }

        public void RedirectToIndex()
        {
            this.Page.Response.Redirect(ScPath.IndexUrl(ENV));
        }

        public void RedirectToLogIn()
        {
            this.Page.Response.Redirect(ScPath.LoginUrl(ENV));
        }

        //public void RedirectToSignIn()
        //{
        //    this.Page.Response.Redirect(ScPath.SigninUrl(IsWL));
        //}
        #endregion

        #region Signin

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

            string uaid = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", au.LoginId, au.AccountId, au.UserType, au.AccType, au.ParentId, au.OwnerId, au.OwnerId);
            return uaid;
        }

        public static void SignIn(UserAuth au, string uaid)
        {
            SignIn(au, uaid, CacheExpiration);
        }

        public static void SignIn(UserAuth au, string uaid, int timeout)
        {
            if (uaid == null)
            {
                uaid = GetUAIDKey(au);
            }
            //RemoteSession rs = new RemoteSession(uaid);
            RemoteCacheApi.Session(ScPath.CacheProtocol).CreateSession(uaid, timeout, au.CacheArgs());
            RemoteCacheApi.Session(ScPath.CacheProtocol).Set(uaid, "ANAME", au.AccountName,CacheExpiration);
        }

        public bool SignIn(string loginName, string pass, bool rememberMeSet)
        {

            try
            {
                UserAuth au = UserAuth_Context.Get(loginName, pass);
                if (au.IsEmpty)
                {
                    //Sessions.SessionLog(SessionType.Logfailed, loginName + ":" + pass);

                    ScPath.RedirectToLogin(this.Page, ENV);
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

                if (au.Evaluation < 0)
                {
                    ScPath.RedirectToSignIn(this.Page, ENV);
                    return false;
                }


                if (CheckConfirmArticle && !au.ConfirmArticle)
                {

                    ScPath.RedirectToConfirmArticleUrl(this.Page, ENV, au.UserId);
                    return false;
                }
                else
                {


                    SetCooki(au.LogInName, au.Pass, uaid, rememberMeSet);
                    ClearActiveCache(true);
                    RemoteCacheApi.Session(ScPath.CacheProtocol).CreateSession(uaid, CacheExpiration, au.CacheArgs());
                    AccountName = au.AccountName;
                    UserName = au.UserName;

                    SetWl(au.OwnerFolder);


                    //FormsAuthentication.RedirectFromLoginPage("Index.aspx", false);

                    //string dash = (!isWl && au.ShowDashboard) ? "?dash=1" : "";

                    ScPath.RedirectToIndex(this.Page, ENV);
                    return true;
                }


            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return false;
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
                cooki[TagUNAME] = loginName;
                cooki[TagUPASS] = Encryption.EncryptPass(secure);
            }
            else
            {
                cooki[TagUNAME] = Encryption.EncryptPass(loginName);
                cooki[TagUPASS] = Encryption.EncryptPass(pass);
            }
            cooki[TagRSET] = rememberMeSet ? "1" : "0";
            cooki[TagUAID] = Encryption.EncryptPass(uaid);
            cooki[TagULOG] = DateTime.Now.ToString();
            cooki.Expires = DateTime.Now.AddDays(60);
            this.Page.Response.Cookies.Add(cooki);

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

        //protected UserInfo VerifyLogin(Page p, HttpCookie cookies)
        //{
        //    if (cookies == null || cookies.Values.Count == 0)
        //        return false;
        //    //if (cookies.Expires < DateTime.Now)
        //    //    return false;
        //    string login = Encryption.DecryptPass(string.Format("{0}", cookies[UNAME]));
        //    string pass = Encryption.DecryptPass(string.Format("{0}", cookies[UPASS]));


        //    if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
        //        return null;

        //    if (SecureLogin)
        //    {
        //        try
        //        {
        //            string[] args = pass.Split(';', '|');
        //            login = args[0];
        //            pass = args[1];
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }


        //    return VerifyLogin(p, login, pass);

        //}


        #endregion

        #region cache methods

        //public void Refresh()
        //{
        //    string ck = GetUAID();
        //    if (ck != null)
        //    {
        //        RemoteSession.Instance.KeepAliveSession(UAID, true);
        //    }
        //}

        void ClearActiveCache(bool removeSession)
        {
            int activeUid = ActiveUserId;
            if (activeUid > 0)
            {
                string activeUAID = ActiveUAID;
                //SessionApi.RemoveSession(activeUAID);
                if (removeSession)
                    RemoteCacheApi.Session(ScPath.CacheProtocol).RemoveSession(activeUAID);
                else
                    RemoteCacheApi.Session(ScPath.CacheProtocol).ClearItems(activeUAID);
                //ActiveAccId = 0;
                //ActiveUserId = 0;
                //ActiveUAID = null;
            }
            else
            {
                if (removeSession)
                {
                    RemoteCacheApi.Session(ScPath.CacheProtocol).RemoveSession(UAID);
                }
            }
        }

        void ClearCache()
        {
            //if (CookiUtil.Current.Session != null)
            //{
            //    MCacheHelper.Remote.RemoveSession(CookiUtil.Current.Session.SessionID);
            //}

            RemoteCacheApi.Session(ScPath.CacheProtocol).RemoveSession(UAID);
        }
        #endregion

        #region cookies

        private HttpCookie GetCookie()
        {
            HttpCookie cookies = null;
            if (this.Page.Request.Browser.Cookies)
            {
                cookies = this.Page.Request.Cookies[ScPath.SiteName];
                //if (cookies == null || cookies.Values.Count == 0)
                //{
                //    //throw new Exception("Invalid Cooki");
                //    //Response.Redirect(LoginUrl);

                //    string sPagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                //    if (sPagePath.Contains("LogIn"))
                //       ScPath.JsToErr401(this.Page); //JS.Redirect(this.Page, ResolveClientUrl(ErrUrl) + "?m=unauthorized");
                //    else
                //        ScPath.RedirectToLogin(this.Page,false);// JS.Redirect(this.Page, ResolveClientUrl(LoginUrl));
                    
                //}
            }
            else
            {
                throw new NotSupportedException("Cooki Is not supported");
            }
            return cookies;
        }
        public void DeleteCookie()
        {
            // Set the value of the cookie to null and
            // set its expiration to some time in the past
            this.Page.Response.Cookies[ScPath.SiteName].Value = null;
            this.Page.Response.Cookies[ScPath.SiteName].Expires =
                System.DateTime.Now.AddMonths(-1); // last month
        }

        #endregion

        #region Login /logout

        public void Logoff()
        {
            if (this.Page.Request.Browser.Cookies)
            {
                //HttpCookie cookies = p.Request.Cookies[SiteName];
                ////ok = VerifyLogin(Request.Cookies[SiteName]);
                //if (cookies == null || cookies.Values.Count == 0)
                //    return;
                //cookies.Expires = DateTime.Now.AddDays(-1);
                //p.Response.Cookies.Remove(SiteName);

                if (this.Page.Request.Cookies[ScPath.SiteName] != null)
                {
                    HttpCookie cookies = new HttpCookie(ScPath.SiteName);
                    cookies.Expires = DateTime.Now.AddDays(-1d);
                    this.Page.Response.Cookies.Add(cookies);
                }
            }
        }

        #endregion

        #region manager login\logout

        public int GetCA()
        {
            //SessionValidating(p);
            string ca = Page.Request.QueryString["ca"];
            int aid = CurrentAccountId;
            if (string.IsNullOrEmpty(ca))
            {
                return aid;
            }
            int curac = 0;
            int accid = 0;
            if (Nistec.Generic.GenericArgs.SplitArgs<int, int>(ca, '@', ref accid, ref curac))
            {
                if (accid == aid)
                {
                    return curac;
                }
            }
            return aid;
        }

        public bool SetActiveAccount(int accId, string accName, string rootFolder)
        {
            try
            {
                //int loginId = this.UserId;// Sessions.GetLoginId();
                //UserAuth au = UserAuthId_Context.GetByManager(userId, loginId);

                //int activeUid = ActiveUserId;
                //if (activeUid > 0)
                //{
                //    string activeUAID = ActiveUAID;
                //    SessionCache.ClearAll(activeUAID, false);
                //}


                ActiveAccId = accId;
                //ActiveUserId = au.UserId;
                //ActiveUAID = GetUAIDKey(au);
                AccountName = accName;
                RootFolder = rootFolder;
                //SignIn(au, ActiveUAID, CacheActiveTimeout);
                return true;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                //Log.ErrorFormat("SetActiveManagerLogin error:{0}", ex.Message);

                //p.Session[Sessions.UType] = 0;

                //throw ex;

                return false;
            }
        }
 
        public bool SetActiveManagerLogin(int userId)
        {
            try
            {
                int loginId = this.UserId;// Sessions.GetLoginId();
                UserAuth au = UserAuthId_Context.GetByManager(userId, loginId);

                int activeUid = ActiveUserId;
                if (activeUid > 0)
                {
                    string activeUAID = ActiveUAID;
                    RemoteCacheApi.Session(ScPath.CacheProtocol).ClearItems(activeUAID);
                }


                ActiveAccId = au.AccountId;
                ActiveUserId = au.UserId;
                ActiveUAID = GetUAIDKey(au);
                AccountName = au.AccountName;
                SignIn(au, ActiveUAID, CacheActiveTimeout);
                return true;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                //Log.ErrorFormat("SetActiveManagerLogin error:{0}", ex.Message);

                //p.Session[Sessions.UType] = 0;

                //throw ex;

                return false;
            }
        }
        public bool SetActiveManagerLogout()
        {
            try
            {
                UserAuth au = UserAuthId_Context.Get(UserId);
                if (au.IsEmpty)
                {
                    return false;
                }
                ClearActiveCache(false);

                ActiveAccId = 0;
                ActiveUserId = 0;
                ActiveUAID = null;
                AccountName = au.AccountName;

                return true;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                //Log.ErrorFormat("SetActiveManagerLogout error:{0}", ex.Message);

                //p.Session[Sessions.UType] = 0;

                //throw ex;

                return false;
            }
        }

        public void SetActiveAccount(UserAuth au)
        {
            try
            {
                //int activeUid = ActiveUserId;
                //if (activeUid > 0)
                //{
                //    string activeUAID = ActiveUAID;
                //    MCacheHelper.ClearAll(activeUAID, true, false);
                //}

                ClearActiveCache(false);

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
        #endregion


    }

}
