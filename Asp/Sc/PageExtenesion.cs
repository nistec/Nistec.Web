using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;

namespace Nistec.Web.Asp
{
   
    public static class PageExtenesion 
    {
        public static ISc GetSc(this Page p)
        {
            if (p.Master != null && p.Master is IScPage)
            {
                return ((IScPage)p.Master).Sc;
            }

            if (p is IScPage)
            {
                return ((IScPage)p).Sc;
            }
            return NcSession(p);

            //return ((IScPage)p).Sc;
        }

        public static int GetSc(this Page p, ScField field)
        {
            if (p.Master != null && p.Master is IScPage)
            {
                return ((IScPage)p.Master).Sc.Get(field);
            }

            if (p is IScPage)
            {
                return ((IScPage)p).Sc.Get(field);
            }
            return NcSession(p).Get(field);
        }

        public static string GetScField(this Page p, ScField field)
        {
            if (p.Master != null && p.Master is IScPage)
            {
                return ((IScPage)p.Master).Sc.GetField(field);
            }

            if (p is IScPage)
            {
                return ((IScPage)p).Sc.GetField(field);
            }
            return NcSession(p).GetField(field);
        }

        public static ISc NcSession(this Page p, bool chekMaster=true)
        {
            Control c = null;
            if (!chekMaster || p.Master == null)
                c = p.FindControl("ncSession");
            else
                c = p.Master.FindControl("ncSession");

            if (c == null)
            {
                throw new Exception("The current session was lost.");
            }
            return (ISc)c;
        }

        public static CultureControl GetCulture(this Page p, bool chekMaster = true)
        {
            Control c = null;
            if (!chekMaster || p.Master == null)
                c = p.FindControl("ncCulture");
            else
                c = p.Master.FindControl("ncCulture");

            if (c == null)
            {
                return null;
            }

            return (CultureControl)c;
        }

        public static string GetCultureString(this Page p, string key, string defaultValue, bool chekMaster = true)
        {
            CultureControl c = GetCulture(p, chekMaster);
            if (c == null)
                return defaultValue;
 
            return c.GetResourceString(key, defaultValue);
        }

   }

 
    public static class MasterPageExtenesion //: MasterPage
    {
        public static ISc GetSc(this MasterPage p)
        {
            if (p is IScPage)
                return ((IScPage)p).Sc;
            return NcSession(p);
        }

        public static int GetSc(this MasterPage p, ScField field)
        {
            //ScControl ck = (ScControl)view.FindControl("ucSessionMonitor");
            //return ck.Get(field);
            if (p is IScPage)
                return ((IScPage)p).Sc.Get(field);
            return NcSession(p).Get(field);
        }

        public static string GetScField(this MasterPage p, ScField field)
        {
            if (p is IScPage)
            {
                return ((IScPage)p).Sc.GetField(field);
            }
            return NcSession(p).GetField(field);
        }
        public static ISc NcSession(this MasterPage p)
        {
            Control c = p.FindControl("ncSession");
            if (c == null)
            {
                throw new Exception("The current session was lost.");
            }
            return (ISc)c;
        }
    }
}
