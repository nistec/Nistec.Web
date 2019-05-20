using Nistec.Data;
using Nistec.Data.Entities;
using Nistec.Generic;
using Nistec.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Nistec.Web.Security
{

  
    public class SignedUser : UserProfile, IUser, ISignedUser
    {
        internal const string SessionKey = "SignedUser";
        public static SignedUser Get(HttpContextBase context)
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
            signedUser = new SignedUser(formsIdentity);
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

        public static SignedUser Get(HttpContextBase context, UserRole role)
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
            signedUser = new SignedUser(formsIdentity);
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

        //public static SignedUser GetAdmin(HttpContextBase context)
        //{
        //    if (context == null || !context.Request.IsAuthenticated || !(context.User.Identity is FormsIdentity))
        //    {
        //        return NotAuthrized(AuthState.UnAuthorized, "Http error: Invalid HttpContext");
        //    }

        //    var formsIdentity = (FormsIdentity)context.User.Identity;
        //    var signedUser = new SignedUser(formsIdentity);
                
        //    //_isAuthenticated = signedUser.IsAuthenticated;
        //    if (signedUser.IsAuthenticated == false || signedUser.IsBlocked)
        //    {
        //        //Log.Fatal("User not Authenticated");
        //        return NotAuthrized(AuthState.UnAuthorized, "Authenticatation error: User not Authenticated");
        //    }
        //    if (signedUser.UserRole != 9)
        //    {
        //        //Log.Fatal("User not Authenticated");
        //        return NotAuthrized(AuthState.UnAuthorized, "Authenticatation error: Access is denied");
        //    }
        //    signedUser.IsMobile = DeviceHelper.IsMobile(context.Request);
        //    return signedUser;
        //}

        public static string GetJson(HttpContextBase context)
        {
            if (context == null || !context.Request.IsAuthenticated || !(context.User.Identity is FormsIdentity))
            {
                return JsonSerializer.ConvertToJson(new object[] { "state", AuthState.UnAuthorized, "desc", "Http error: Invalid HttpContext" },null);
            }
            string userData = null;
            var formsIdentity = (FormsIdentity)context.User.Identity;
            if(formsIdentity!=null)
            {
                userData = formsIdentity.Ticket.UserData;
            }
            if(userData==null)
            {
                return JsonSerializer.ConvertToJson(new object[] { "state", AuthState.UnAuthorized, "desc", "FormsIdentity error: Invalid User Data" }, null);
            }

            return UserProfile.UserDataToJson(userData);
        }

        internal static SignedUser NotAuthrized(AuthState state, string desc)
        {
            return new SignedUser() { State = (int)state, StateDescription = desc };
        }

        public SignedUser() { }

        public SignedUser(FormsIdentity identity)
            : base(identity)
        {
              
        }

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

        //[EntityProperty]
        //public int ApplicationId { get; set; }

        //public AuthState AuthState
        //{
        //    get { return (AuthState)State; }
        //}


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
    }

}
