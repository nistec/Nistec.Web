using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Nistec.Web.Asp
{
    public enum ScField
    {
        UID, AID, UTYPE, ATYPE, PID, ENV, CurrentAccountId, CurrentUserId, CA,Culture
    }
    public interface ISc
    {

        int Get(ScField field);
        int GetCA();
        string GetField(ScField field);
        bool IsAdminOrManager { get; }

        #region view state properties

        string UAID { get; }
        int UserId { get; }
        int AccountId { get; }
        UserType UserType { get; }
        AccountType AccType { get; }
        int ParentId { get; }
        int EnvId { get; }
        UserAuth UserInfo { get; }
        //UserType UserType { get; }
        //AccountType AccType { get; }
        string ENV { get; }
        string AccountName { get; }
        string UserName { get; }
        int ActiveAccId { get; }
        int ActiveUserId { get; }

       
        string RootFolder { get; }
        int ContactCapacity { get; }
        int FilesCapacity { get; }
      
        int CurrentAccountId { get; }
        int CurrentUserId { get; }
        string CultureName { get; set; }
        //int ParentId { get; }
        //int EnvId { get; }
        int AccountManager { get; }
        //int AccountId { get; }
        //int UserId { get; }
        string CacheKey { get; }
        string CacheKeyDependency(string currentKey, bool validateManager=false);
        string AdminKeyDependency(string currentKey);

        #endregion

        void ReSign();
        void RedirectToLogIn();
        void RedirectToIndex();
        void Logoff();
        void ValidateSc();
        void ValidateScManager();
        void SetActiveAccount(UserAuth ui);
        bool SetActiveAccount(int accId, string accName, string rootFolder);
        bool SignIn(UserAuth au, bool rememberMeSet);
        bool SetActiveManagerLogin(int userId);
        bool SetActiveManagerLogout();
        int GetAccParentId();
    }
}
