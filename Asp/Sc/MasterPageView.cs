using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;

namespace Nistec.Web.Asp
{
    public interface IScPage
    {
        ISc Sc { get; }
    }

    public abstract class ScPage : System.Web.UI.Page, IScPage
    {
        public abstract ISc Sc { get; }

        /// <summary>
        /// Set response NoCache
        /// </summary>
        protected void SetResponseNoCache()
        {
            //if (response == null)
            //    throw new ArgumentNullException("response");

            Response.Cache.SetAllowResponseInBrowserHistory(false);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(60));
            Response.Cache.SetValidUntilExpires(true);

            //=====================
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //Response.CacheControl = "private";
            //Response.Expires = 0;
            //Response.AddHeader("pragma", "no-cache");

            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Expires", DateTime.Now.AddDays(-1).ToShortDateString());

        }
    }


    public abstract class ScMasterPage : System.Web.UI.MasterPage, IScPage
    {

        public abstract ISc Sc { get; }

    }

 
}
