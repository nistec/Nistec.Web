using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Nistec.Web.Asp
{
    [Serializable]
    public class SessionException : Exception
    {
        //public const string SessionKeyPrevent = "לא ניתן להמשיך בפעולה זו , אנא פנה לתמיכה";
        public const string SessionKeyPrevent = "You can not proceed with this action, please contact support";

        public SessionException(string msg)
            : base(msg)
        {
        }

        public SessionException(string msg, EntryPointNotFoundException innerExeption)
            : base(msg, innerExeption)
        {
        }

        public SessionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
