using Nistec.Data;
using Nistec.Data.Entities;
using Nistec.Generic;
using Nistec.Runtime;
using Nistec.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace Nistec.Web.Security
{
    public enum PasswordScore
    {
        Blank = 0,
        Denied = 1,
        Weak = 3,
        Medium = 6,
        Strong = 10,

        //VeryWeak = 1,
        //Weak = 2,
        //Medium = 3,
        //Strong = 4,
        //VeryStrong = 5
    }

    public class SignedUser : UserProfile, ISignedUser//, IUser
    {
        internal const string SessionKey = "SignedUser";
        public static SignedUser Get(HttpContextBase context, UserDataVersion version)
        {
            if (context == null || !context.Request.IsAuthenticated || !(context.User.Identity is FormsIdentity))
            {
                return NotAuthrized(AuthState.UnAuthorized, "Http error: Invalid HttpContext");
            }
            SignedUser signedUser = null;
            //SignedUser signedUser = (SignedUser)context.Session[SignedUser.SessionKey];
            //if(signedUser!=null)
            //{
            //    return signedUser;
            //}
            var formsIdentity = (FormsIdentity)context.User.Identity;
            signedUser = SignedUser.Parse(formsIdentity, version);
            //signedUser = new SignedUser(formsIdentity);
            if (signedUser.IsAuthenticated == false || signedUser.IsBlocked)
            {
                //Log.Fatal("User not Authenticated");
                return NotAuthrized(AuthState.UnAuthorized, "Authenticatation error: User not Authenticated");
            }
            signedUser.State = (int)AuthState.Succeeded;
            signedUser.IsMobile = DeviceHelper.IsMobile(context.Request);
            //context.Session[SignedUser.SessionKey] = signedUser;
            return signedUser;
        }

        public static SignedUser Get(HttpContextBase context, UserRole role, UserDataVersion version)
        {

            if (context == null || !context.Request.IsAuthenticated || !(context.User.Identity is FormsIdentity))
            {
                return NotAuthrized(AuthState.UnAuthorized, "Http error: Invalid HttpContext");
            }
            SignedUser signedUser = null;
            //SignedUser signedUser = (SignedUser)context.Session[SignedUser.SessionKey];
            //if(signedUser!=null)
            //{
            //    return signedUser;
            //}
            var formsIdentity = (FormsIdentity)context.User.Identity;
            signedUser = SignedUser.Parse(formsIdentity, version);
            //signedUser = new SignedUser(formsIdentity);
            if (signedUser.IsAuthenticated == false || signedUser.IsBlocked)
            {
                //Log.Fatal("User not Authenticated");
                return NotAuthrized(AuthState.UnAuthorized, "Authenticatation error: User not Authenticated");
            }
            if (signedUser.UserRole < (int)role)
            {
                //Log.Fatal("User not Authenticated");
                return NotAuthrized(AuthState.UnAuthorized, "Authenticatation error: Access is denied");
            }
            signedUser.State = (int)AuthState.Succeeded;
            signedUser.IsMobile = DeviceHelper.IsMobile(context.Request);
            //context.Session[SignedUser.SessionKey] = signedUser;
            return signedUser;
        }

        internal static SignedUser Parse(FormsIdentity identity, UserDataVersion version)
        {
            SignedUser user = null;
            if (version == UserDataVersion.Json)
            {
                user = JsonSerializer.Deserialize<SignedUser>(identity.Ticket.UserData);
            }
            else
            {
                user = new SignedUser(identity, version);
            }

            if (user == null || user.UserId == 0)
            {
                throw new Exception("Invalid User Data");
            }
            return user;
        }

        static SignedUser ParseData(string userData)
        {
            SignedUser user = JsonSerializer.Deserialize<SignedUser>(userData);
            if (user == null || user.UserId == 0)
            {
                throw new Exception("Invalid User Data");
            }
            return user;
        }

        //public static string GetJson(HttpContextBase context)
        //{
        //    if (context == null || !context.Request.IsAuthenticated || !(context.User.Identity is FormsIdentity))
        //    {
        //        return JsonSerializer.ConvertToJson(new object[] { "state", AuthState.UnAuthorized, "desc", "Http error: Invalid HttpContext" }, null);
        //    }
        //    string userData = null;
        //    var formsIdentity = (FormsIdentity)context.User.Identity;
        //    if (formsIdentity != null)
        //    {
        //        userData = formsIdentity.Ticket.UserData;
        //    }
        //    if (userData == null)
        //    {
        //        return JsonSerializer.ConvertToJson(new object[] { "state", AuthState.UnAuthorized, "desc", "FormsIdentity error: Invalid User Data" }, null);
        //    }

        //    return SignedUser.UserDataToJson(userData);
        //}

        internal static SignedUser NotAuthrized(AuthState state, string desc)
        {
            return new SignedUser() { State = (int)state, StateDescription = desc };
        }

        public static PasswordScore CheckStrength(string password)
        {
            int score = 0;

            string strongRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})";
            string mediumRegex = @"^(((?=.*[a-z])(?=.*[A-Z]))|((?=.*[a-z])(?=.*[0-9]))|((?=.*[A-Z])(?=.*[0-9])))(?=.{6,})";


            if (password.Length < 1)
                return PasswordScore.Blank;
            if (password.Length < 5)
                return PasswordScore.Denied;
            if (Regex.Match(password, strongRegex, RegexOptions.ECMAScript).Success)
                return PasswordScore.Strong;
            if (Regex.Match(password, mediumRegex, RegexOptions.ECMAScript).Success)
                return PasswordScore.Medium;
            else
                return PasswordScore.Weak;

            //if (password.Length >= 8)
            //    score++; 
            //if (password.Length >= 12)
            //    score++; 
            //if (Regex.Match(password, @"/\d+/", RegexOptions.ECMAScript).Success)
            //    score++; 
            //if (Regex.Match(password, @"/[a-z]/", RegexOptions.ECMAScript).Success &&
            //  Regex.Match(password, @"/[A-Z]/", RegexOptions.ECMAScript).Success)
            //    score++; 
            //if (Regex.Match(password, @"/.[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]/", RegexOptions.ECMAScript).Success)
            //    score++;

            //return (PasswordScore)score;
        }

        public SignedUser() { }

        public SignedUser(FormsIdentity identity, UserDataVersion version)
        //: base(identity, version)
        {
            UserDataVersion = version;
            UserName = identity.Name;
            Decode(identity.Ticket.UserData, version);
        }

        bool Decode(string userData, UserDataVersion version)//, HttpContextBase context)
        {
            //version=Serialize|DataJson|DataPipe

            if (version == UserDataVersion.Json)
            {
                //use SignedUser.ParseData(userData);
                return false;
            }
            else //if (version == UserDataVersion.DataJson)
            {
                Data = NameValueArgs.ParseJson(userData);
                if (Data == null || Data.Count == 0)
                    return false;

                UserId = Data.Get<int>("UserId");
                AccountId = Data.Get<int>("AccountId");
                UserRole = Data.Get<int>("UserRole");
                Lang = Data["Lang"];
                AccountCategory = Data.Get<int>("AccountCategory");
                AccountName = Data["AccountName"];
                DisplayName = Data["DisplayName"];
                Phone = Data["Phone"];
                Email = Data["Email"];
                //IsConfirmed = Data["IsConfirmed"];
                State = Data.Get<int>("State");
                //HostClient = context.Request.UserHostAddress;
                int accessId = 0;
                if (Data.TryGetValue("AccessId", out accessId))
                    AccessId = accessId;
                else if (Data.TryGetValue("ParentId", out accessId))
                    AccessId = accessId;

                AccType = Data.Get<int>("AccType");
                //AccAccess = Data["AccAccess"];
                //ClaimsJson = Data["ClaimsJson"];
                ExType = Data.Get<int>("ExType");
                //Cv = Data["Cv"];
                return true;
            }
            //else
            //{

            //    //int applicationId = 0;
            //    int accountId = 0;
            //    int userId = 0;
            //    int userRole = 0;
            //    int accountCategory = 0;
            //    string lang = "he";
            //    string accountName = null;
            //    string displayName = null;
            //    int accessId = 0;
            //    string dataConfig = null;

            //    if (GenericArgs.SplitArgs<int, int, int, string, string, int, string, int, string>(userData, DataSplitterCh, ref accountId, ref userId, ref userRole, ref lang, ref accountName, ref accountCategory, ref displayName, ref parentId, ref dataConfig))
            //    {
            //        UserId = userId;
            //        AccountId = accountId;
            //        UserRole = userRole;
            //        Lang = lang;
            //        AccountCategory = 0;
            //        AccountName = BaseConverter.UnEscape(accountName, DataSplitter, DataSplitEscape);
            //        DisplayName = BaseConverter.UnEscape(displayName, DataSplitter, DataSplitEscape);
            //        AccessId = accessId;
            //        var args = BaseConverter.UnEscape(dataConfig, DataSplitter, DataSplitEscape);
            //        if (!string.IsNullOrEmpty(args))
            //        {
            //            var array = args.Split('|');
            //            Data = NameValueArgs.Get(array);
            //            //Data = NameValueArgs.Create(args);// GenericRecord.Parse(BaseConverter.UnEscape(dataConfig, "-", "%"));
            //        }
            //        return true;
            //    }
            //    //if (GenericArgs.SplitArgs<int, int, int, int, int, string, string>(userData, '-', ref applicationId, ref accountId, ref userId, ref userRole, ref ownerId, ref lang, ref perms))
            //    //{
            //    //    UserId = userId;
            //    //    AccountId = accountId;
            //    //    UserRole = userRole;
            //    //    ApplicationId = applicationId;
            //    //    OwnerId = ownerId;
            //    //    Lang = lang;
            //    //    Perms = perms;
            //    //    return true;
            //    //}
            //    return false;
            //}

        }

        #region Properties
        [EntityProperty]
        public UserDataVersion UserDataVersion { get; set; }
        //[EntityProperty]
        //public int AuthState { get; set; }
        

        [EntityProperty]
        public int State { get; set; }
        [EntityProperty(EntityPropertyType.NA)]
        public string StateDescription { get; set; }
        [EntityProperty]
        public int EvaluationDays { get; set; }
        [EntityProperty(EntityPropertyType.NA)]
        public bool IsMobile { get; set; }
        [EntityProperty(EntityPropertyType.NA)]
        public int ExType { get; set; }
        [EntityProperty(EntityPropertyType.NA)]
        public string AppName { get; set; }
        [EntityProperty(EntityPropertyType.NA)]
        public string HostClient { get; set; }

        [EntityProperty(EntityPropertyType.View)]
        public int AccountCategory { get; set; }
        [EntityProperty(EntityPropertyType.View)]
        public string AccountName { get; set; }
        [EntityProperty(EntityPropertyType.View)]
        public int AccType { get; set; }
        //[EntityProperty(EntityPropertyType.View)]
        //public string AccAccess { get; set; }
        //[EntityProperty(EntityPropertyType.View)]
        //public string ClaimsJson { get; set; }
        //[EntityProperty(EntityPropertyType.View)]
        //public string Cv { get; set; }

        public void ChangeAuthState(int state) {

            Data["State"] = state.ToString();
            State = state;
        }

        #endregion

        #region Properties ex
        /*
        //v-e-r-x-m
        //view-edit-remove-export-management
        public bool AllowEdit
        {
            get { return UserRole >= 1; }
        }
        public bool AllowAdd
        {
            get { return UserRole >= 1; }
        }
        public bool AllowExport
        {
            get { return UserRole >= 5; }
        }
        public bool AllowDelete
        {
            get { return UserRole >= 5; }
        }
        public string AllowEditClass
        {
            get { return AllowEdit ? "" : "item-pasive"; }
        }
        public string AllowAddClass
        {
            get { return AllowAdd ? "" : "item-pasive"; }
        }
        public string AllowExportClass
        {
            get { return AllowExport ? "" : "item-pasive"; }
        }
        public string AllowDeleteClass
        {
            get { return AllowDelete ? "" : "item-pasive"; }
        }
        //public bool IsManager
        //{
        //    get { return UserRole >= 5; }
        //}
        */
        #endregion



        #region User Data Json

        public void SetUserDataJson(string AppName, string ClientIP)
        {
            DataJson = UserDataContext.GetUserDataJson(AccountId, UserId, AppName, ClientIP);
        }

        [EntityProperty(EntityPropertyType.NA)]
        public string DataJson { get; set; }
        //public string UserDataJson()
        //{
        //    string genericData = Data == null ? "" : Data.ToJson();
        //    object[] args = new object[] { "acid", AccountId, "uid", UserId, "role", UserRole, "lang", Lang, "acname", AccountName, "category", AccountCategory, "uname", DisplayName, "pid", ParentId, "data", genericData };
        //    return JsonSerializer.ConvertToJson(args, null);

        //    //return ""+ string.Format("aid:{0},uid:{1},role:{2},lang:{3},aname:{4},category:{5},uname{6},pid:{7},data:{8}", AccountId, UserId, UserRole, Lang, AccountName, AccountCategory, DisplayName, ParentId, genericData)+""+ DataJson;
        //}
        //public static string UserDataToJson(string userData)
        //{
        //    string[] data = userData.Split(DataSplitterCh);
        //    string genericData = "";
        //    string dataargs = data[8];
        //    if (!string.IsNullOrEmpty(dataargs))
        //    {
        //        dataargs = BaseConverter.UnEscape(dataargs, DataSplitter, DataSplitEscape);
        //        genericData = JsonSerializer.ConvertToJson(dataargs.Split('|'), null);
        //    }

        //    data[4] = BaseConverter.UnEscape(data[4], DataSplitter, DataSplitEscape);
        //    data[6] = BaseConverter.UnEscape(data[6], DataSplitter, DataSplitEscape);

        //    object[] args = new object[] { "acid", data[0], "uid", data[1], "role", data[2], "lang", data[3], "acname", data[4], "category", data[5], "uname", data[6], "pid", data[7], "data", genericData };
        //    return JsonSerializer.ConvertToJson(args, null);
        //}
        #endregion

        //[EntityProperty(EntityPropertyType.NA)]
        //public NameValueArgs Claims { get; set; }
        //public string ClaimsSerilaize()
        //{
        //    string genericData = (Claims != null) ? Claims.ToKeyValuePipe() : null;
        //    return genericData;
        //}
        //public NameValueArgs ClaimsDeserilaize(string data)
        //{
        //    string[] args = data.SplitTrim('|');
        //    NameValueArgs claims = new NameValueArgs(args);
        //    return claims;
        //}

        public void SetUserDataEx(UserDataVersion version, string AppName, string ClientIP)
        {
            if (version == UserDataVersion.DataJson || version == UserDataVersion.DataPipe)
            {
                Data = UserDataContext.GetUserDataEx(AccountId, UserId, AppName, ClientIP);
            }

        }

        [EntityProperty(EntityPropertyType.NA)]
        public NameValueArgs Data { get; set; }

        public string GetDataValue(string key)
        {
            if (Data == null)
                return "";
            return Data.Get(key);
        }
        public T GetDataValue<T>(string key)
        {
            if (Data == null)
                return default(T);
            return Data.Get<T>(key);
        }

        //public string Serialize()
        //{
        //    return JsonSerializer.Serialize(this);
        //}


        public string UserData(UserDataVersion version)
        {

            if (version == UserDataVersion.Json)
            {
                return JsonSerializer.Serialize(this);
            }
            else //if (version == UserDataVersion.DataJson)
            {
                return (Data != null) ? Data.ToJson() : null;
            }
            //else //DataPipe
            //{
            //    string genericData = (Data != null) ? Data.ToKeyValuePipe() : null;

            //    return string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}", AccountId, UserId, UserRole, Lang, BaseConverter.Escape(AccountName, UserProfile.DataSplitter, DataSplitEscape), AccountCategory, BaseConverter.Escape(DisplayName, DataSplitter, DataSplitEscape), ParentId, BaseConverter.Escape(genericData, DataSplitter, DataSplitEscape));
            //}
        }

        //use UserData(UserDataVersion version)
        //public string UserData()
        //{

        //    string genericData = (Data != null) ? Data.ToKeyValuePipe() : null;

        //    return string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}", AccountId, UserId, UserRole, Lang, BaseConverter.Escape(AccountName, UserProfile.DataSplitter, DataSplitEscape), AccountCategory, BaseConverter.Escape(DisplayName, DataSplitter, DataSplitEscape), ParentId, BaseConverter.Escape(genericData, DataSplitter, DataSplitEscape));

        //}
    }

}
