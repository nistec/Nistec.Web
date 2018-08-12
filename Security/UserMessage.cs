using Nistec.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nistec.Generic;

namespace Nistec.Web.Security
{

    [Entity("UserMessage", EntityMode.Config)]
    public class UserMessageContext : EntityContext<UserMessage>
    {
        #region ctor


        public UserMessageContext()
        {

        }

        public UserMessageContext(string MessageType)
            : base(MessageType)
        {

        }

        #endregion
    }

    public class UserMessage
    {
        //public const string MappingName = "UserMessage";

        public static UserMessage Get(string MessageType)
        {
            if (MessageType == null)
            {
                throw new Exception("UserMessage error,null MessageType");
            }
            UserMessage entity = null;
            using (UserMessageContext context = new UserMessageContext(MessageType))
            {
                entity = context.Entity;
            }
            if (entity == null)
            {
                throw new Exception("UserMessage Exception, MessageType not found");
            }

            return entity;
        }

        public static string GetPersonalMessage(string Body, string personalFields, string[] personal)
        {
            if (string.IsNullOrEmpty(personalFields) || personal==null || personal.Length==0)
                return Body;
            string body = Body;
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (string parg in personal)
            {
                string[] pargs = parg.SplitTrim(':');
                if (pargs != null && pargs.Length > 1)
                {
                    dic[pargs[0]] = pargs[1];
                }
            }

            string[] args = personalFields.SplitTrim(',', ';');
            foreach (string arg in args)
            {
                string replacement = " ";
                string key = arg.Replace("[", "").Replace("]", "");
                dic.TryGetValue(key, out replacement);
                body = body.Replace(arg, replacement);
            }

            return body;
        }


        public string GetMailMessage(params string[] personal)
        {
            return GetPersonalMessage(Body, PersonalFields, personal);
        }

        public string GetShortMessage(params string[] personal)
        {
            return GetPersonalMessage(ShortMessage, PersonalFields, personal);
        }

        public string GetErrorMessage(string reason)
        {
            return GetPersonalMessage(ErrorMessage, "[Reason]", new string[]{string.Format("Reason:{1}", reason)});
        }

        public string GetNotifyMessage(string defaultMessage)
        {
            return Types.NZorEmpty(NotifyMessage, defaultMessage);
        }

        [EntityProperty(EntityPropertyType.Key)]
        public string MessageType { get; set; }
        [EntityProperty]
        public string Subject { get; set; }
        [EntityProperty]
        public string Body { get; set; }
        [EntityProperty]
        public string ShortMessage { get; set; }
        [EntityProperty]
        public string PersonalFields { get; set; }
        [EntityProperty]
        public string NotifyMessage { get; set; }
        [EntityProperty]
        public string ErrorMessage { get; set; }
    }
}
