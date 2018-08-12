using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.IO;
using System.Collections.Specialized;
using System.Web;

namespace Nistec.Web.Asp
{

   
    //[Designer(typeof(CultureControl)), ToolboxData("<{0}:CultureControl runat=\"server\" Width=\"250px\" Height=\"150px\"></{0}:CultureControl>")]
    public class CultureControl : System.Web.UI.WebControls.WebControl, IPostBackEventHandler, IPostBackDataHandler
    {

        //private static string a_31 = "Cannot find Nistec.Web resources folder .\n\rCopy or Deploy this folder with all the files it contains to http://{your site}/ on your web site. (You require only single copy of this folder, all other application can refer to this.)\n\rIf you want to deploy the Nistec.Web folder to a different location, you will have to make sure that you set the Editor.ClientDirectory property correctly.\n\r";
        private static string a_31 = "Cannot find App_Web resources folder [App_Web].\n\rCopy or Deploy this folder with all the files it contains to http://{your site}/ on your web site. (You require only single copy of this folder, all other application can refer to this.)\n\rIf you want to deploy the App_Web folder to a different location, you will have to make sure that you set the Editor.ClientDirectory property correctly.\n\rForexample if you want to locate the App_Web folder to http://{your site}/demo then the Editor.ClientDirectory property should be \"~/demo/App_Web/\"\n\r ";

        public event PostBackHandler PostBack;


        CultureUtil _CultureUtil;
        //private string _eventArgs;
        //private string _Data;
        //private bool _isPostDta;
        private bool _isRaisePostBack=false;
        private bool _isScriptManager = false;
        private StringUtil _StringUtil;

        private StringUtil StringUtil
        {
            get
            {
                if (this._StringUtil == null)
                {
                    this._StringUtil = new StringUtil(HttpContext.Current);
                }
                return this._StringUtil;
            }
        }

        public bool IsActive
        {
            get
            {
              return  this._CultureUtil != null;
            }
        }

        #region properties

        [Description("Include directory."), DefaultValue("~/App_Web/"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.Attribute), Bindable(true), Category("Setup")]
        public string ClientDirectory
        {
            get
            {
                object obj2 = this.ViewState["ClientDirectory"];
                if (obj2 != null)
                {
                    return obj2.ToString();
                }
                return "~/App_Web/";
            }
            set
            {
                this.ViewState["ClientDirectory"] = value;
            }
        }

        [Bindable(true), DefaultValue("en-US"), Category("Setup"), Description("Sets the name of culture")]
        public string CultureName
        {
            get
            {
                if (this.ViewState["CultureName"] == null)
                {
                    return "en-US";
                }
                return (string)this.ViewState["CultureName"];
            }
            set
            {
                this.ViewState["CultureName"] = value;
            }
        }

        [Bindable(true), Category("Setup"), DefaultValue(2), Description("Sets the behavior for determining how the culture is chosen for the WebEditor")]
        public CultureSetting CultureSetting
        {
            get
            {
                if (this.ViewState["CultureSetting"] == null)
                {
                    return CultureSetting.Server;
                }
                return (CultureSetting)this.ViewState["CultureSetting"];
            }
            set
            {
                this.ViewState["CultureSetting"] = value;
            }
        }

        [DefaultValue("Silver"), PersistenceMode(PersistenceMode.Attribute), Category("Display"), Description("Theme applied to the editor style.")]
        public string Theme
        {
            get
            {
                if (this.ViewState["Theme"] == null)
                {
                    return "Silver";
                }
                return this.ViewState["Theme"].ToString();
            }
            set
            {
                this.ViewState["Theme"] = value;
            }
        }

#endregion

         //private bool isPostBack()
        //{
        //    return (!this._isRaisePostBack && this._isPostDta);
        //}

        //public void RaisePostBackEvent(string eventArgument)
        //{
        //    this._isRaisePostBack = true;
        //    if (this.PostBack != null)
        //    {
        //        this.PostBack(this, new PostBackEventArgs(eventArgument));
        //    }
        //}
        public void RenderCulture(EventArgs e)
        {
            OnPreRender(e);
        }
        protected override void OnPreRender(EventArgs e)
        {
            //if (this.isPostBack())
            //{
            //    this.RaisePostBackEvent(this._eventArgs);
            //}
            base.OnPreRender(e);
            this.RegisterAsyncPostBack();
            if (!this._isScriptManager)
            {
                this.Page.RegisterRequiresPostBack(this);
            }
            string path = StringUtil.GetPhysicalPath(this.ClientDirectory, false);
            if (!Directory.Exists(path))
            {
                throw new Exception(a_31);
            }
                       
            if (this._CultureUtil == null)
            {
                this._CultureUtil = new CultureUtil(this.Context, path, this.CultureName, this.CultureSetting,false);
            }

            string script = string.Format("\r\n<script type=\"text/javascript\" src=\"{0}\"></script>\r\n", this.GetClientDirectory() + "scripts/Nistec.Web.js");

            this.RegisterClientScriptBlock("NetcellScript", script, false);

            //if (this._ToolBarList.Count == 0)
            //{
            //    this.LoadToolbarsFromXml();
            //}
/*
            if (!this.IsClientScriptBlockRegistered("McScript"))
            {
                StringBuilder builder = new StringBuilder();
                bool readOnly = this.ReadOnly;
                string str3 = "1";
                if (!readOnly && this._BrowserCap.IsSupportJS)
                {
                    builder.Append(StringUtil.startScriptTag + string.Format("var cpath ='{0}';\r\n", this.GetClientDirectory()) + StringUtil.endScriptTag);
                    if (this._BrowserCap.IsIE)
                    {
                        str3 = "1";
                    }
                    else
                    {
                        str3 = "2";
                    }
                }
                else
                {
                    str3 = "3";
                }
                builder.Append(FormatLoader(this.GetClientDirectory(), str3, MccId()));//string.Empty));
                // builder.Append(string.Format("\r\n<script type=\"text/javascript\" src=\"{0}\"></script>\r\n", this.GetClientDirectory() + "scripts/jquery-1.3.2.min.js")); 
                builder.Append(string.Format("\r\n<script type=\"text/javascript\" src=\"{0}\"></script>\r\n", this.GetClientDirectory() + "scripts/editorSettings.js"));
                builder.Append(string.Format("\r\n<script type=\"text/javascript\" src=\"{0}\"></script>\r\n", this.GetClientDirectory() + "scripts/Html2Xhtml.js"));

#if(DEBUGGING)
                builder.Append(string.Format("\r\n<script type=\"text/javascript\" src=\"{0}\"></script>\r\n", this.GetClientDirectory() + "scripts/editor.js"));
                builder.Append(string.Format("\r\n<script type=\"text/javascript\" src=\"{0}\"></script>\r\n", this.GetClientDirectory() + "scripts/editor_ie.js"));
                //builder.Append(string.Format("\r\n<script type=\"text/javascript\" src=\"{0}\"></script>\r\n", this.GetClientDirectory() + "scripts/editor_moz.js"));
#endif
                this.RegisterClientScriptBlock("McScript", builder.ToString(), false);
            }
*/
            if (!this.IsClientScriptBlockRegistered(this._CultureUtil.CacheKey))
            {
                string cultureScript = this._CultureUtil.RenderDictionary();
                if ((cultureScript != null) && (cultureScript.Length > 0))
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(),this._CultureUtil._key, str4, true);
                    this.RegisterClientScriptBlock(this._CultureUtil.CacheKey, cultureScript, true);
                }
            }

            string cssFilename = this.GetThemesPath() + "netcell.css";
            string cssKey = "NetcellStyle" + cssFilename.GetHashCode();
            if (!this.IsClientScriptBlockRegistered(cssKey))
            {   //a395
                this.RegisterClientScriptBlock(cssKey, "netcell_registerCss(document,\"" + cssFilename + "\");", true);
            }

            //string startupKey = "NetcellStartup" +this.ClientID.GetHashCode();
            ////startupKey+=startupKey.GetHashCode();
            //if (!this.IsStartupScriptRegistered(startupKey))
            //{
            //    string init = string.Format("netcell_init('{0}');",CultureName);
            //    this.RegisterScript(startupKey, init, true);
            //}
 
        }

        public string GetResourceString(string key, string defaultValue)
        {
            try
            {
                if (_CultureUtil == null)
                {
                    RenderCulture(EventArgs.Empty);
                }
                if (_CultureUtil != null)
                {
                    return _CultureUtil.LangValue(key, defaultValue);

                }
                NameValueCollection col = GetResource(CultureName);
                if (col == null)
                    return defaultValue;
                return col.Get(key);

            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public NameValueCollection GetResource()
        {
            try
            {
                if (_CultureUtil == null)
                {
                    RenderCulture(EventArgs.Empty);
                }
                if (_CultureUtil != null)
                {
                    return _CultureUtil.CollectionArray();
                }

                //string culture= CookieItem.Get(this.Page).Culture;
                string key = CultureUtil.GetCultureKey(CultureName);
                return (NameValueCollection)this.Page.Cache.Get(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public NameValueCollection GetResource(string culture)
        {
            try
            {
                if (_CultureUtil == null)
                {
                    RenderCulture(EventArgs.Empty);
                }
                if (_CultureUtil != null)
                {
                    return _CultureUtil.CollectionArray();
                }

                //string culture= CookieItem.Get(this.Page).Culture;
                string key = CultureUtil.GetCultureKey(culture);
                return (NameValueCollection)this.Page.Cache.Get(key);
            }
            catch (Exception)
            {
                return null;
            }
        }
      
        private bool IsClientScriptBlockRegistered(string key)
        {
            if (this._isScriptManager)
            {
                return false;
            }
            return this.Page.ClientScript.IsClientScriptBlockRegistered(key);
        }

        private string GetClientDirectory()
        {
            return StringUtil.GetVirtualPath(this.ClientDirectory, true);
        }

        private void RegisterClientScriptBlock(string key, string script, bool adddScriptTag)//a_70
        {
            if (this._isScriptManager)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), key, script, adddScriptTag);
            }
            else
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), key, script, adddScriptTag);
            }
        }

        private string GetThemesPath()
        {
            return (this.GetClientDirectory() + "Themes/" + this.Theme + "/");
        }

        private bool IsStartupScriptRegistered(string key)
        {
            if (this._isScriptManager)
            {
                return false;
            }
            return this.Page.ClientScript.IsStartupScriptRegistered(key);
        }

        private void RegisterScript(string key, string script, bool addTag)//a_75
        {
            if (this._isScriptManager)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), key, script, addTag);
            }
            else
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), key, script, addTag);
            }
        }
        #region post back
        
        public bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if ((postDataKey == this.UniqueID) && ((this.Enabled && this.Visible) && (postCollection != null)))
            {
                //this._isPostDta = true;
                //this._eventArgs = postCollection["__EVENTARGUMENT"];
                ////string str = postCollection[this.GetEditorData()];
                ////str = StringUtil.ToHexRvs(str);
                //string text = this._Data;
                //if (!((text != null) && StringUtil.IsEqual(text, str, false)))
                //{
                //    //this.Content = str;
                //    return true;
                //}

                return true;
            }
            return false;
        }
        
        public void RegisterAsyncPostBack()//a_62
        {
            try
            {
                ScriptManager current = ScriptManager.GetCurrent(this.Page);
                if (current != null)
                {
                    current.RegisterAsyncPostBackControl(this);
                    this._isScriptManager = true;
                }
                else
                {
                    this._isScriptManager = false;
                }
            }
            catch
            {
                this._isScriptManager = false;
            }
        }


        public bool IsRaisePostBack()
        {
            return (!this._isRaisePostBack /*&& this._isPostDta*/);
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            this._isRaisePostBack = true;
            if (this.PostBack != null)
            {
                this.PostBack(this, new PostBackEventArgs(eventArgument));
            }
        }

        public void RaisePostDataChangedEvent()
        {
        }
        #endregion
    }
}
