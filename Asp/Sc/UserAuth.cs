using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Nistec.Runtime;
using Nistec.Data.Entities;
using Nistec.Data;
using Nistec.Generic;
using Nistec.Channels.RemoteCache;


namespace Nistec.Web.Asp
{

    [Entity("UserAuth", "vw_Users_Auth", "cnn_Docs", EntityMode.Generic, "LogInName,Pass")]
    public class UserAuth_Context : EntityContext<UserAuth>
    {
        #region ctor

        public UserAuth_Context(DataFilter filter)
            : base()
        {
            Init(filter);
        }
        public UserAuth_Context(UserAuth item)
            : base(item)
        {

        }

        public UserAuth_Context(string LogInName, string Pass)
            : base(LogInName, Pass)
        {

        }

        protected UserAuth_Context()
            : base()
        {
        }
        #endregion

        #region binding

        protected override void EntityBind()
        {
            //base.EntityDb.EntityCulture = Nistec.Data.DB.NetcellDB.GetCulture();
            //If EntityAttribute not define you can initilaize the entity here
            //base.InitEntity<AdventureWorks>("Contact", "Person.Contact", EntityKeys.Get("ContactID"));

        }

        #endregion

        #region methods

        public static UserAuth Get(string LogInName, string Pass)
        {
            if (LogInName == null || Pass == null)
            {
                throw new NetException("IllegalAuthentication: user name or password");
            }
            if (!SecurityUtil.IsAlphaNumeric(LogInName, Pass))
            {
                throw new NetException("IllegalAuthentication: Illegal UserName or password, expect AlphaNumeric");
            }
            if (!SecurityUtil.IsValidString(LogInName) || !SecurityUtil.IsValidString(Pass))
            {
                throw new NetException("IllegalAuthentication: Illeagal user name or password");
            }
            using (UserAuth_Context context = new UserAuth_Context(LogInName,Pass))
            {
                //context.Set(KeySet.Get("LogInName", LogInName,"Pass", Pass);
                return context.Entity;
            }
        }

        public static UserAuth Get(DataRow dr)
        {
            using (UserAuth_Context context = new UserAuth_Context())
            {
                context.EntityRecord = new GenericRecord(dr);
                return context.Entity;
            }
        }

        public static List<UserAuth> GetList(DataFilter filter)
        {
            using (UserAuth_Context context = new UserAuth_Context())
            {
                return context.EntityList(filter);
            }
        }

        public string ToHtml(bool encrypt)
        {
            string h = this.EntityProperties.ToHtmlTable("", "");
            if (encrypt)
                return Encryption.ToBase64String(h, true);
            return h;
        }

        #endregion

    }

    [Entity("UserAuth", "vw_Users_Auth", "cnn_Docs", EntityMode.Generic, "UserId")]
    public class UserAuthId_Context : EntityContext<UserAuth>
    {
        #region ctor

        public UserAuthId_Context(int userId)
            : base(userId)
        {

        }

        protected UserAuthId_Context()
            : base()
        {
        }
        #endregion

        #region binding

        protected override void EntityBind()
        {
            //base.EntityDb.EntityCulture = Nistec.Data.DB.NetcellDB.GetCulture();
            //If EntityAttribute not define you can initilaize the entity here
            //base.InitEntity<AdventureWorks>("Contact", "Person.Contact", EntityKeys.Get("ContactID"));
        }

        #endregion

        #region methods

        public static UserAuth Get(int userId)
        {
            using (UserAuthId_Context context = new UserAuthId_Context(userId))
            {
                //context.Set(userId);
                return context.Entity;
            }
        }

        public static UserAuth GetByManager(int userId, int loginId)
        {
            using (UserAuthId_Context context = new UserAuthId_Context(userId))
            {
                //context.Set(userId);
                context.Entity.SetManager(loginId);
                return context.Entity;
            }
        }

        public static UserAuth Get(DataRow dr)
        {
            using (UserAuthId_Context context = new UserAuthId_Context())
            {
                context.EntityRecord = new GenericRecord(dr);
                return context.Entity;
            }
        }

        public static List<UserAuth> GetList(DataFilter filter)
        {
            using (UserAuthId_Context context = new UserAuthId_Context())
            {
                return context.EntityList(filter);
            }
        }

        public string ToHtml(bool encrypt)
        {
            string h = this.EntityProperties.ToHtmlTable("", "");
            if (encrypt)
                return Encryption.ToBase64String(h, true);
            return h;
        }

        #endregion
    }



    public class UserAuth : IEntityItem
    {
        #region ctor

        public UserAuth() { }
        /*
        public UserAuth(int userId)
        {
            Set(userId);
        }

        public UserAuth(int userId, int loginId)
        {
            Set(userId);
            SetManager(loginId);
        }

        public UserAuth(string LogInName, string Pass)
        {
            if (LogInName == null || Pass == null)
            {
                throw new NetException("IllegalAuthentication: user name or password");
            }
            if (!SecurityUtil.IsAlphaNumeric(LogInName, Pass))
            {
                throw new NetException("IllegalAuthentication: Illegal UserName or password, expect AlphaNumeric");
            }
            if (!SecurityUtil.IsValidString(LogInName) || !SecurityUtil.IsValidString(Pass))
            {
                throw new NetException("IllegalAuthentication: Illeagal user name or password");
            }
        }

        protected override void EntityBind()
        {
            
        }

        protected virtual void Set(int userId)
        {

        }

        protected virtual void Set(string LogInName, string Pass)
        {
 
        }


         public string ToHtml(bool encrypt)
        {
            string h = this.EntityProperties.ToHtmlTable("", "");
            if (encrypt)
                return Encryption.ToBase64String(h, true);
            return h;
        }
        */
        #endregion

        public static UserAuth Empty
        {
            get { return new UserAuth(); }
        }
        public bool IsValid
        {
            get { return AccountId > 0 && UserId > 0; }
        }
        public string DecodedPass
        {
            get { return Encryption.DecryptPass(Pass); }
        }

        public string EncryptAuth()
        {
            return Nistec.Runtime.RequestQuery.EncryptEx32(string.Format("{0}-{1}-{2}-{3}", UserId, AccountId, UserType, UserName));
        }

        public static bool DecryptAuth(string args, ref int UserId, ref int AccountId, ref int UserType, ref string UserName)
        {
            string arg = Nistec.Runtime.RequestQuery.DecryptEx32(args);
            return Nistec.Generic.GenericArgs.SplitArgs<int, int, int>(arg, '-', ref UserId, ref AccountId, ref UserType);
        }

        public bool IsEmpty
        {
            get { return UserId <= 0; }
        }

        #region properties

        [EntityProperty(EntityPropertyType.Identity)]
        public int UserId { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public string LogInName { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public string Pass { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public int AccountId { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public int UserType { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public string UserName { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public int AccType { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public string AccountName { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public int ParentId { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public int OwnerId { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public string LastLoggedIn { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public int Evaluation { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public bool ConfirmArticle { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public string OwnerFolder { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public string RootFolder { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public int ContactCapacity { get; set; }

        [EntityProperty(EntityPropertyType.Default)]
        public int FilesCapacity { get; set; }

        #endregion

        public bool IsUserManager
        {
            get
            {
                UserType ut = (UserType)UserType;
                return ut == Asp.UserType.Admin || ut == Asp.UserType.Manager;
            }
        }

        internal void SetManager(int loginId)
        {
            _LoginId = loginId;
            _IsManager = true;
        }

        bool _IsManager = false;
        public bool IsManagerControled
        {
            get { return _IsManager; }
        }
        int _LoginId = 0;
        public int LoginId
        {
            get
            {
                if (_LoginId <= 0)
                    return UserId;
                return _LoginId;
            }
        }

        // public string GetCacheArgs()
        //{
        //    return UserName + ", " + AccountId.ToString() + ", " + AccountName+", "+ LastLoggedIn;
        //}

        public string[] CacheArgs()
        {
            return new string[] { KnowsArgs.UserId, LoginId.ToString(), KnowsArgs.StrArgs, UserName + ", " + AccountId.ToString() + ", " + AccountName + ", " + LastLoggedIn };
        }
    }
}
