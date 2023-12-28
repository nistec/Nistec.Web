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
        public static UserResult Get(MembershipStatus state)
        {

            bool IsOk = false;
            string Message = null;

            switch ((MembershipStatus)state)
            {
                case MembershipStatus.Success:
                    IsOk = true;
                    Message = "התהליך הושלם בהצלחה";
                    break;
                case MembershipStatus.Error:
                    Message = "אירעה שגיאה, נא לנסות שוב, אחרת נא לפנות לתמיכה";
                    break;
                case MembershipStatus.CouldNotResetPassword:
                    Message = "לא ניתן לאתחל את סיסמתך במערכת, נא לנסות שוב, אחרת נא לפנות לתמיכה";
                    break;
                case MembershipStatus.DuplicateUserNameOrEmail:
                    Message = "שם משתמש או כתובת דואל כבר קיימים במערכת, נא לבחור שם משתמש או כתובת דואל אחרים";
                    break;
                case MembershipStatus.InvalidAccountPath:
                    Message = "נתונים שגויים, לא הוגדר נתיב לחשבון הנוכחי, נא לפנות לתמיכה";
                    break;
                case MembershipStatus.InvalidEmailFormat:
                    Message = "כתובת הדואל אינה תקינה";
                    break;
                case MembershipStatus.InvalidPasswordFormat:
                    Message = "הסיסמה שהוקלדה אינה תואמת את כללי הסיסמאות במערכת, נא לנסות שוב עם סיסמה שונה";
                    break;
                case MembershipStatus.UserNameOrEmailNotExists:
                    Message = "שם משתמש או כתובת דואל שציינת אינם מוכרים במערכת, נא לפנות לתמיכה";
                    break;
                case MembershipStatus.UserRejected:
                    Message = "בקשתך נדחתה, נא לפנות לתמיכה";
                    break;
                case MembershipStatus.MembershipNotExists:
                    Message = "לא נמצאו הגדרות הזדהות במערכת, נא לפנות לתמיכה";
                    break;
                case MembershipStatus.UserIsBlocked:
                    Message = "אינך מורשה לשימוש במערכת, נא לפנות למנהל המערכת";
                    break;
                case MembershipStatus.ResetTokenSent:
                    IsOk = true;
                    Message = "בשלב זה נשלח לכתובת הדואל שלך הודעה לאתחול סיסמה, נא לפעול בהתאם להוראות שנשלחו לתיבת הדאר שלך";
                    break;
                case MembershipStatus.InvalidUser:
                    Message = "פרטי המשתמש שצויינו אינם מוכרים במערכת, נא לפנות לתמיכה";
                    break;
                case MembershipStatus.InvalidTokenFormt:
                    Message = "קוד הפנייה לאתחול סיסמה אינו תקין, נא לפנות לתמיכה";
                    break;
                case MembershipStatus.UserPasswordWasReset:
                    IsOk = true;
                    Message = "סיסמתך אותחלה בהצלחה, כעת ניתן להיכנס למערכת עם פרטי ההזדהות החדשים";
                    break;
                case MembershipStatus.TokenVerificationExpired:
                    IsOk = true;
                    Message = "פג תוקפו של קוד הפנייה לאתחול סיסמה";
                    break;
            }

            //if (type == "membershipstatus")
            //{
            //    switch ((MembershipStatus)code)
            //    {
            //        case MembershipStatus.Success:
            //            IsOk = true;
            //            Message = "התהליך הושלם בהצלחה";
            //            break;
            //        case MembershipStatus.Error:
            //            Message = "אירעה שגיאה, נא לנסות שוב, אחרת נא לפנות לתמיכה";
            //            break;
            //        case MembershipStatus.CouldNotResetPassword:
            //            Message = "לא ניתן לאתחל את סיסמתך במערכת, נא לנסות שוב, אחרת נא לפנות לתמיכה";
            //            break;
            //        case MembershipStatus.DuplicateUserNameOrEmail:
            //            Message = "שם משתמש או כתובת דואל כבר קיימים במערכת, נא לבחור שם משתמש או כתובת דואל אחרים";
            //            break;
            //        case MembershipStatus.InvalidAccountPath:
            //            Message = "נתונים שגויים, לא הוגדר נתיב לחשבון הנוכחי, נא לפנות לתמיכה";
            //            break;
            //        case MembershipStatus.InvalidEmailFormat:
            //            Message = "כתובת הדואל אינה תקינה";
            //            break;
            //        case MembershipStatus.InvalidPasswordFormat:
            //            Message = "הסיסמה שהוקלדה אינה תואמת את כללי הסיסמאות במערכת, נא לנסות שוב עם סיסמה שונה";
            //            break;
            //        case MembershipStatus.UserNameOrEmailNotExists:
            //            Message = "שם משתמש או כתובת דואל שציינת אינם מוכרים במערכת, נא לפנות לתמיכה";
            //            break;
            //        case MembershipStatus.UserRejected:
            //            Message = "בקשתך נדחתה, נא לפנות לתמיכה";
            //            break;
            //        case MembershipStatus.MembershipNotExists:
            //            Message = "לא נמצאו הגדרות הזדהות במערכת, נא לפנות לתמיכה";
            //            break;
            //        case MembershipStatus.UserIsBlocked:
            //            Message = "אינך מורשה לשימוש במערכת, נא לפנות למנהל המערכת";
            //            break;
            //        case MembershipStatus.ResetTokenSent:
            //            IsOk = true;
            //            Message = "בשלב זה נשלח לכתובת הדואל שלך הודעה לאתחול סיסמה, נא לפעול בהתאם להוראות שנשלחו לתיבת הדאר שלך";
            //            break;
            //        case MembershipStatus.InvalidUser:
            //            Message = "פרטי המשתמש שצויינו אינם מוכרים במערכת, נא לפנות לתמיכה";
            //            break;
            //        case MembershipStatus.InvalidTokenFormt:
            //            Message = "קוד הפנייה לאתחול סיסמה אינו תקין, נא לפנות לתמיכה";
            //            break;
            //        case MembershipStatus.UserPasswordWasReset:
            //            IsOk = true;
            //            Message = "סיסמתך אותחלה בהצלחה, כעת ניתן להיכנס למערכת עם פרטי ההזדהות החדשים";
            //            break;
            //        case MembershipStatus.TokenVerificationExpired:
            //            IsOk = true;
            //            Message = "פג תוקפו של קוד הפנייה לאתחול סיסמה";
            //            break;
            //    }
            //}
            //else if (type == "authstate")
            //{
            //    switch ((AuthState)code)
            //    {
            //        case AuthState.Succeeded:
            //            IsOk = true;
            //            Message = "התהליך הושלם בהצלחה";
            //            break;
            //        case AuthState.EvaluationExpired:
            //            Message = "זמן הנסיון במערכת הסתיים, נא לפנות לתמיכה";
            //            break;
            //        case AuthState.Failed:
            //            Message = "התהליך נכשל";
            //            break;
            //        case AuthState.IpNotAlowed:
            //            Message = "אין הרשאה לפעולה מהכתובת הנוכחית";
            //            break;
            //        case AuthState.NonConfirmed:
            //            Message = "אין אישור לפעולה המבוקשת";
            //            break;
            //        case AuthState.UnAuthorized:
            //            Message = "פרטי ההזדהות אינם מוכרים במערכת";
            //            break;
            //        case AuthState.UserNotRemoved:
            //            Message = "המשתמש לא הוסר";
            //            break;
            //        case AuthState.UserNotUpdated:
            //            Message = "פרטי המשתמש לא עודכנו במערכת";
            //            break;
            //        case AuthState.UserRemoved:
            //            Message = "המשתמש הוסר מהמערכת";
            //            break;
            //        case AuthState.UserUpdated:
            //            Message = "פרטי המשתמש עודכנו";
            //            break;
            //    }
            //}

            int status = IsOk ? 10 : -1;
            return new UserResult() { Status = status, Message = Message };
        }

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
                case AuthState.UserNotAllowed:// = 5,//user removed
                    desc = "User not allowed"; break;
                case AuthState.UserNotExists:// = 6,//user not removed
                    desc = "User not exists"; break;
                case AuthState.PasswordShouldChange:// = 7,//user updated
                    desc = "Password Should Change"; break;
                case AuthState.PasswordManyFailures:// = 8,//user updated
                    desc = "Many Failures"; break;
                case AuthState.Succeeded:// = 10//--10=ok
                    desc = "Ok"; break;
            }
            return new UserResult() { Status = (int)state, Message = desc };
        }

        public static UserResult Get(UserUpdateState state)
        {
            string desc = "";
            switch (state)
            {
                case UserUpdateState.Failed:// = -1,
                    desc = "Internal error"; break;
                case UserUpdateState.UnAuthorized:// = 0, //--0=auth faild
                    desc = "User un authorized"; break;
                case UserUpdateState.IpNotAlowed:// = 1,//--1=ip not alowed
                    desc = "Ip not alowed"; break;
                case UserUpdateState.EvaluationExpired:// = 2,//--2=Evaluation expired
                    desc = "Evaluation expired"; break;
                case UserUpdateState.Blocked:// = 3,//--3=account blocked
                    desc = "User is blocked"; break;
                case UserUpdateState.NonConfirmed:// = 4,//--4=non confirmed, username or password exists
                    desc = "Non confirmed, UserName allready exists"; break;
                case UserUpdateState.UserNotAllowed:// = 5,//user removed
                    desc = "User np allowed"; break;
                case UserUpdateState.UserNotExists:// = 6,//user not removed
                    desc = "User not exists"; break;
                case UserUpdateState.UserUpdated:// = 7,//user updated
                    desc = "User Updated"; break;
                case UserUpdateState.UserNotUpdated:// = 8,//user updated
                    desc = "User Not Updated"; break;
                case UserUpdateState.Succeeded:// = 10//--10=ok
                    desc = "Ok"; break;
            }
            return new UserResult() { Status = (int)state, Message = desc };
        }

        public static UserResult IsDeleted(int result)
        {
            if (result == -1)
                return Get(UserUpdateState.Failed);
            if (result == 10)
                return Get(UserUpdateState.Succeeded);//.UserRemoved);
            else
                return Get(UserUpdateState.Failed);//.UserNotRemoved);
        }

        public static UserResult IsUpdated(int result)
        {
            if (result == -1)
                return Get(UserUpdateState.Failed);
            if (result == 1)
                return Get(UserUpdateState.UserUpdated);
            else
                return Get(UserUpdateState.UserNotUpdated);
        }

        
        [EntityProperty]
        public int Status { get; set; }
        [EntityProperty(EntityPropertyType.NA)]
        public string Message { get; set; }

        //[EntityProperty(EntityPropertyType.NA)]
        //public bool Commit
        //{
        //    get
        //    {
        //        switch ((AuthState)Status)
        //        {
        //            case AuthState.UserRemoved:// = 5,//user removed
        //            //case AuthState.UserUpdated:// = 7,//user updated
        //            case AuthState.Succeeded:// = 10//--10=ok
        //                return true;
        //            default:
        //                return false;
        //        }
        //    }
        //}
    }
}
