using Nistec.Data;
using Nistec.Data.Entities;
using Nistec.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Nistec.Web.Cms
{

    public class CmsSiteTree : IEntityItem
    {
        public const string MappingName = "webvw_Cms_Site_Tree";

        #region static get

        public static IList<CmsSiteTree> Get(int SiteId)
        {
            if (SiteId <= 0)
            {
                throw new Exception("CmsSiteTree error,null SiteId");
            }
            IList<CmsSiteTree> tree = null;
            using (CmsSiteContext context = new CmsSiteContext())
            {
                tree = context.EntityDb.Query<CmsSiteTree>("select * from [" + MappingName + "] where SiteId=@SiteId", DataParameter.GetSql("SiteId", SiteId), System.Data.CommandType.Text);
            }
            if (tree == null)
            {
                throw new Exception("CmsSiteTree, Site tree not found");
            }

            return tree;
        }



        #endregion

        [EntityProperty]
        public int SiteId { get; set; }
        [EntityProperty]
        public string PageName { get; set; }
        [EntityProperty]
        public int PageCategory { get; set; }

        [EntityProperty]
        public string PageTitle { get; set; }
        [EntityProperty]
        public int PageState { get; set; }

        [EntityProperty(EntityPropertyType.Identity)]
        public int SectionId { get; set; }
        [EntityProperty]
        public string SectionName { get; set; }

        [EntityProperty]
        public int PageId { get; set; }

        [EntityProperty]
        public string SectionContent { get; set; }


    }
}
