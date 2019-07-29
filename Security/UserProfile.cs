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

    public class UserRegister : UserProfile
    {
        public UserRegister() {
            this.Creation = DateTime.Now;
            this.Modified = DateTime.Now;
        }

        //[EntityProperty]
        //public string Email { get; set; }
        //[EntityProperty]
        //public string Phone { get; set; }
        //[EntityProperty]
        //public DateTime Creation { get; set; }
        //[EntityProperty]
        //public bool IsBlocked { get; set; }

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
        [EntityProperty(EntityPropertyType.View)]
        public int AccountCategory { get; set; }
        [EntityProperty(EntityPropertyType.View)]
        public string AccountName { get; set; }

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
        public int AccountId { get; set; }
        [EntityProperty]
        public int UserRole { get; set; }
        [EntityProperty]
        public string UserName { get; set; }
        [EntityProperty]
        public string Email { get; set; }
        [EntityProperty]
        public string Phone { get; set; }
        [EntityProperty]
        public string Lang { get; set; }
        [EntityProperty]
        public UserEvaluation Evaluation { get; set; }
        [EntityProperty]
        public bool IsBlocked { get; set; }
    

        [EntityProperty(EntityPropertyType.View)]
        public DateTime Creation { get; set; }
        [EntityProperty]
        public DateTime Modified { get; set; }
        [EntityProperty]
        public string Profession { get; set; }

        [EntityProperty]
        public int ParentId { get; set; }
        [EntityProperty]
        public bool IsVirtual { get; set; }
       
    }

    public class UserProfile : UserItem, IUserProfile
    {
        public const string MappingName = "Ad_UserProfile";
        public const char DataSplitterCh = '-';
        public const string DataSplitter = "-";
        public const string DataSplitEscape = "%";

        public UserProfile Copy() {
            return (UserProfile)this.MemberwiseClone();
        }

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

        public static NameValueArgs DecodeUserData(string userData)
        {
            NameValueArgs Data = NameValueArgs.ParseJson(userData);
            if (Data == null || Data.Count == 0)
            {
                throw new Exception("Invalid User Data");
            }
            return Data;
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

        //public UserProfile(string userData, string userName)
        //{
        //    UserName = userName;
        //    Decode(userData);
        //}

        //public UserProfile(FormsIdentity identity, UserDataVersion version) 
        //{
        //    UserName = identity.Name;

        //    Decode(identity.Ticket.UserData, version);
        //}


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

        public bool IsAdmin
        {
            get { return UserRole == 9; }
        }
        
        

        //public static UserProfile Get(string userData)
        //{
        //    var user= new UserProfile();
        //    if (user.Decode(userData))
        //        return user;
        //    return null;
        // }
    }
    //AuthState
    public interface ISignedUser : IUserProfile//,IUser
    {
        //[EntityProperty]
        //int State { get; set; }
        UserDataVersion Version { get; set; }
        PermsValue DefaultRule { get;}
            
        int EvaluationDays { get; set; }
        string HostClient { get; set; }
        string AppName { get; set; }

        int State { get; set; }
        bool IsAuthenticated { get; }
        //[EntityProperty]
        //bool IsAdmin { get; }

        [EntityProperty(EntityPropertyType.View)]
        int AccountCategory { get; set; }
        [EntityProperty(EntityPropertyType.View)]
        string AccountName { get; set; }
        [EntityProperty(EntityPropertyType.View)]
        int AccType { get; set; }
        [EntityProperty(EntityPropertyType.View)]
        string AccAccess { get; set; }

        [EntityProperty(EntityPropertyType.NA)]
        NameValueArgs Claims { get; set; }
        [EntityProperty(EntityPropertyType.NA)]
        NameValueArgs Data { get; set; }

        void SetUserDataEx(UserDataVersion version);

        string UserData(UserDataVersion version);

        //string UserData();

        //bool IsAuthenticated { get; }

    }

    public interface IUserProfile : IEntityItem
    {

        #region IUser

        [EntityProperty(EntityPropertyType.Identity)]
        int UserId { get; set; }
        [EntityProperty]
        string DisplayName { get; set; }
        [EntityProperty]
        int UserRole { get; set; }
        [EntityProperty]
        string UserName { get; set; }
        [EntityProperty]
        string Email { get; set; }
        [EntityProperty]
        string Phone { get; set; }
        [EntityProperty]
        int AccountId { get; set; }
        [EntityProperty]
        string Lang { get; set; }
        [EntityProperty]
        UserEvaluation Evaluation { get; set; }
        [EntityProperty]
        bool IsBlocked { get; set; }
        //[EntityProperty]
        //DateTime Creation { get; set; }

        //string UserData();

        #endregion

        #region Properties
        //[EntityProperty]
        //string DisplayName { get; set; }
        //[EntityProperty(EntityPropertyType.Identity)]
        //int UserId { get; set; }
        // [EntityProperty]
        //string Lang { get; set; }

        [EntityProperty]
        int ParentId { get; set; }
        //[EntityProperty]
        //int Profession { get; set; }
        [EntityProperty]
        bool IsVirtual { get; set; }

        #endregion

        

    }
}
