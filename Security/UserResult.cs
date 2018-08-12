using Nistec.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Security
{
    public static class UserResultExtension
    {
        public static void SetLang(this UserResult ur, string lang = "en")
        {
            switch (lang)
            {
                case "he":
                    SetLang(ur); break;
            }
        }
        //internal static void SetLangHe(UserResult ur)
        //{
        //    switch ((AuthState)ur.Status)
        //    {
        //        case AuthState.Failed:// = -1,
        //            ur.Description = "אירעה שגיאה"; break;
        //        case AuthState.UnAuthorized:// = 0, //--0=auth faild
        //            ur.Description = "פרטי ההזדהות אינם מוכרים במערכת"; break;
        //        case AuthState.IpNotAlowed:// = 1,//--1=ip not alowed
        //            ur.Description = "כתובת השרת אינה מוכרת במערכת"; break;
        //        case AuthState.EvaluationExpired:// = 2,//--2=Evaluation expired
        //            ur.Description = "תוקף תקופת הנסיון הסתיים"; break;
        //        case AuthState.Blocked:// = 3,//--3=account blocked
        //            ur.Description = "משתמש חסום במערכת"; break;
        //        case AuthState.NonConfirmed:// = 4,//--4=non confirmed, username or password exists
        //            ur.Description = "שם משתמש כבר קיים במערכת"; break;
        //        case AuthState.UserRemoved:// = 5,//user removed
        //            ur.Description = "המשתמש הוסר מהמערכת"; break;
        //        case AuthState.UserNotRemoved:// = 6,//user not removed
        //            ur.Description = "המשתמש לא הוסר מהמערכת"; break;
        //        case AuthState.UserUpdated:// = 7,//user updated
        //            ur.Description = "פרטי המשתמש עודכנו במערכת"; break;
        //        case AuthState.UserNotUpdated:// = 7,//user updated
        //            ur.Description = "פרטי המשתמש לא עודכנו במערכת"; break;
        //        case AuthState.Succeeded:// = 10//--10=ok
        //            ur.Description = "Ok"; break;
        //    }
        //}

    }

    public class UserResult : IEntityItem
    {
         public static UserResult Get(AuthState state)
        {
            string desc = "";
            switch (state)
            {
                case AuthState.Failed:// = -1,
                    desc = "Internal error"; break;
                case AuthState.UnAuthorized:// = 0, //--0=auth faild
                    desc = "User un authorized"; break;
                case AuthState.IpNotAlowed:// = 1,//--1=ip not alowed
                    desc = "Ip not alowed"; break;
                case AuthState.EvaluationExpired:// = 2,//--2=Evaluation expired
                    desc = "Evaluation expired"; break;
                case AuthState.Blocked:// = 3,//--3=account blocked
                    desc = "User is blocked"; break;
                case AuthState.NonConfirmed:// = 4,//--4=non confirmed, username or password exists
                    desc = "Non confirmed, UserName allready exists"; break;
                case AuthState.UserRemoved:// = 5,//user removed
                    desc = "User removed"; break;
                case AuthState.UserNotRemoved:// = 6,//user not removed
                    desc = "User not removed"; break;
                //case AuthState.UserUpdated:// = 7,//user updated
                //    desc = "User updated"; break;
                //case AuthState.UserNotUpdated:// = 7,//user updated
                //    desc = "User not updated"; break;
                case AuthState.Succeeded:// = 10//--10=ok
                    desc = "Ok"; break;
            }
            return new UserResult() { Status = (int)state, Description = desc };
        }

        public static UserResult IsDeleted(int result)
        {
            if (result == -1)
                return Get(AuthState.Failed);
            if (result == 1)
                return Get(AuthState.UserRemoved);
            else
                return Get(AuthState.UserNotRemoved);
        }

        public static UserResult IsUpdated(int result)
        {
            if (result == -1)
                return Get(AuthState.Failed);
            if (result == 1)
                return Get(AuthState.UserUpdated);
            else
                return Get(AuthState.UserNotUpdated);
        }

        
        [EntityProperty]
        public int Status { get; set; }
        [EntityProperty(EntityPropertyType.NA)]
        public string Description { get; set; }

        [EntityProperty(EntityPropertyType.NA)]
        public bool Commit
        {
            get
            {
                switch ((AuthState)Status)
                {
                    case AuthState.UserRemoved:// = 5,//user removed
                    //case AuthState.UserUpdated:// = 7,//user updated
                    case AuthState.Succeeded:// = 10//--10=ok
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}
