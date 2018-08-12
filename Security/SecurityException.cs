using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Security
{
    public class SecurityException : Exception
    {
        public int ErrorCode { get; private set; }

        public SecurityException(AuthState state)
            : base(state.ToString())
        {
            ErrorCode = (int)state;
        }
        public SecurityException(AuthState errorCode, string message)
            : base(message)
        {
            ErrorCode = (int)errorCode;
        }
        public SecurityException(int errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public SecurityException(int errorCode, string message,Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }
    }
}
