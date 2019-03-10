using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nistec.Generic;
using Nistec.Data.Entities;
using System.Text.RegularExpressions;
using Nistec.Data;
using System.Data;
using Nistec.Runtime;

namespace Nistec.Web.Security
{
    //db objects
    //=============================
    //websp_UserRegister
    //websp_UserAuthenticateService
    //websp_UserAuthenticate
    //web_UserProfile
    //web_UserMembership
    //web_UserEvaluation
    //web_UserRoles
    //web_EmailProvider
    [Entity("Authorizer", EntityMode.Config)]
    public class Authorizer : EntityContext<SignedUser>
    {
        internal const string UserRegisterMappingName = "websp_UserRegister";
        internal const string UserAuthenticateServiceMappingName = "websp_UserAuthenticateService";
        internal const string UserAuthenticateMappingName = "websp_UserAuthenticate";
        //web_UserProfile
        //web_UserMembership
        //web_UserEvaluation
        //web_UserRoles
        //web_EmailProvider

        #region ctor


        public Authorizer(string connectionName)
            : base()
        {
        }

        public Authorizer(int UserId)
            : base(UserId)
        {

        }

        public Authorizer(SignedUser item)
            : base(item)
        {

        }
        protected Authorizer()
            : base()
        {
        }

        internal static Authorizer Instance
        {
            get { return new Authorizer(); }
        }
        #endregion

        #region binding

        protected override void EntityBind()
        {
            //base.EntityDb = new EntityDb();
            //base.EntityDb.EntityCulture = Nistec.Data.DB.NetcellDB.GetCulture();
            //If EntityAttribute not define you can initilaize the entity here
            //base.InitEntity<AdventureWorks>("Contact", "Person.Contact", EntityKeys.Get("ContactID"));
        }

        #endregion

        #region Register


        public static UserResult Register(UserRegister u, bool SendResetToken = true)
        {
            try
            {
                if (u == null || u.UserName == null || u.Password == null)
                {
                    throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context Register error,null UserProfile or password");
                }
                string Password = u.Password;
                if (!Authorizer.IsValidPassword(Password))
                {
                    throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illegal UserName or password, expect AlphaNumeric");
                }
                if (!Authorizer.IsValidUserName(u.UserName))
                {
                    throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illegal UserName or password, expect AlphaNumeric");
                }
                SignedUser res = null;
                using (Authorizer context = new Authorizer())
                {
                    res = context.EntityDb.DoCommand<SignedUser>("sp_Ad_UserRegister",
                    DataParameter.GetSql("DisplayName", u.DisplayName, "Email", u.Email, "Phone", u.Phone, "UserName", u.UserName, "UserRole", u.UserRole, "AccountId", u.AccountId, "Lang", u.Lang, "Evaluation", u.Evaluation, "IsBlocked", u.IsBlocked, "Password", Password, "SendResetToken", SendResetToken, "PasswordShouldChange", u.PasswordShouldChange, "PasswordExpirationDate", u.PasswordExpirationDate), CommandType.StoredProcedure);
                }
                return UserResult.Get((MembershipStatus)res.State); //new UserResult() { Status = res.State };
            }
            catch (Exception ex)
            {
                return new UserResult() { Status = -1, Message = ex.Message };
            }
        }

        //public static SignedUser Register(UserProfile u)
        //{
        //    string Password = Authorizer.GenerateRandomPassword(6);

        //    if (u == null || u.UserName == null || Password == null)
        //    {
        //        throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context Register error,null UserProfile or password");
        //    }
        //    if (!Authorizer.IsAlphaNumeric(u.UserName, Password))
        //    {
        //        throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illegal UserName or password, expect AlphaNumeric");
        //    }
        //    if (!Authorizer.IsValidString(u.UserName) || !Authorizer.IsValidString(Password))
        //    {
        //        throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illeagal user name or password");
        //    }

        //    using (Authorizer context = new Authorizer())
        //    {
        //        return context.EntityDb.DoCommand<SignedUser>("sp_Ad_UserRegister",
        //        DataParameter.GetSql("DisplayName", u.DisplayName, "Email", u.Email, "Phone", u.Phone, "UserName", u.UserName, "UserRole", u.UserRole, "AccountId", u.AccountId, "Lang", u.Lang, "Evaluation", u.Evaluation, "IsBlocked", u.IsBlocked, "Password", Password), CommandType.StoredProcedure);
        //    }
        //}

        public static int Register(UserProfile u, string Password, bool SendResetToken=true,bool PasswordShouldChange=true, DateTime? PasswordExpirationDate=null)
        {
            if (u == null || u.UserName == null || Password == null)
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context Register error,null UserProfile or password");
            }
            if (!Authorizer.IsValidPassword(Password))
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illegal UserName or password, expect AlphaNumeric");
            }
            if (!Authorizer.IsValidUserName(u.UserName))
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illegal UserName or password, expect AlphaNumeric");
            }

            using (Authorizer context = new Authorizer())
            {
                return context.EntityDb.Context().ExecuteReturnValue("sp_Ad_UserRegister",-1,
                "DisplayName", u.DisplayName, "Email", u.Email, "Phone", u.Phone, "UserName", u.UserName, "UserRole", u.UserRole, "AccountId", u.AccountId, "Lang", u.Lang, "Evaluation", u.Evaluation, "IsBlocked", u.IsBlocked, "Password", Password, "SendResetToken", SendResetToken, "PasswordShouldChange", PasswordShouldChange, "PasswordExpirationDate", PasswordExpirationDate, CommandType.StoredProcedure);
            }
        }

        public static string CreatePassword(int UserId, bool sendMail, string resetToken, int expirationInMinute = 1440)
        {
            UserProfile user = UserProfile.Get(UserId);
            string newpassword = Authorizer.GenerateRandomPassword(6);
            UserMembership member = new UserMembership()
            {
                UserId = UserId,
                Password = Encryption.HashPassword(newpassword),
                PasswordChangedDate = DateTime.Now,
                PasswordVerificationToken = resetToken,
                PasswordVerificationTokenExpirationDate = DateTime.Now.AddMinutes(expirationInMinute)
            };
            UserMessage message = UserMessage.Get("CreatePassword");
            var res = member.Insert();
            if (res.Status <= 0)
            {
                throw new SecurityException(AuthState.NonConfirmed, message.GetErrorMessage("Internal error, could not create password"));
            }

            string notifyMessage = "";
            if (sendMail)
            {
                EmailProvider provider = EmailProvider.Get();
                var state = provider.SendEmail(user.Email, message.Subject, message.GetMailMessage("username:" + user.UserName, "password:" + newpassword), true);
                notifyMessage = message.GetNotifyMessage(state.Message);
            }

            return newpassword;
        }

        
 

        #endregion

        #region Login

        public static SignedUser Login(string UserName, string Password, string HostClient = null, string HostReferrer = null, string AppName = null, bool? IsMobile = null)//, int AccountId)
        {

            if (UserName == null || Password == null)
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,null user name or password");
            }
            if (!IsValidPassword(Password))//(UserName, Password))
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illegal UserName or password, expect AlphaNumeric");
            }
            if (!IsValidUserName(UserName))
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illeagal user name or password");
            }
            SignedUser user = null;
            using (Authorizer context = new Authorizer())
            {
                user = context.EntityDb.QuerySingle<SignedUser>("UserName", UserName, "Password", Password, "HostClient", HostClient, "HostReferrer", HostReferrer, "AppName", AppName, "IsMobile", IsMobile);//, "AccountId", AccountId);
            }
            if (user == null)
            {
                throw new SecurityException(AuthState.UnAuthorized, "AuthorizationException, Un Authorized User");
            }

            AuthState state = (AuthState)user.State;
            if (state != AuthState.Succeeded)
            {
                throw new SecurityException(state, "AuthorizationException, Un Authorized, status:" + state.ToString());
            }

            if (user.AccountId <= 0)
            {
                throw new SecurityException(AuthState.UnAuthorized, "AuthorizationException, Un Authorized Account");
            }

            return user;
        }

        public static T Login<T>(string UserName, string Password, string HostClient = null, string HostReferrer = null, string AppName = null, bool? IsMobile = null) where T: ISignedUser
        {

            if (UserName == null || Password == null)
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,null user name or password");
            }
            if (!IsValidPassword(Password))//(UserName, Password))
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illegal UserName or password, expect AlphaNumeric");
            }
            if (!IsValidUserName(UserName))
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illeagal user name or password");
            }
            ISignedUser user = null;
            using (Authorizer context = new Authorizer())
            {
                user = context.EntityDb.QuerySingle<T>("UserName", UserName, "Password", Password, "HostClient", HostClient, "HostReferrer", HostReferrer, "AppName", AppName, "IsMobile", IsMobile);//, "AccountId", AccountId);
            }

            if (user == null)
            {
                throw new SecurityException(AuthState.UnAuthorized, "AuthorizationException, Un Authorized User");
            }

            AuthState state = (AuthState)user.State;
            if (state != AuthState.Succeeded)
            {
                throw new SecurityException(state, "AuthorizationException, Un Authorized, status:" + state.ToString());
            }

            if (user.AccountId <= 0)
            {
                throw new SecurityException(AuthState.UnAuthorized, "AuthorizationException, Un Authorized Account");
            }

            return (T)user;
        }

        #endregion

        #region Authenticate

        public static SignedUser Get(int UserId)
        {
            using (Authorizer context = new Authorizer(UserId))
            {
                return context.Entity;
            }
        }


        #endregion

        #region AuthenticateService

        public static SignedUser AuthenticateService(
         string UserName,
         string Password)
        {
            return AuthenticateService(UserName, Password, DataParameter.GetSql("UserName", UserName, "Password", Password));
        }

        public static SignedUser AuthenticateService(
         string UserName,
         string Password,
         int Application)
        {
            return AuthenticateService(UserName, Password, DataParameter.GetSql("Application", Application, "UserName", UserName, "Password", Password));
        }


        public static SignedUser AuthenticateService(
         string UserName,
         string Password,
         int Application,
         int RequiredIp,
         string HostClient,
         string HostServer)
        {
            return AuthenticateService(UserName, Password, DataParameter.GetSql("Application", Application, "UserName", UserName, "Password", Password, "RequiredIp", RequiredIp, "HostClient", HostClient, "HostServer", HostServer));
        }

        public static SignedUser AuthenticateService(
         string UserName,
         string Password,
         IDbDataParameter[] parameters)
        {
            if (UserName == null || Password == null)
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,null user name or password");
            }
            if (!Authorizer.IsValidPassword(Password))
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illegal UserName or password, expect AlphaNumeric");
            }
            if (!Authorizer.IsValidUserName(UserName))
            {
                throw new SecurityException(AuthState.UnAuthorized, "Authorizer_Context error,Illeagal user name or password");
            }
            SignedUser user = null;
            using (Authorizer context = new Authorizer())
            {
                user = context.EntityDb.DoCommand<SignedUser>("websp_UserAuthenticateService", parameters, CommandType.StoredProcedure);
            }
            if (user == null)
            {
                throw new SecurityException(AuthState.UnAuthorized, "AuthorizationException, Un Authorized User");
            }

            AuthState state = (AuthState)user.State;
            if (state != AuthState.Succeeded)
            {
                throw new SecurityException(state, "AuthorizationException, Un Authorized, status:" + state.ToString());
            }

            if (user.AccountId <= 0)
            {
                throw new SecurityException(AuthState.UnAuthorized, "AuthorizationException, Un Authorized Account");
            }

            return user;
        }
        #endregion

        #region util
        public static bool IsAlphaNumeric(params string[] expression)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]+$");
            foreach (string str in expression)
            {
                if (!regex.Match(str).Success)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsValidPassword(string expression)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9\.\-_!#@]+$");

            if (!regex.Match(expression).Success)
            {
                return false;
            }
            return true;
        }
        public static bool IsValidUserName(string s)
        {
            if (!Regx.RegexValidateIgnoreCase(@"^[a-zA-Z0-9\.\-_]+$", s))
            {
                return false;
            }
            if (Regx.RegexValidateIgnoreCase(@"(\s|)(drop|create|alter|delete|insert|update|select|from|exec|execute|script)\s.*", s))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidString(string s)
        {
            if (s.IndexOfAny(new char[] { 
                '[', ']', '(', ')', '{', '}', '|', '<', '>', '!', '=', ';', ':', '&', '?', '*', 
                '%', '&', '+', ' ', '\''
             }) > 0)
            {
                return false;
            }
            if (Regx.RegexValidateIgnoreCase(@"(\s|)(drop|create|alter|delete|insert|update|select|from|exec|execute|script)\s.*", s))
            {
                return false;
            }
            return true;
        }
        

        public static string GenerateRandomPassword(int length)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-*&#+";
            char[] chars = new char[length];
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }


        public static UserMessage GetMessageTemplate(string MessageType)
        {
            if (MessageType == null)
            {
                throw new SecurityException(AuthState.Failed, "Authorizer_Context error,null MessageTemplate");
            }
            UserMessage entity = null;
            using (Authorizer context = new Authorizer())
            {
                entity = context.ExecuteCommand<UserMessage>("select * from Ad_Message where MessageType=@MessageType", DataParameter.GetSql("MessageType", MessageType));
            }
            if (entity == null)
            {
                throw new SecurityException(AuthState.UnAuthorized, "AuthorizationException, Un Authorized UserMessage");
            }

            return entity;
        }

        public static void ValidateUserAndPass(string UserName,string Password)
        {
            if (UserName == null || Password == null)
            {
                throw new ArgumentException("Authorizer_Context error,null user name or password");
            }
            if (!Authorizer.IsValidPassword(Password))
            {
                throw new ArgumentException("Authorizer_Context error,Illegal UserName or password, expect AlphaNumeric");
            }
            if (!Authorizer.IsValidUserName(UserName))
            {
                throw new ArgumentException("Authorizer_Context error,Illeagal user name or password");
            }
        }
        #endregion
    }

    [Entity("AuthorizerService", EntityMode.Config)]
    public class AuthorizerService : EntityContext<SignedUser>
    {
        #region ctor


        public AuthorizerService(string connectionName)
            : base()
        {
        }

        public AuthorizerService(int UserId)
            : base(UserId)
        {

        }

        public AuthorizerService(SignedUser item)
            : base(item)
        {

        }
        protected AuthorizerService()
            : base()
        {
        }

        internal static AuthorizerService Instance
        {
            get { return new AuthorizerService(); }
        }
        #endregion

        #region AuthenticateService

        public static SignedUser Auth(
         string UserName,
         string Password,
         int AccountId,
         int AppId)
        {
            Authorizer.ValidateUserAndPass(UserName, Password);
            return AuthService("UserName", UserName, "Password", Password, "AccountId", AccountId, "AppId", AppId);
        }

        public static SignedUser AuthApi(
         string UserName,
         string Password,
         int AccountId,
         int AppId,
         //string AccessPoint,
         string HostClient,
         string HostServer = null)
        {
            Authorizer.ValidateUserAndPass(UserName, Password);
            return AuthService("UserName", UserName, "Password", Password, "AccountId", AccountId, "AppId", AppId, "AccessPoint", "api", "HostClient", HostClient, "HostServer", HostServer);
        }

        public static SignedUser Auth(
         string UserName,
         string Password,
         int AccountId,
         int AppId,
         string AccessPoint,
         string HostClient,
         string HostServer=null)
        {
            Authorizer.ValidateUserAndPass(UserName, Password);
            return AuthService("UserName", UserName, "Password", Password, "AccountId", AccountId, "AppId", AppId, "AccessPoint", AccessPoint, "HostClient", HostClient, "HostServer", HostServer);
        }

         internal static SignedUser AuthService(params object[] keyValueParameters)
        {
            
            SignedUser user = null;
            using (AuthorizerService context = new AuthorizerService())
            {
                user = context.EntityDb.QuerySingle<SignedUser>(keyValueParameters);
            }
            if (user == null)
            {
                throw new SecurityException(AuthState.UnAuthorized, "AuthorizationException, Un Authorized User");
            }

            AuthState state = (AuthState)user.State;
            if (state != AuthState.Succeeded)
            {
                throw new SecurityException(state, "AuthorizationException, Un Authorized, status:" + state.ToString());
            }

            if (user.AccountId <= 0)
            {
                throw new SecurityException(AuthState.UnAuthorized, "AuthorizationException, Un Authorized Account");
            }

            return user;
        }
        #endregion
    }
}
