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

    public class UserAccount : IEntityItem
    {
        public const string MappingName = "web_UserAccount";

        public static IEnumerable<UserAccount> View()
        {
            return Authorizer.Instance.EntityDb.Context().EntityItemList<UserAccount>(MappingName, null);
        }

        public static UserAccount Get(int AccountId)
        {
            return Authorizer.Instance.EntityDb.Context().EntityItemGet<UserAccount>(MappingName, "AccountId", AccountId);
        }

        [EntityProperty(EntityPropertyType.Key)]
        public int AccountId { get; set; }
        [EntityProperty]
        public int ParentId { get; set; }
        [EntityProperty]
        public string AccountName { get; set; }
        [EntityProperty]
        public string ContactName { get; set; }
        [EntityProperty]
        public string Address { get; set; }
        [EntityProperty]
        public string City { get; set; }
        [EntityProperty]
        public string ZipCode { get; set; }
        [EntityProperty]
        public string Phone { get; set; }
        [EntityProperty]
        public string Fax { get; set; }
        [EntityProperty]
        public string Mobile { get; set; }
        [EntityProperty]
        public string Email { get; set; }
        [EntityProperty]
        public string IdNumber { get; set; }
        [EntityProperty]
        public int Country { get; set; }
        [EntityProperty]
        public int OwnerId { get; set; }
        [EntityProperty]
        public int AccType { get; set; }
        [EntityProperty]
        public int BusinessGroup { get; set; }
        [EntityProperty]
        public string Details { get; set; }
        [EntityProperty]
        public bool IsActive { get; set; }
        [EntityProperty]
        public DateTime CreationDate { get; set; }
        [EntityProperty]
        public DateTime LastUpdate { get; set; }
    }

}
