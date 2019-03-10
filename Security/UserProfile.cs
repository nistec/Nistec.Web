using Nistec.Data;
using Nistec.Data.Entities;
using Nistec.Generic;
using Nistec.Runtime;
using Nistec.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Nistec.Web.Security
{
    
    [Entity("UserProfile", EntityMode.Config)]
    public class UserProfileContext : EntityContext<UserProfile>
    {
        #region ctor


        public UserProfileContext()
        {

        }

        public UserProfileContext(int UserId)
            : base(UserId)
        {

        }

        #endregion
    }
    //[Entity("UserRegister", EntityMode.Config)]
    //public class UserRegisterContext : EntityContext<UserProfile>
    //{
    //    #region ctor

    //    public UserRegisterContext(UserProfile user)
    //    {

    //    }

    //    #endregion


    //    public SignedUser Register(UserProfile u, string Password)
    //    {
    //        using (UserProfileContext context = new UserProfileContext())
    //        {
    //            return context.EntityDb.DoCommand<SignedUser>(
    //            DataParameter.GetSql("DisplayName",u.DisplayName,"Email",u.Email,"Phone",u.Phone,"UserName",u.UserName,"UserRole",u.UserRole,"AccountId",u.AccountId,"Lang",u.Lang,"Evaluation",u.Evaluation,"IsBlocked",u.IsBlocked,"Password",Password));
    //        }
    //    }
    //}


    public class UserRegister : UserProfile
    {
        public UserRegister() {
            this.Creation = DateTime.Now;
        }

        //public UserRegister(string UserName, int UserRole, string Email, string Phone, int AccountId, string Lang, int Evaluation, bool IsBlocked, string DisplayName, string Password)
        //{
        //    //this.UserId = UserId;
        //    this.UserName = UserName;
        //    this.UserRole = UserRole;
        //    this.Email = Email;
        //    this.Phone = Phone;
        //    this.AccountId = AccountId;
        //    this.Lang = Lang;
        //    this.Evaluation = Evaluation;
        //    this.IsBlocked = IsBlocked;
        //    this.DisplayName = DisplayName;
        //    this.Password = Password;
        //    this.Creation = DateTime.Now;
        //}

        [EntityProperty]
        public string Password { get; set; }
        [EntityProperty]
        public bool PasswordShouldChange { get; set; }
        [EntityProperty]
        public DateTime? PasswordExpirationDate { get; set; }
    }

    [Entity("UserData", EntityMode.Config)]
    public class UserDataContext : EntityContext<GenericRecord>
    {

        #region ctor

        public UserDataContext()
            : base()
        {

        }
  
        #endregion

        public static NameValueArgs GetUserDataEx(int AccountId, int UserId)
        {
            NameValueArgs userdata = null;
            using (UserDataContext context = new UserDataContext())
            {
                userdata = context.EntityDb.QuerySingle<NameValueArgs>("AccountId", AccountId, "UserId", UserId);
            }

            return userdata;// == null ? null : userdata.ToJson();
        }

        public static string GetUserDataJson(int AccountId, int UserId)
        {
            using (UserDataContext context = new UserDataContext())
            {
                return context.EntityDb.QueryJsonRecord("AccountId", AccountId, "UserId", UserId);
            }
        }

        //public static string GetUserData1(int AccountId,int UserId)
        //{
        //    GenericRecord userdata = null;
        //    using (UserDataContext context = new UserDataContext())
        //    {
        //        userdata = context.EntityDb.QuerySingle<GenericRecord>("AccountId", AccountId, "UserId", UserId);
        //    }

        //    return userdata==null ? null: userdata.ToJson();
        //}

    }
    public class UserProfileView : UserItem
    {
        public new const string MappingName = "vw_Ad_UserProfile";

        [EntityProperty]
        public string RoleName { get; set; }
    }
    [EntityMapping("vw_UserInfo", "vw_UserInfo", "משתמשים")]
    public class UserItemInfo : IEntityItem
    {
        // UserId, AccountId, DisplayName, ParentId, IsDeleted, IsBlocked

        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public int AccountId { get; set; }
        public int ParentId { get; set; }
    }
    public class UserItem : IEntityItem
    {
        [EntityProperty]
        public string DisplayName { get; set; }

        [EntityProperty(EntityPropertyType.Identity)]
        public int UserId { get; set; }
        [EntityProperty]
        public int UserRole { get; set; }
        [EntityProperty]
        public string UserName { get; set; }
        [EntityProperty]
        public string Email { get; set; }
        [EntityProperty]
        public string Phone { get; set; }
        [EntityProperty]
        public int AccountId { get; set; }
        [EntityProperty]
        public string Lang { get; set; }
        [EntityProperty]
        public int Evaluation { get; set; }
        [EntityProperty]
        public bool IsBlocked { get; set; }
        [EntityProperty]
        public DateTime Creation { get; set; }

        [EntityProperty(EntityPropertyType.View)]
        public int AccountCategory { get; set; }
        [EntityProperty(EntityPropertyType.View)]
        public string AccountName { get; set; }

        [EntityProperty]
        public int ParentId { get; set; }
        [EntityProperty]
        public int Profession { get; set; }
        [EntityProperty]
        public bool IsVirtual { get; set; }
       
    }

    public class UserProfile : UserItem, IUserProfile
    {
        public const string MappingName = "Ad_UserProfile";
        public const char DataSplitterCh = '-';
        public const string DataSplitter = "-";
        public const string DataSplitEscape = "%";

        #region static get

        public static UserProfile GetByEmail(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                throw new Exception("Authorizer_Context error,null Email");
            }
            UserProfile user = null;
            using (UserProfileContext context = new UserProfileContext())
            {
                user = context.EntityDb.QuerySingle<UserProfile>("Email", Email);//.DoCommand<UserProfile>(DataParameter.Get("Email", Email));
            }
            if (user == null)
            {
                user = new UserProfile();
                //throw new Exception("AuthorizationException, Un Authorized User Email");
            }

            return user;
        }
        public static UserProfile GetByUserName(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new Exception("Authorizer_Context error,null UserName");
            }
            UserProfile user = null;
            using (UserProfileContext context = new UserProfileContext())
            {
                user = context.EntityDb.QuerySingle<UserProfile>("UserName", UserName);
            }
            if (user == null)
            {
                user = new UserProfile();
                //throw new Exception("AuthorizationException, Un Authorized UserName");
            }

            return user;
        }
        public static UserProfile Get(int UserId)
        {
            if (UserId <= 0)
            {
                throw new Exception("Authorizer_Context error,null UserId");
            }
            UserProfile user = null;
            using (UserProfileContext context = new UserProfileContext(UserId))
            {
                user = context.Entity;
            }
            if (user == null)
            {
                throw new Exception("AuthorizationException, Un Authorized User");
            }

            return user;
        }

        public static string LookupUserName(int UserId)
        {
            if (UserId <= 0)
            {
                throw new Exception("Authorizer_Context error,null UserId");
            }
            using (UserProfileContext context = new UserProfileContext())
            {
                return context.EntityDb.Context().QueryScalar<string>("select UserName from " + MappingName + " where UserId=@UserId", "", "UserId", UserId);
            }
        }

        public static string LookupDisplayName(int UserId)
        {
            if (UserId <= 0)
            {
                throw new Exception("Authorizer_Context error,null UserId");
            }
            using (UserProfileContext context = new UserProfileContext())
            {
                return context.EntityDb.Context().QueryScalar<string>("select DisplayName from " + MappingName + " where UserId=@UserId", "", "UserId", UserId);
            }
        }

        #endregion

        public UserResult Update(UserProfile newItem)
        {
            int res = 0;
            try
            {
                using (UserProfileContext context = new UserProfileContext())
                {
                    context.Set(this);
                    res= context.SaveChanges(newItem, UpdateCommandType.Update);
                    return UserResult.IsUpdated(res);
                }
            }
            catch (Exception)
            {
                return UserResult.IsUpdated(-1);
            }
        }

        public UserResult Delete()
        {
            int res = 0;
            try
            {
                using (UserProfileContext context = new UserProfileContext())
                {

                    res = context.EntityDb.ExecuteNonQuery("delete from " + MappingName + " where UserId=@UserId", DataParameter.GetSql("UserId", UserId), System.Data.CommandType.Text);
                    return UserResult.IsDeleted(res);
                }
            }
            catch (Exception)
            {
                return UserResult.IsDeleted(-1);
            }
        }
        public UserResult Delete(string procedure)
        {
            int res = 0;
            try
            {
                using (UserProfileContext context = new UserProfileContext())
                {
                    res = context.EntityDb.ExecuteNonQuery(procedure, DataParameter.GetSql("UserId", UserId), System.Data.CommandType.StoredProcedure);
                    return UserResult.IsDeleted(res);
                }
            }
            catch (Exception)
            {
                return UserResult.IsDeleted(-1);
            }
        }
        public int Register()
        {
            using (UserProfileContext context = new UserProfileContext())
            {
                return context.SaveChanges(this, UpdateCommandType.Insert);
            }
        }
        public int Register(string procedure)
        {
            using (UserProfileContext context = new UserProfileContext())
            {
                context.Set(this);
                return context.EntityDb.ExecuteNonQuery(procedure, DataParameter.GetSqlParameters(context.EntityRecord, true), System.Data.CommandType.StoredProcedure);
            }
        }

        //public int Update(UserProfile newItem, UpdateCommandType command)
        //{
        //    int res = Authorizer.Instance.EntityDb.Db.SetEntity<UserProfile>(MappingName, this, newItem, command);
        //    return res;
        //}

        //public int Register(UserProfile newItem)
        //{
        //    int res = Authorizer.Instance.EntityDb.Db.SetEntity<UserProfile>(MappingName, this, newItem, UpdateCommandType.Insert);
            
        //    return res;
        //}


        public UserProfile() { }

        public UserProfile(string userData, string userName)
        {
            UserName = userName;
            Decode(userData);
        }

        public UserProfile(FormsIdentity identity) 
        {
            UserName = identity.Name;
            Decode(identity.Ticket.UserData);
        }

        //[EntityProperty]
        //public string DisplayName { get; set; }


        //[EntityProperty(EntityPropertyType.Identity)]
        //public int UserId { get; set; }
        //[EntityProperty]
        //public int UserRole { get; set; }
        //[EntityProperty]
        //public string UserName { get; set; }
        //[EntityProperty]
        //public string Email { get; set; }
        //[EntityProperty]
        //public string Phone { get; set; }
        ////[EntityProperty]
        ////public int ApplicationId { get; set; }
        //[EntityProperty]
        //public int AccountId { get; set; }
        //[EntityProperty]
        //public string Lang { get; set; }
        ////[EntityProperty]
        ////public string Perms { get; set; }
        ////[EntityProperty]
        ////public int OwnerId { get; set; }
        //[EntityProperty]
        //public int Evaluation { get; set; }
        //[EntityProperty]
        //public bool IsBlocked { get; set; }
        //[EntityProperty]
        //public DateTime Creation { get; set; }



        //public string UserData(int ApplicationId, int OwnerId, string Perms)
        //{
        //    return string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", ApplicationId, AccountId, UserId, UserRole, OwnerId, Lang, Perms);
        //}

        #region User Data Json

        public void SetUserDataJson()
        {
            DataJson = UserDataContext.GetUserDataJson(AccountId, UserId);
        }

        [EntityProperty(EntityPropertyType.NA)]
        public string DataJson { get; set; }
        public string UserDataJson()
        {
            string genericData = Data == null ? "" : Data.ToJson();
            object[] args = new object[] { "acid", AccountId, "uid", UserId, "role", UserRole, "lang", Lang, "acname", AccountName, "category", AccountCategory, "uname", DisplayName, "pid", ParentId, "data", genericData };
            return JsonSerializer.ConvertToJson(args, null);

            //return ""+ string.Format("aid:{0},uid:{1},role:{2},lang:{3},aname:{4},category:{5},uname{6},pid:{7},data:{8}", AccountId, UserId, UserRole, Lang, AccountName, AccountCategory, DisplayName, ParentId, genericData)+""+ DataJson;
        }
        public static string UserDataToJson(string userData)
        {
            string[] data = userData.Split(DataSplitterCh);
            string genericData = "";
            string dataargs = data[8];
            if (!string.IsNullOrEmpty(dataargs))
            {
                dataargs = BaseConverter.UnEscape(dataargs, DataSplitter, DataSplitEscape);
                genericData = JsonSerializer.ConvertToJson(dataargs.Split('|'), null);
            }

            data[4] = BaseConverter.UnEscape(data[4], DataSplitter, DataSplitEscape);
            data[6] = BaseConverter.UnEscape(data[6], DataSplitter, DataSplitEscape);

            object[] args = new object[] { "acid", data[0], "uid", data[1], "role", data[2], "lang", data[3], "acname", data[4], "category", data[5], "uname", data[6], "pid", data[7], "data", genericData };
            return JsonSerializer.ConvertToJson(args, null);
        }
        #endregion

        [EntityProperty(EntityPropertyType.NA)]
        public NameValueArgs Claims { get; set; }
        public string ClaimsSerilaize()
        {
            string genericData = (Claims != null) ? Claims.ToKeyValuePipe() : null;
            return genericData;
        }
        public NameValueArgs ClaimsDeserilaize(string data)
        {
            string[] args= data.SplitTrim('|');
            NameValueArgs claims = new NameValueArgs(args);
            return claims;
        }

        public void SetUserDataEx()
        {
            Data = UserDataContext.GetUserDataEx(AccountId, UserId);
        }

        [EntityProperty(EntityPropertyType.NA) ]
        public NameValueArgs Data { get; set; }

        public string GetDataValue(string key)
        {
            if (Data == null)
                return "";
            return Data.Get(key);
        }
        public T GetDataValue<T>(string key)
        {
            if(Data==null)
                return default(T);
            return Data.Get<T>(key);
        }

        public string UserData()
        {
            string genericData = (Data != null) ? Data.ToKeyValuePipe() : null;

            return string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}", AccountId, UserId, UserRole, Lang, BaseConverter.Escape(AccountName, UserProfile.DataSplitter, DataSplitEscape), AccountCategory, BaseConverter.Escape(DisplayName, DataSplitter, DataSplitEscape), ParentId, BaseConverter.Escape(genericData, DataSplitter, DataSplitEscape));
        }

        public bool IsAuthenticated
        {
            get { return UserId > 0; }
        }

        [EntityProperty(EntityPropertyType.NA)]
        public PermsValue DefaultRule
        {
            get
            {
                if (UserRole >= 5) //UserRole.Manager
                    return PermsValue.FullControl;
                if (UserRole == 2)  //UserRole.Super
                    return PermsValue.Modify;
                //if (UserRole > 1)
                //    return PermsValue.Add;
                //if (UserRole >= 1)
                //    return PermsValue.Write;
                if (UserRole == 1) //UserRole.User
                    return PermsValue.Read;

                return PermsValue.None;

            }
        }

        //public bool IsLessThenManager
        //{
        //    get { return UserRole < 5; }
        //}
        //public bool IsManager
        //{
        //    get { return UserRole >=5; }
        //}
        public bool IsAdmin
        {
            get { return UserRole == 9; }
        }
        //public bool IsMaster
        //{
        //    get { return UserRole >= 7; }
        //}

        //public bool IsManagerOrAdmin
        //{
        //    get { return UserRole == 5 || UserRole==9; }
        //}

        //public string Encode()
        //{
        //    return string.Format("{0}-{1}-{2}-{3}", ApplicationId, AccountId, UserId, UserType);
        //}

        bool Decode(string userData)
        {
            //int applicationId = 0;
            int accountId = 0;
            int userId = 0;
            int userRole = 0;
            int accountCategory = 0;
            string lang = "he";
            string accountName = null;
            string displayName=null;
            int parentId=0;
            string dataConfig = null;

            if (GenericArgs.SplitArgs<int, int, int, string, string, int, string, int, string>(userData, DataSplitterCh, ref accountId, ref userId, ref userRole, ref lang, ref accountName, ref accountCategory, ref displayName, ref parentId, ref dataConfig))
            {
                UserId = userId;
                AccountId = accountId;
                UserRole = userRole;
                Lang = lang;
                AccountCategory = 0;
                AccountName = BaseConverter.UnEscape(accountName, DataSplitter, DataSplitEscape);
                DisplayName = BaseConverter.UnEscape(displayName, DataSplitter, DataSplitEscape);
                ParentId = parentId;
                var args = BaseConverter.UnEscape(dataConfig, DataSplitter, DataSplitEscape);
                if (!string.IsNullOrEmpty(args))
                {
                    var array= args.Split('|');
                    Data = NameValueArgs.Get(array);
                    //Data = NameValueArgs.Create(args);// GenericRecord.Parse(BaseConverter.UnEscape(dataConfig, "-", "%"));
                }
                return true;
            }

            //if (GenericArgs.SplitArgs<int, int, int, int, int, string, string>(userData, '-', ref applicationId, ref accountId, ref userId, ref userRole, ref ownerId, ref lang, ref perms))
            //{
            //    UserId = userId;
            //    AccountId = accountId;
            //    UserRole = userRole;
            //    ApplicationId = applicationId;
            //    OwnerId = ownerId;
            //    Lang = lang;
            //    Perms = perms;
            //    return true;
            //}
            return false;
        }

        public static UserProfile Get(string userData)
        {
            var user= new UserProfile();
            if (user.Decode(userData))
                return user;
            return null;
         }
    }
    //AuthState
    public interface ISignedUser : IUserProfile,IUser
    {
        //[EntityProperty]
        //int State { get; set; }

        PermsValue DefaultRule { get;}
            
        int EvaluationDays { get; set; }
    }

    public interface IUserProfile : IEntityItem
    {
        #region Properties
        [EntityProperty]
        string DisplayName { get; set; }
        [EntityProperty(EntityPropertyType.Identity)]
        int UserId { get; set; }
        //[EntityProperty]
        //int UserRole { get; set; }
        //[EntityProperty]
        //string UserName { get; set; }
        //[EntityProperty]
        //string Email { get; set; }
        //[EntityProperty]
        //string Phone { get; set; }
        //[EntityProperty]
        //int AccountId { get; set; }
        [EntityProperty]
        string Lang { get; set; }
        //[EntityProperty]
        //int Evaluation { get; set; }
        [EntityProperty]
        bool IsBlocked { get; set; }
        [EntityProperty]
        DateTime Creation { get; set; }

        [EntityProperty(EntityPropertyType.View)]
        int AccountCategory { get; set; }
        [EntityProperty(EntityPropertyType.View)]
        string AccountName { get; set; }

        [EntityProperty]
        int ParentId { get; set; }
        [EntityProperty]
        int Profession { get; set; }
        [EntityProperty]
        bool IsVirtual { get; set; }

        #endregion

        [EntityProperty(EntityPropertyType.NA)]
        NameValueArgs Claims { get; set; }
        [EntityProperty(EntityPropertyType.NA)]
        NameValueArgs Data { get; set; }

        void SetUserDataEx();

        //string UserData();

        //bool IsAuthenticated { get; }

    }
}
