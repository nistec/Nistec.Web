using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using Nistec;

namespace Nistec.Web.Asp
{
    public class ScContext
    {
        public readonly string UAID;
        public readonly int UserId;
        public readonly int AccountId;
        public readonly UserType UserType;
        public readonly AccountType AccType;
        public readonly int ParentId;
        public readonly int EnvId;
        public readonly string ENV;

        public readonly bool IsEmpty;
        //public UserType UserType
        //{
        //    get { return (UserType)UTYPE; }
        //}


        public ScContext(string value, bool encrypted=true)
        {
            UAID = encrypted ? Nistec.Runtime.RequestQuery.DecryptEx32(value) : value;
            //int uid = 0;
            //int aid = 0;
            //int userType = 0;
            //int accType = 0;
            //int iparent = 0;

            //Nistec.Generic.GenericArgs.SplitArgs<int, int, int, int, int>(UAID, '-', ref uid, ref aid, ref userType, ref accType, ref iparent);
            //UID = uid;
            //AID = aid;
            //UTYPE = userType;
            //ATYPE = accType;
            //PID = iparent;

            string[] args = value.Split('-');
            if (args.Length > 6)
            {

                UserId = Types.ToInt(args[0]);
                AccountId = Types.ToInt(args[1]);
                UserType = (UserType)Types.ToInt(args[2]);
                AccType = (AccountType)Types.ToInt(args[3]);
                ParentId = Types.ToInt(args[4]);
                EnvId = Types.ToInt(args[5]);
                ENV = args[6];
                IsEmpty = false;
            }
            else
            {
                IsEmpty = true;
            }
        }

      
        #region static

        public static ScContext Parse(string sessionId)
        {
            return new ScContext(sessionId, false);
        }


        //public static string GetAutoAccListUrl(Page p)
        //{
        //    return GetAutoListUrl(p, "~/View/Ajax/AutoAccList.ashx");
        //}

        public static string GetAutoListUrl(Page p, string url)
        {
            string ick = Nistec.Runtime.RequestQuery.EncryptEx32(p.GetSc().UAID);
            return p.ResolveClientUrl(url) + "?ick=" + ick;
        }

        #endregion
    }


}
