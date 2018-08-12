using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Asp
{
    public enum UserType
    {
        Guest = 0,
        User = 1,
        Manager = 2,
        Owner = 3,
        Admin = 9,
    }

    public enum UserLevel
    {
        DenyAll = 0,
        ReadOnly = 1,
        FullControl = 2,
    }

  
    public enum AccountType
    {
        Customer = 0,
        Parent = 1,
        Child = 2,
        Owner = 3,
        Evaluate = 4,
        Demo = 5,
        Admin = 9
    }
}
