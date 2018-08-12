using Nistec.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Security
{
    public class UserContext
    {
        public IUser User { get; set; }
        public bool Cancel { get; set; }
        public UserParams UserParameters { get; set; }
    }
}
