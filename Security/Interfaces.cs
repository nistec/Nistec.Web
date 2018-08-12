using Nistec.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Security
{
    /// <summary>
    /// Base interface for services that are instantiated per unit of work (i.e. web request).
    /// </summary>
    public interface IDependency
    {
    }
    /// <summary>
    /// Entry-point for configured authorization scheme. Role-based system
    /// provided by default. 
    /// </summary>
    public interface IAuthorization : IDependency
    {
        void CheckAccess(Permission permission, IUser user, IContent content);
        bool TryCheckAccess(Permission permission, IUser user, IContent content);
    }

    public class StubAuth : IAuthorization
    {
        public void CheckAccess(Permission permission, IUser user, IContent content)
        {
        }

        public bool TryCheckAccess(Permission permission, IUser user, IContent content)
        {
            return true;
        }
    }

    public interface IMembershipService : IDependency
    {
        MembershipSettings GetSettings();

        IUser CreateUser(UserParams userParams);
        IUser GetUser(string username);

        IUser ValidateUser(string userNameOrEmail, string password);
        void SetPassword(IUser user, string password);

    }

    public interface IUserService : IDependency
    {
        bool VerifyUserUnicity(string userName, string email);
        bool VerifyUserUnicity(int id, string userName, string email);

        void SendChallengeEmail(IUser user, Func<string, string> createUrl);
        IUser ValidateChallenge(string challengeToken);

        bool SendLostPasswordEmail(string usernameOrEmail, Func<string, string> createUrl);
        IUser ValidateLostPassword(string nonce);

        string CreateNonce(IUser user, TimeSpan delay);
        bool DecryptNonce(string challengeToken, out string username, out DateTime validateByUtc);
    }

    public interface IEventHandler : IDependency
    {
    }

    public interface IUserEventHandler : IEventHandler
    {
        /// <summary>
        /// Called before a User is created
        /// </summary>
        void Creating(UserContext context);

        /// <summary>
        /// Called after a user has been created
        /// </summary>
        void Created(UserContext context);

        /// <summary>
        /// Called after a user has logged in
        /// </summary>
        void LoggedIn(IUser user);

        /// <summary>
        /// Called when a user explicitly logs out (as opposed to one whose session cookie simply expires)
        /// </summary>
        void LoggedOut(IUser user);

        /// <summary>
        /// Called when access is denied to a user
        /// </summary>
        void AccessDenied(IUser user);

        /// <summary>
        /// Called after a user has changed password
        /// </summary>
        void ChangedPassword(IUser user);

        /// <summary>
        /// Called after a user has confirmed their email address
        /// </summary>
        void SentChallengeEmail(IUser user);

        /// <summary>
        /// Called after a user has confirmed their email address
        /// </summary>
        void ConfirmedEmail(IUser user);

        /// <summary>
        /// Called after a user has been approved
        /// </summary>
        void Approved(IUser user);
    }

    public interface IContent
    {
        //ContentItem ContentItem { get; }

        /// <summary>
        /// The ContentItem's identifier.
        /// </summary>
        int Id { get; }
    }

    // Summary:
    //     Represents an HTML-encoded string that should not be encoded again.
    public interface IHtmlString
    {
        // Summary:
        //     Returns an HTML-encoded string.
        //
        // Returns:
        //     An HTML-encoded string.
        string ToHtmlString();
    }

    public interface IAuthentication : IDependency
    {
        void SignIn(SignedUser user, bool createPersistentCookie);
        void SignOut();
        void SetAuthenticatedUserForRequest(SignedUser user);
        SignedUser GetAuthenticatedUser();
    }

    

    ///// Interface provided by the "User" model. 
    ///// </summary>
    //public interface IUser //: IContent
    //{
    //    bool IsAuthenticated { get; }

    //    //string Email { get; }

    //    int UserId { get; }
    //    string UserName { get; }
        
    //    int AccountId { get; }
    //    int UserRole { get; }
    //    //int AccType { get; }
    //    //int ApplicationId{ get; }

    //    string UserData();
    //}

   
}
