using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Nistec;
using Nistec.Runtime;

namespace Nistec.Web.Asp
{
    public abstract class LoginControl : SessionUserControl
    {
 
        protected override bool AutoReSign
        {
            get
            {
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bool ok = false;
                if (AutoReSign && Request.Browser.Cookies)
                {
                    if (Request.Cookies[ScPath.SiteName] != null)
                    {
                        ok = VerifyLogin(Request.Cookies[ScPath.SiteName]);
                    }
                }
                if (!ok)
                {
                    AspLogin.Focus();
                }
            }
        }

        protected void OnAuthenticate(object sender, AuthenticateEventArgs e)
        {
            e.Authenticated = true;
                SignIn(AspLogin.UserName, AspLogin.Password, AspLogin.RememberMeSet);
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            e.Authenticated = true;
                SignIn(AspLogin.UserName, AspLogin.Password, AspLogin.RememberMeSet);
        }

        protected void Login_Clicked(object sender, EventArgs e)
        {
             SignIn(AspLogin.UserName, AspLogin.Password, AspLogin.RememberMeSet);
        }

        public abstract string WL { get; set; }
        public abstract string RedirectTo { get; set; }
        public abstract Login AspLogin { get; }
        
        public virtual bool EnableCookiLogin
        {
            get { return true; }
        }

        protected bool VerifyLogin(HttpCookie cookies)
        {
            if (cookies == null || cookies.Values.Count == 0)
                return false;
            //if (cookies.Expires < DateTime.Now)
            //    return false;
            string login = Encryption.DecryptPass(string.Format("{0}", cookies[SessionControl.TagUNAME]));
            string pass = Encryption.DecryptPass(string.Format("{0}", cookies[SessionControl.TagUPASS]));
            bool rset = cookies[SessionControl.TagRSET] == "1";


            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass) || rset==false)
                return false;

            if (SecureLogin)
            {
                try
                {
                    string[] args = pass.Split(';', '|');
                    login = args[0];
                    pass = args[1];
                }
                catch
                {
                    return false;
                }
            }

            if (EnableCookiLogin)
            {
                return SignIn(login, pass, rset);
            }
            else
            {
                AspLogin.UserName = login;
                //return System.Web.Security.FormsAuthentication.Authenticate(login, pass);
                //System.Web.Security.FormsAuthentication.SetAuthCookie(AspLogin.UserName, true, cookies.Path);
                //System.Web.Security.FormsAuthentication.Initialize();
                return false;
            }
        }

        public static void Logoff(Page p, string SiteName)
        {
            if (p.Request.Browser.Cookies)
            {
                //HttpCookie cookies = p.Request.Cookies[SiteName];
                ////ok = VerifyLogin(Request.Cookies[SiteName]);
                //if (cookies == null || cookies.Values.Count == 0)
                //    return;
                //cookies.Expires = DateTime.Now.AddDays(-1);
                //p.Response.Cookies.Remove(SiteName);

                if (p.Request.Cookies[SiteName] != null)
                {
                    HttpCookie cookies = new HttpCookie(SiteName);
                    cookies.Expires = DateTime.Now.AddDays(-1d);
                    p.Response.Cookies.Add(cookies);
                }
            }
        }

    }

}

