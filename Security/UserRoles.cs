using Nistec.Data;
using Nistec.Data.Entities;
using Nistec.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Nistec.Web.Security
{

    public enum UserRole
    {
        Unknown = 0,
        //client users
        User = 1,
        Super = 2,
        Manager = 5,

        //Managements users
        System=6,   //sub excet: no credit, no user creation
        SubAdmin = 7, //admin except: limited credit, limited user creation
        Admin = 9 // can do anything
    }

    public class UserRoles : IEntityItem
    {
        public const string MappingName = "web_UserRoles";

        public static IEnumerable<UserRoles> View()
        {
            return Authorizer.Instance.EntityDb.Context().EntityItemList<UserRoles>(MappingName, null);
        }

        [EntityProperty(EntityPropertyType.Key)]
        public int RoleId { get; set; }
        [EntityProperty]
        public string RoleName { get; set; }
    }

}
