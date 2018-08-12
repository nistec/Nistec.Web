using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.IO;

namespace Nistec.Web.Asp
{
    //[Designer(typeof(ToolControlDesginer)), ToolboxData("<{0}:Editor runat=\"server\" Width=\"250px\" Height=\"150px\"></{0}:Editor>")]
 
    public class CulturePage:System.Web.UI.Page
    {

        private static string a_31 = "Cannot find Nistec.Web resources folder .\n\rCopy or Deploy this folder with all the files it contains to http://{your site}/ on your web site. (You require only single copy of this folder, all other application can refer to this.)\n\rIf you want to deploy the Nistec.Web folder to a different location, you will have to make sure that you set the Editor.ClientDirectory property correctly.\n\r";
        //private static string a_31 = "Cannot find WebEditor.NET resources folder [Editor_Client].\n\rCopy or Deploy this folder with all the files it contains to http://{your site}/ on your web site. (You require only single copy of this folder, all other application can refer to this.)\n\rIf you want to deploy the Editor_Client folder to a different location, you will have to make sure that you set the Editor.ClientDirectory property correctly.\n\rForexample if you want to locate the Editor_Client folder to http://{your site}/demo then the Editor.ClientDirectory property should be \"~/demo/Editor_Client/\"\n\r ";


        CultureUtil _CultureUtil;
        //private bool _isRaisePostBack;
        private bool _isScriptManager = false;
        //private StringUtil _StringUtil;

        [Description("Include directory."), DefaultValue("~/Editor_Client/"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.Attribute), Bindable(true), Category("Setup")]
        public string ClientDirectory
        {
            get
            {
                object obj2 = this.ViewState["ClientDirectory"];
                if (obj2 != null)
                {
                    return obj2.ToString();
                }
                return "~/Editor_Client/";
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
            if (!this.IsClientScriptBlockRegisteredInternal(this._CultureUtil.CacheKey))
            {
                string str4 = this._CultureUtil.RenderDictionary();
                if ((str4 != null) && (str4.Length > 0))
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(),this._CultureUtil._key, str4, true);
                    this.RegisterClientScriptBlock(this._CultureUtil.CacheKey, str4, true);
                }
            }
/*
            string str5 = this.GetThemesPath() + "editor.css";
            string str2 = "McStyle" + str5.GetHashCode();
            if (!this.IsClientScriptBlockRegistered(str2))
            {   //a395
                this.RegisterClientScriptBlock(str2, "registerCss(document,\"" + str5 + "\");", true);
            }
            str2 = "McStartup" + this.MccId().GetHashCode();
            if (!this.IsStartupScriptRegistered(str2))
            {
                this.RegisterScript(str2, this.SetEditor(), true);
            }
 */ 
        }


        private bool IsClientScriptBlockRegisteredInternal(string key)
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

        protected new bool IsStartupScriptRegistered(string key)
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

    }
}
