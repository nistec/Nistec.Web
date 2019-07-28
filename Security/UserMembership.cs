using Nistec.Data;
using Nistec.Data.Entities;
using Nistec.Runtime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Nistec.Web.Security
{
    [EntityMapping("Ad_UserMembership")]
    public class UserMembership : IEntityItem
    {
        //public const string MappingName = "Ad_UserMembership";


        public static UserMembership Get(int UserId)
        {
            if (UserId <= 0)
            {
                throw new Exception("Authorizer_Context error,null UserId");
            }
            UserMembership user = null;
            using (Authorizer context = Authorizer.Instance)
            {
                user = context.EntityDb.DoCommand<UserMembership>("select * from [" + EntityMappingAttribute.Mapping<UserMembership>() + "] where UserId=@UserId", DataParameter.GetSql("UserId", UserId), System.Data.CommandType.Text);
            }
            if (user == null)
            {
                throw new Exception("AuthorizationException, Un Authorized UserMembership");
            }

            return user;
        }
        public static UserMembership Get(int UserId, string resetToken)
        {
            if (UserId <= 0)
            {
                throw new SecurityException((int)MembershipStatus.InvalidUser, "Authorizer_Context error,null UserId");
            }
            if (string.IsNullOrEmpty(resetToken))
            {
                throw new SecurityException((int)MembershipStatus.InvalidTokenFormt, "Authorizer_Context error,null resetToken memebership");
            }
            UserMembership user = null;
            string mappingName = EntityMappingAttribute.Mapping<UserMembership>();
            using (Authorizer context = Authorizer.Instance)
            {
                user = context.EntityDb.DoCommand<UserMembership>("select * from [" + mappingName + "] where UserId=@UserId and PasswordVerificationToken=@PasswordVerificationToken", DataParameter.GetSql("UserId", UserId, "PasswordVerificationToken", resetToken), System.Data.CommandType.Text);
            }
            if (user == null)
            {
                throw new SecurityException((int)MembershipStatus.InvalidUser,"AuthorizationException, Un Authorized UserMembership");
            }

            return user;
        }


        //public static UserMembership View(int UserId)
        //{
        //    return Authorizer.GetUserMembership(UserId);
        //}

        public UserResult Update(UserMembership newItem)
        {
            try
            {
                var res = Authorizer.Instance.EntityDb.Context().EntityUpdate<UserMembership>(this, newItem);
                if (EntityCommandResult.GetAffectedRecords(res) >= 0)
                    return UserResult.Get(AuthState.Succeeded);
                return UserResult.Get(AuthState.Failed);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate"))
                    return UserResult.Get(AuthState.NonConfirmed);
                return UserResult.IsUpdated(-1);
            }
        }

        public UserResult Insert()
        {
            try
            {
                var res = Authorizer.Instance.EntityDb.Context().EntityInsert<UserMembership>(this);
                if (EntityCommandResult.GetAffectedRecords(res) >= 0)
                    return UserResult.Get(AuthState.Succeeded);
                return UserResult.Get(AuthState.Failed);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate"))
                    return UserResult.Get(AuthState.NonConfirmed);
                return UserResult.IsUpdated(-1);
            }
        }
        public UserResult Delete()
        {
            try
            {
                int res = Authorizer.Instance.EntityDb.Context().EntityDelete<UserMembership>(this);
                if (res >= 0)
                    return UserResult.Get(AuthState.Succeeded);
                return UserResult.Get(AuthState.Failed);
            }
            catch (Exception ex)
            {
                return UserResult.IsUpdated(-1);
            }
        }

        public static string CreatePassword(UserProfile user, bool sendMail, string resetToken, int expirationInMinute = 1440)
        {
            string newpassword = Authorizer.GenerateRandomPassword(6);
            UserMembership member = new UserMembership()
            {
                UserId = user.UserId,
                Password = Encryption.HashPassword(newpassword),
                PasswordChangedDate = DateTime.Now,
                PasswordVerificationToken = resetToken,
                PasswordVerificationTokenExpirationDate = DateTime.Now.AddMinutes(expirationInMinute)
            };
            UserMessage message = UserMessage.Get("CreatePassword");
            var res = member.Insert();
            if (res.Status <= 0)
            {
                throw new Exception(message.GetErrorMessage("Internal error, could not create password"));
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

        public static string CreatePassword(int UserId, bool sendMail, string resetToken, int expirationInMinute = 1440)
        {
            UserProfile user = UserProfile.Get(UserId);
            return CreatePassword(user, sendMail, resetToken, expirationInMinute);
        }

        public static string ForgotPassword(UserProfile user, string resetLink, string resetToken, int expirationInMinute = 1440)
        {
            //var resetLink = "<a href='" + Url.Action("ResetPassword", "Home", new { un = UserName, rt = token }, "http") + "'>Reset Password</a>";
            //string subject = "Password Reset Token";
            //string body = "<b>Please find the Password Reset Token</b><br/>" + resetLink; 

            if (user == null)
            {
                throw new Exception("Invalid User Profile");
            }
            UserMembership member = UserMembership.Get(user.UserId, resetToken);
            string username = user.UserName;
            UserMembership newItem = DataProperties.Copy<UserMembership>(member);
            newItem.PasswordVerificationToken = resetToken;
            newItem.PasswordVerificationTokenExpirationDate = DateTime.Now.AddMinutes(expirationInMinute);
            UserMessage message = UserMessage.Get("ForgotPassword");
            EmailProvider provider = EmailProvider.Get();

            var res = member.Update(newItem);
            if (res.Status <= 0)
            {
                throw new Exception(message.GetErrorMessage("Internal error, could not reset password"));
            }
            var state = provider.SendEmail(user.Email, message.Subject, message.GetMailMessage("username:" + username, "resetlink:" + resetLink), true);

            return message.GetNotifyMessage(state.Message);
        }

        public static MembershipStatus ForgotPassword(string email, string resetLink, string resetToken, int expirationInMinute = 1440)
        {
            //var resetLink = "<a href='" + Url.Action("ResetPassword", "Home", new { un = UserName, rt = token }, "http") + "'>Reset Password</a>";
            //string subject = "Password Reset Token";
            //string body = "<b>Please find the Password Reset Token</b><br/>" + resetLink; 
            var user= UserProfile.GetByEmail(email);
            if(user==null || user.IsAuthenticated==false)
            {
                //throw new SecurityException((int)MembershipStatus.UserNameOrEmailNotExists, "The email provided is invalid. Please check the value and try again.");
                return MembershipStatus.UserNameOrEmailNotExists;
            }
            if (string.IsNullOrEmpty(resetToken))
            {
                //throw new SecurityException((int)MembershipStatus.InvalidTokenFormt, "Authorizer_Context error,null resetToken memebership");
                return MembershipStatus.InvalidTokenFormt;
            }

            //UserMembership member = UserMembership.Get(user.UserId, resetToken);

            UserMembership member = null;
            string mappingName = EntityMappingAttribute.Mapping<UserMembership>();
            using (Authorizer context = Authorizer.Instance)
            {
                member = context.EntityDb.DoCommand<UserMembership>("select * from [" + mappingName + "] where UserId=@UserId and PasswordVerificationToken=@PasswordVerificationToken", DataParameter.GetSql("UserId", user.UserId, "PasswordVerificationToken", resetToken), System.Data.CommandType.Text);
            }
            if (member == null)
            {
                //throw new SecurityException((int)MembershipStatus.InvalidUser, "AuthorizationException, Un Authorized UserMembership");
                return MembershipStatus.MembershipNotExists;
            }

            string username = user.UserName;
            UserMembership newItem = DataProperties.Copy<UserMembership>(member);
            newItem.PasswordVerificationToken = resetToken;
            newItem.PasswordVerificationTokenExpirationDate = DateTime.Now.AddMinutes(expirationInMinute);
            UserMessage message = UserMessage.Get("ForgotPassword");
            EmailProvider provider = EmailProvider.Get();

            var res = member.Update(newItem);
            if (res.Status <= 0)
            {
                //throw new SecurityException((int)MembershipStatus.CouldNotResetPassword, message.GetErrorMessage("Internal error, could not reset password"));
                return MembershipStatus.CouldNotResetPassword;
            }
            var state = provider.SendEmail(user.Email, message.Subject, message.GetMailMessage("username:" + username, "resetlink:" + resetLink), true);
            
            //return message.GetNotifyMessage(state.Message);

            if(state.StatusCode== System.Net.Mail.SmtpStatusCode.Ok)
                return MembershipStatus.UserPasswordWasReset;
            else
                return MembershipStatus.CouldNotResetPassword;

        }

        //public static int ForgotPassword(string email)
        //{
        //    using (Authorizer context = Authorizer.Instance)
        //    {
        //        var db = context.EntityDb.Context();

        //        return db.ExecuteReturnValue("sp_Ad_UserForgotPass", -1,"Email", email);
        //    }

        //}

        //public static int ForgotPassword(string email, string Token, string ConfirmUrl,string AppUrl, string MailSenderFrom, string Subject, string profile_name)
        //{
        //    using (Authorizer context = Authorizer.Instance)
        //    {
        //        var db = context.EntityDb.Context();

        //        return db.ExecuteReturnValue("sp_Ad_UserForgotPass", -1, "Email", email, "Token", Token, "ConfirmUrl", ConfirmUrl, "AppUrl", AppUrl, "MailSenderFrom", MailSenderFrom, "Subject", Subject, "profile_name", profile_name);
        //    }

        //}

        public static int SendResetToken(string email, int AppId)
        {
            string Token = Guid.NewGuid().ToString().Replace("-", "");
            using (Authorizer context = Authorizer.Instance)
            {
                var db = context.EntityDb.Context();

                return db.ExecuteReturnValue("sp_Ad_UserSendResetToken", -1, "Email", email, "Token", Token, "AppId", AppId);
            }

        }

        [Obsolete("use ResetNewPassword instead")]
        public static int ResetPassword(int accountId, string email, string newpassword, string resetToken)
        {
            //UserProfile user = UserProfile.GetByEmail(email);
            //if (user == null)
            //{
            //    throw new SecurityException((int)MembershipStatus.EmailNotExists, "The email provided is invalid. Please check the value and try again.");
            //}

            using (Authorizer context = Authorizer.Instance)
            {
                var db = context.EntityDb.Context();

                return db.ExecuteReturnValue("sp_Ad_UserResetPass", -1,"Email", email, "AccountId", accountId, "Password", newpassword, "ConfirmationToken", resetToken);
            }
        }
        public static int ResetNewPassword(int accountId, string email, string newpassword, string resetToken)
        {
            //UserProfile user = UserProfile.GetByEmail(email);
            //if (user == null)
            //{
            //    throw new SecurityException((int)MembershipStatus.EmailNotExists, "The email provided is invalid. Please check the value and try again.");
            //}

            using (Authorizer context = Authorizer.Instance)
            {
                var db = context.EntityDb.Context();

                return db.ExecuteReturnValue("sp_Ad_UserResetNewPass", -1, "Email", email, "AccountId", accountId, "Password", newpassword, "ResetToken", resetToken);
            }
        }
        public static int ResetPassword(int UserId, int AssignBy, int AppId)
        {
            using (Authorizer context = Authorizer.Instance)
            {
                var db = context.EntityDb.Context();

                return db.ExecuteReturnValue("sp_Ad_UserResetPassword", -1, "UserId", UserId, "AssignBy", AssignBy, "AppId", AppId);
            }
        }

        public static UserProfile UserVerificationToken(string token)
        {
            using (Authorizer context = Authorizer.Instance)
            {
                var db = context.EntityDb.Context();

                return db.ExecuteSingle<UserProfile>("sp_Ad_UserVerificationToken", "Token", token);
            }

        }

        public static string ResetPassword(string username, string resetToken)
        {
            UserProfile user = UserProfile.GetByUserName(username);
            UserMembership member = UserMembership.Get(user.UserId, resetToken);
            string newpassword = Authorizer.GenerateRandomPassword(6);
            UserMembership newItem = DataProperties.Copy<UserMembership>(member);
            newItem.Password = newpassword;
            newItem.PasswordChangedDate = DateTime.Now;
            //newItem.PasswordVerificationToken = resetToken;
            //newItem.PasswordVerificationTokenExpirationDate = DateTime.Now.AddMinutes(expirationInMinute);
            UserMessage message = UserMessage.Get("ResetPassword");
            EmailProvider provider = EmailProvider.Get();

            var res = member.Update(newItem);
            if (res.Status <= 0)
            {
                throw new SecurityException((int)MembershipStatus.CouldNotResetPassword,message.GetErrorMessage("Internal error, could not reset password"));
            }
            var state = provider.SendEmail(user.Email, message.Subject, message.GetMailMessage("username:" + username), true);

            return message.GetNotifyMessage(state.Message);



            /*
            UsersContext db = new UsersContext();
            //TODO: Check the un and rt matching and then perform following
            //get userid of received username
            var userid = (from i in db.UserProfiles
                          where i.UserName == un
                          select i.UserId).FirstOrDefault();
            //check userid and token matches
            bool any = (from j in db.webpages_Memberships
                        where (j.UserId == userid)
                        && (j.PasswordVerificationToken == rt)
                        //&& (j.PasswordVerificationTokenExpirationDate < DateTime.Now)
                        select j).Any();

            if (any == true)
            {
                //generate random password
                string newpassword = Authorizer.GenerateRandomPassword(6);
                //reset password
                bool response = WebSecurity.ResetPassword(rt, newpassword);
                if (response == true)
                {
                    //get user emailid to send password
                    var emailid = (from i in db.UserProfiles
                                   where i.UserName == un
                                   select i.EmailId).FirstOrDefault();
                    //send email
                    string subject = "New Password";
                    string body = "<b>Please find the New Password</b><br/>" + newpassword; //edit it
                    try
                    {
                        SendEMail(emailid, subject, body);
                        TempData["Message"] = "Mail Sent.";
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Error occured while sending email." + ex.Message;
                    }

                    //display message
                    TempData["Message"] = "Success! Check email we sent. Your New Password Is " + newpassword;
                }
                else
                {
                    TempData["Message"] = "Hey, avoid random request on this page.";
                }
            }
            else
            {
                TempData["Message"] = "Username and token not maching.";
            }

            return View();
             **/
        }

        private string EncodePassword(byte passFormat, string passtext, string passwordSalt)
        {
            if (passFormat.Equals(0)) // passwordFormat="Clear" (0)
                return passtext;
            else
            {
                byte[] bytePASS = Encoding.Unicode.GetBytes(passtext);
                byte[] byteSALT = Convert.FromBase64String(passwordSalt);
                byte[] byteRESULT = new byte[byteSALT.Length + bytePASS.Length + 1];

                System.Buffer.BlockCopy(byteSALT, 0, byteRESULT, 0, byteSALT.Length);
                System.Buffer.BlockCopy(bytePASS, 0, byteRESULT, byteSALT.Length, bytePASS.Length);

                System.Security.Cryptography.HashAlgorithm hashAlgo = new System.Security.Cryptography.SHA256Managed();
                byte[] hash = hashAlgo.ComputeHash(byteRESULT);
                return Convert.ToBase64String(hash);

                //if (passFormat.Equals(1)) // passwordFormat="Hashed" (1)
                //{
                //    HashAlgorithm ha = HashAlgorithm.Create();//Membership.HashAlgorithmType);
                //    return (Convert.ToBase64String(ha.ComputeHash(byteRESULT)));
                //}
                //else // passwordFormat="Encrypted" (2)
                //{
                //    MyCustomMembership myObj = new MyCustomMembership();
                //    return (Convert.ToBase64String(myObj.EncryptPassword(byteRESULT)));
                //}
            }
        }
        //Example usage:

        //    string passSalt = // Either generate a random salt for that user, or retrieve the salt from database if the user is in edit and has a password salt
        //    EncodePassword(/* 0 or 1 or 2 */, passwordText, passSalt);


        private string EncodePassword(byte passFormat, string passwordSalt)
        {
            if (passFormat.Equals(0)) // passwordFormat="Clear" (0)
                return passwordSalt;
            else
            {
                byte[] byteSALT = UnicodeEncoding.Unicode.GetBytes(passwordSalt);
                System.Security.Cryptography.HashAlgorithm hashAlgo = new System.Security.Cryptography.SHA256Managed();
                byte[] hash = hashAlgo.ComputeHash(byteSALT);
                return Convert.ToBase64String(hash);
                //return UnicodeEncoding.Unicode.GetString(hash);
            }
        }


        private string StorePassword(byte passFormat, string passtext, string passwordSalt)
        {

            //To store password hashes:

            //a) Generate a random salt value:

            byte[] salt = new byte[32];
            System.Security.Cryptography.RNGCryptoServiceProvider.Create().GetBytes(salt);

            //b) Append the salt to the password.

            // Convert the plain string pwd into bytes
            byte[] plainTextBytes = UnicodeEncoding.Unicode.GetBytes(passtext);
            // Append salt to pwd before hashing
            byte[] combinedBytes = new byte[plainTextBytes.Length + salt.Length];
            System.Buffer.BlockCopy(plainTextBytes, 0, combinedBytes, 0, plainTextBytes.Length);
            System.Buffer.BlockCopy(salt, 0, combinedBytes, plainTextBytes.Length, salt.Length);
            //c) Hash the combined password & salt:

            // Create hash for the pwd+salt
            System.Security.Cryptography.HashAlgorithm hashAlgo = new System.Security.Cryptography.SHA256Managed();
            byte[] hash = hashAlgo.ComputeHash(combinedBytes);
            //d) Append the salt to the resultant hash.

            // Append the salt to the hash
            byte[] hashPlusSalt = new byte[hash.Length + salt.Length];
            System.Buffer.BlockCopy(hash, 0, hashPlusSalt, 0, hash.Length);
            System.Buffer.BlockCopy(salt, 0, hashPlusSalt, hash.Length, salt.Length);
            //e) Store the result in your user store database.

            return UnicodeEncoding.Unicode.GetString(hashPlusSalt);
        }


        public UserMembership(int UserId, string Password, string PasswordSalt)
        {

            this.UserId = UserId;
            this.Password = Password;
            this.PasswordSalt = EncodePassword(1, PasswordSalt);
            this.CreateDate = DateTime.Now;
        }
        public UserMembership()
        {
            this.CreateDate = DateTime.Now;
        }

        [EntityProperty(EntityPropertyType.Key)]
        public int UserId { get; set; }
        [EntityProperty]
        public DateTime? CreateDate { get; set; }
        [EntityProperty]
        public string ConfirmationToken { get; set; }
        [EntityProperty]
        public bool IsConfirmed { get; set; }
        [EntityProperty]
        public DateTime? LastPasswordFailureDate { get; set; }
        [EntityProperty]
        public int PasswordFailuresSinceLastSuccess { get; set; }
        [EntityProperty]
        public string Password { get; set; }
        [EntityProperty]
        public DateTime? PasswordChangedDate { get; set; }
        [EntityProperty]
        public string PasswordSalt { get; set; }
        [EntityProperty]
        public string PasswordVerificationToken { get; set; }
        [EntityProperty]
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
    }
}
