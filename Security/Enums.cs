using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Security
{
    public enum TokenType
    {
        PasswordVerificationToken = 0,
        ConfirmationToken = 1
    }

    public enum UserDataVersion
    {
        Json,
        DataJson,
        DataPipe
    }

    public enum UserEvaluation
    {
        Active = 0,
        Trial = 1,
        Blocked = 2
    }

    /*
     AuthState	-109	Action not allowed	0
AuthState	-108	PasswordManyFailures	0
AuthState	-107	PasswordShouldChange	0
AuthState	-106	User was not removed ??	0
AuthState	-105	User not allowed	0
AuthState	-104	Non Confirmed	0
AuthState	-103	User or Account is blocked	0
AuthState	-102	Evaluation expired	0
AuthState	-101	Ip not alowed	0
AuthState	-100	Un Authorized	0
AuthState	-1	Error	0
AuthState	0	Un Authorized	0
AuthState	1	Ip not alowed	0
AuthState	2	Evaluation expired	0
AuthState	3	User or Account is blocked	0
AuthState	4	Non Confirmed	0
AuthState	5	User not allowed	0
AuthState	6	User was not removed ??	0
AuthState	7	PasswordShouldChange	0
AuthState	8	PasswordManyFailures	0
AuthState	9	Action not allowed	0
AuthState	10	ok	0
AuthState	200	ok	0
     */

    public enum AuthState_NEW
    {
        Failed = -1,
        UnAuthorized = -100, //--0=auth faild
        IpNotAlowed = -101,//--1=ip not alowed
        EvaluationExpired = -102,//--2=Evaluation expired
        Blocked = -103,//--3=account blocked
        NonConfirmed = -104,//--4=non confirmed, username or password exists
        //UserRemoved = 5,//user removed
        //UserNotRemoved = 6,//user not removed
        //ShouldOtp = 5,
        UserNotAllowed = -105,
        UserNotExists = -106,
        PasswordShouldChange = -107,//UserUpdated = 7,//not used
        PasswordManyFailures = -108,// UserNotUpdated = 8,//not used
        ActionNotAllowed = -109,//Action not allowed
        Succeeded = 200//--10=ok

    }

    public enum AuthState
    {
        ShouldOtp = -102,
        UserNotAllowed = -106,
        UserNotExists=-105,
        Failed = -1,
        UnAuthorized = 0, //--0=auth faild
        IpNotAlowed = 1,//--1=ip not alowed
        EvaluationExpired = 2,//--2=Evaluation expired
        Blocked = 3,//--3=account blocked
        NonConfirmed = 4,//--4=non confirmed, username or password exists
        //UserRemoved = 5,//user removed
        //UserNotRemoved = 6,//user not removed
        PasswordShouldChange = 7,//UserUpdated = 7,//not used
        PasswordManyFailures = 8,// UserNotUpdated = 8,//not used
        ActionNotAllowed = 9,//Action not allowed
        Succeeded = 10//--10=ok
    }


    public enum UserUpdateState
    {
        ShouldOtp = -102,
        UserNotAllowed = -106,
        UserNotExists = -105,
        Failed = -1,
        UnAuthorized = 0, //--0=auth faild
        IpNotAlowed = 1,//--1=ip not alowed
        EvaluationExpired = 2,//--2=Evaluation expired
        Blocked = 3,//--3=account blocked
        NonConfirmed = 4,//--4=non confirmed, username or password exists
        //UserNotAllowed = 5,//user removed
        //UserNotExists = 6,//user not removed
        UserUpdated = 7,//not used
        UserNotUpdated = 8,//not used
        Succeeded = 10//--10=ok

    }
    //public enum UserSole
    //{
    //    User = 0,
    //    Super = 1,
    //    Admin = 9
    //}

    public enum MembershipStatus
    {
        Error = -1,
        Success = 0,//     The user was successfully created.
        UserNameOrEmailNotExists = 1,//     The user name was not found in the database.
        InvalidPasswordFormat = 2,//     The password is not formatted correctly.
        InvalidEmailFormat = 3,//     The e-mail address is not formatted correctly.
        DuplicateUserNameOrEmail = 4,//     The user name already exists in the database for the application.
        UserRejected = 5,//     The user was not created, for a reason defined by the provider.
        CouldNotResetPassword = 6,//     Internal error, could not reset password.
        InvalidAccountPath = 7,
        MembershipNotExists = 8,
        UserIsBlocked = 9,
        ResetTokenSent = 10,
        InvalidUser = 11,
        InvalidTokenFormt = 12,
        TokenVerificationExpired = 13,
        UserPasswordWasReset = 20
    }
    //public enum MembershipStatus
    //{

    //    Success                     = 0,//     The user was successfully created.
    //    UserNameNotExists           = 1,//     The user name was not found in the database.
    //    InvalidPasswordFormat       = 2,//     The password is not formatted correctly.
    //    //InvalidQuestionFormat       = 3,//     The password question is not formatted correctly.
    //    //InvalidAnswerFormat         = 4,//     The password answer is not formatted correctly.
    //    InvalidEmailFormat          = 5,//     The e-mail address is not formatted correctly.
    //    DuplicateUserName           = 6,//     The user name already exists in the database for the application.
    //    DuplicateEmail              = 7,//     The e-mail address already exists in the database for the application.
    //    UserRejected                = 8,//     The user was not created, for a reason defined by the provider.
    //    //InvalidProviderUserKey      = 9,//     The provider user key is of an invalid type or format.
    //    //DuplicateProviderUserKey    = 10,//     The provider user key already exists in the database for the application.
    //    //ProviderError               = 11,//     The provider returned an error that is not described by other System.Web.Security.MembershipCreateStatus enumeration values.
    //    EmailNotExists              = 12,//     The email was not found in the database.
    //    CouldNotResetPassword       = 13,//     Internal error, could not reset password.
    //    InvalidAccountPath          = 14
    //}

    //public class UserMessages
    //{
    //public static string ErrorCodeToString(MembershipCreateStatus createStatus)
    //    {
    //        // See http://go.microsoft.com/fwlink/?LinkID=177550 for
    //        // a full list of status codes.
    //        switch (createStatus)
    //        {
    //            case MembershipCreateStatus.DuplicateUserName:
    //                return "User name already exists. Please enter a different user name.";

    //            case MembershipCreateStatus.DuplicateEmail:
    //                return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

    //            case MembershipCreateStatus.InvalidPassword:
    //                return "The password provided is invalid. Please enter a valid password value.";

    //            case MembershipCreateStatus.InvalidEmail:
    //                return "The e-mail address provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidAnswer:
    //                return "The password retrieval answer provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidQuestion:
    //                return "The password retrieval question provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidUserName:
    //                return "The user name provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.ProviderError:
    //                return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

    //            case MembershipCreateStatus.UserRejected:
    //                return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

    //            default:
    //                return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
    //        }
    //    }



    //}
}
