using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web;
using Nistec.Logging;
using Nistec.Data;

namespace Nistec.Web.Security
{
    public class FormsAuth : IAuthentication
    {
        private readonly ShellSettings _settings;
        //private readonly IClock _clock;
        //private readonly IContentManager _contentManager;
        private readonly IHttpContextAccess _httpContextAccess;
        private ISignedUser _signedInUser;
        private bool _isAuthenticated;

        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        public static ISignedUser GetCurrent(UserDataVersion version)
        {
            var form = new FormsAuth();
            return form.GetAuthenticatedUser(version);
        }
        public static ISignedUser GetCurrent(IHttpContextAccess httpContextAccess, UserDataVersion version)
        {
            var form = new FormsAuth(httpContextAccess);
            return form.GetAuthenticatedUser(version);
        }
        //public static IUser GetCurrent(IHttpContextAccess httpContextAccess, UserDataVersion version)
        //{
        //    var form = new FormsAuth(httpContextAccess);
        //    return form.GetAuthenticatedUser(version);
        //}

        public static bool IsCurrentAuthenticated(IHttpContextAccess httpContextAccess, UserDataVersion version)
        {
            var form = new FormsAuth(httpContextAccess);
            return form.IsAuthenticatedUser(version);
        }

        public static FormsAuth Instance { get { return new FormsAuth(); } }

        //internal for GetAuthenticatedUser
        private FormsAuth(IHttpContextAccess httpContextAccess)
        {
            if (httpContextAccess == null)
                httpContextAccess = new HttpContextAccess();
            _httpContextAccess = httpContextAccess;

            this.Log = Logger.Instance;
            ExpirationTimeSpan = TimeSpan.FromDays(30);
        }


        public FormsAuth()
        {
            _settings = new ShellSettings();
            _httpContextAccess = new HttpContextAccess();
            this.Log = Logger.Instance;
            ExpirationTimeSpan = TimeSpan.FromDays(30);
        }

        public FormsAuth (HttpContext httpContext)
        {
           _httpContextAccess = new HttpContextAccess(httpContext); 
            this.Log = Logger.Instance;
            ExpirationTimeSpan = TimeSpan.FromDays(30);
        }
        

        public FormsAuth(ShellSettings settings, IHttpContextAccess httpContextAccess)
        {
            if (settings == null)
                settings = new ShellSettings();
            if (httpContextAccess == null)
                httpContextAccess = new HttpContextAccess();

            _settings = settings;
            _httpContextAccess = httpContextAccess;

            this.Log = Logger.Instance;

            ExpirationTimeSpan = TimeSpan.FromDays(30);
        }

        //public FormsAuthenticationService(ShellSettings settings, IClock clock, IContentManager contentManager, IHttpContextAccessor httpContextAccessor)
        //{
        //    _settings = settings;
        //    _clock = clock;
        //    _contentManager = contentManager;
        //    _httpContextAccessor = httpContextAccessor;

        //    Logger = NullLogger.Instance;

        //    ExpirationTimeSpan = TimeSpan.FromDays(30);
        //}

        public ILogger Log { get; set; }

        public TimeSpan ExpirationTimeSpan { get; set; }

        public static bool DoSignIn(string loginName, string pass, bool createPersistentCookie, bool enableException)
        {
            FormsAuth form = new FormsAuth(new HttpContextAccess());// null);
            return form.SignIn(loginName, pass, createPersistentCookie, enableException);
        }

        public static AuthState DoSignIn(string loginName, string pass, UserDataVersion version, bool createPersistentCookie=true)
        {
            FormsAuth form = new FormsAuth(new HttpContextAccess());// null);
            return form.SignIn(loginName, pass, version, createPersistentCookie);
        }

        public static AuthState DoSignIn(string loginName, string pass, UserDataVersion version, bool createPersistentCookie, string HostClient, string HostReferrer, string AppName,bool IsMobile)
        {
            FormsAuth form = new FormsAuth(new HttpContextAccess());// null);
            return form.SignIn(loginName, pass, version, createPersistentCookie, HostClient, HostReferrer, AppName, IsMobile);
        }

        public AuthState SignIn(string loginName, string pass, UserDataVersion version, bool createPersistentCookie, string HostClient=null, string HostReferrer=null, string AppName=null, bool? IsMobile=null)
        {
            try
            {
                var user = Authorizer.Login(loginName, pass, HostClient, HostReferrer, AppName, IsMobile);
                if (user == null)
                {
                    Log.Error("User not Authenticated");
                    return  AuthState.UnAuthorized;
                }
                if (!user.IsAuthenticated)
                {
                    Log.Error("User not Authenticated");
                    return AuthState.UnAuthorized;
                }
                user.SetUserDataEx(version);
                SignIn(user, createPersistentCookie);
                return (AuthState)user.State;//. IsAuthenticated;
            }
            catch (Exception ex)
            {
                Log.Exception("SignIn error ", ex);
                return  AuthState.Failed;
            }
        }

        public ISignedUser SignInUser<T>(string loginName, string pass, UserDataVersion version, bool createPersistentCookie, string HostClient = null, string HostReferrer = null, string AppName = null, bool? IsMobile = null) where T:ISignedUser
        {
            try
            {
                var user = Authorizer.Login<T>(loginName, pass, HostClient, HostReferrer, AppName, IsMobile);
                if (user == null)
                {
                    Log.Error("User not Authenticated");
                    throw new SecurityException(AuthState.UnAuthorized, "User not Authenticated");
                }
                if (!user.IsAuthenticated)
                {
                    Log.Error("User not Authenticated");
                    throw new SecurityException(AuthState.UnAuthorized, "User not Authenticated");
                }
                //user.Data = UserDataContext.GetUserDataEx(user.AccountId,user.UserId);
                user.SetUserDataEx(version);
                SignIn(user, createPersistentCookie);
                return user;// (AuthState)user.State;//. IsAuthenticated;
            }
            catch (SecurityException sex)
            {
                throw sex;
            }
            catch (Exception ex)
            {
                Log.Exception("SignIn error ", ex);
                //return AuthState.Failed;
                throw new SecurityException(AuthState.Failed, "SignIn error");
            }
        }

        public bool SignIn(string loginName, string pass, bool createPersistentCookie, bool enableException)
        {
            try
            {
                var user = Authorizer.Login(loginName, pass);
                if (user == null)
                {
                    Log.Error("User not Authenticated");
                    if (enableException)
                        throw new MemberAccessException("Access is denied");
                    return false;
                }
                if (!user.IsAuthenticated)
                {
                    Log.Error("User not Authenticated");
                    if (enableException)
                        throw new MemberAccessException("User not Authenticated");
                    return false;
                }
                SignIn(user, createPersistentCookie);
                return IsAuthenticated;
            }
            catch (Exception ex)
            {
                Log.Exception("SignIn error ", ex);
                return false;
            }
        }

        public static SignedUser DoSignInUser(string loginName, string pass, bool createPersistentCookie, bool isAdmin)
        {
            FormsAuth form = new FormsAuth(new HttpContextAccess());// null);
            return form.SignInUser(loginName, pass,createPersistentCookie, isAdmin);
        }

        public SignedUser SignInUser(string loginName, string pass, bool createPersistentCookie, bool isAdmin)
        {
            try
            {
                SignedUser user = Authorizer.Login(loginName, pass);
                if (user == null)
                {
                    Log.Error("User not Authenticated");
                    return SignedUser.NotAuthrized(AuthState.UnAuthorized, "Access is denied");
                }
                if (!user.IsAuthenticated)
                {
                    Log.Error("User not Authenticated");
                    return SignedUser.NotAuthrized(AuthState.UnAuthorized, "User not Authenticated");
                }
                if (isAdmin && user.IsAdmin==false)
                {
                    Log.Error("Permission error: User not allowed");
                    return SignedUser.NotAuthrized(AuthState.UnAuthorized, "Permission error: User not allowed");
                }
                SignIn(user, createPersistentCookie);
                return user;
            }
            catch (Exception ex)
            {
                Log.Exception("SignIn error ", ex);
                return SignedUser.NotAuthrized(AuthState.UnAuthorized, "Authorization error: User not Signed In");
            }
        }


        //public void SignIn(string loginName, string pass, bool createPersistentCookie)
        //{
        //    var auth = Authorizer.Authenticate(loginName, pass);
        //    if (auth == null)
        //    {
        //        throw new MemberAccessException("Access is denied");
        //    }
        //    if (!auth.IsAuthenticated)
        //    {
        //        throw new MemberAccessException("User not Authenticated");
        //    }
        //    SignIn(auth, createPersistentCookie);
        //}

        public void SignIn(ISignedUser user, bool createPersistentCookie)
        {
            var now = DateTime.Now;// _clock.UtcNow.ToLocalTime();
            
            //user.IsMobile= DeviceHelper.IsMobile(this._httpContextAccess.Current());

            var userData = user.UserData(user.UserDataVersion);// Convert.ToString(user.UserId);
            
            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                user.UserName,
                now,
                now.Add(ExpirationTimeSpan),
                createPersistentCookie,
                userData,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };

            var httpContext = _httpContextAccess.Current();
           

            if (_settings!=null &&  !String.IsNullOrEmpty(_settings.RequestUrlPrefix))
            {
                var cookiePath = httpContext.Request.ApplicationPath;
                if (cookiePath != null && cookiePath.Length > 1)
                {
                    cookiePath += '/';
                }

                cookiePath += _settings.RequestUrlPrefix;
                cookie.Path = cookiePath;
            }

            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            if (createPersistentCookie)
            {
                cookie.Expires = ticket.Expiration;
            }

            httpContext.Response.Cookies.Add(cookie);
            httpContext.Session.Remove(SignedUser.SessionKey);
            _isAuthenticated = true;
            _signedInUser = user;
        }

        public void SignOut()
        {
            if (_httpContextAccess != null)
            {
                var httpContext = _httpContextAccess.Current();
                if (httpContext != null)
                    httpContext.Session.Remove(SignedUser.SessionKey);
            }
            _signedInUser = null;
            _isAuthenticated = false;
            FormsAuthentication.SignOut();
        }

        public void SetAuthenticatedUserForRequest(ISignedUser user)
        {
            _signedInUser = user;
            _isAuthenticated = true;
        }

        public bool IsAuthenticatedUser(UserDataVersion version)
        {
            GetAuthenticatedUser(version);
            return IsAuthenticated;
        }

        public string GetUserName()
        {
            try
            {
                return HttpContext.Current.User.Identity.Name;
            }
            catch
            {
                return null;
            }
        }

        public ISignedUser GetAuthenticatedUser(UserDataVersion version)
        {
            if (_signedInUser != null || _isAuthenticated)
                return _signedInUser;

            var httpContext = _httpContextAccess.Current();
            if (httpContext == null || !httpContext.Request.IsAuthenticated || !(httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)httpContext.User.Identity;
            //var userData = formsIdentity.Ticket.UserData;
            //string userName = formsIdentity.Name;
            var signedUser = SignedUser.Parse(formsIdentity, version);
            //var signedUser = new SignedUser(formsIdentity);
            _isAuthenticated = signedUser.IsAuthenticated;
            if (signedUser.IsAuthenticated == false)
            {
                Log.Fatal("User not Authenticated");
                return null;
            }

            return signedUser;

            //    int userId;
            //    if (!int.TryParse(userData, out userId))
            //    {
            //        Log.Fatal("User id not a parsable integer");
            //        return null;
            //    }

            //    _isAuthenticated = true;
            //    return _signedInUser;// = _contentManager.Get(userId).As<IUser>();
        }
    }
}
