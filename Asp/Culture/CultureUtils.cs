using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Web.UI;

namespace Nistec.Web.Asp
{
    public enum CultureSetting
    {
        Custom,
        ClientUI,
        Server,
        ServerUI,
        Cookie
    }
        //a40
        public class CultureUtil
        {

 

            public static Cultures GetCultue(string cultureName)
            {
                return Nistec.Generic.EnumExtension.Parse<Cultures>(cultureName, Cultures.he);
            }

            private NameValueCollection _CollectionArray;
            private HttpContext _HttpContext;
            private string clientPath;
            internal string cultureName;
            private string _key;
            private bool _clearCache = false;

            internal string CacheKey
            {
                get { return _key; }
            }
            internal CultureUtil(HttpContext context, string clientPath, string cultureName, CultureSetting setting, bool clearCache)
            {
                _clearCache = clearCache;
                this.Init(context, clientPath, cultureName, setting);
            }

            internal CultureUtil(HttpContext context, string clientPath, string cultureName, string setting)
            {
                CultureSetting server;
                try
                {
                    server = (CultureSetting)Convert.ToInt32(setting);
                }
                catch
                {
                    server = CultureSetting.Server;
                }
                this.Init(context, clientPath, cultureName, server);
            }
            //a43
            internal string LangValue(string key, string defaultValue)
            {
                NameValueCollection valuesArray = this.CollectionArray();
                string str = (valuesArray != null) ? valuesArray[key] : null;
                if ((str != null) && (str.Length != 0))
                {
                    return str;
                }
                return defaultValue;
            }
            //a44
            public string SpellText(string key, string defaultValue)
            {
                NameValueCollection valuesArray = this.CollectionArray();
                string str = (valuesArray != null) ? valuesArray[key] : null;
                if ((str != null) && (str.Length != 0))
                {
                    return str;
                }
                return defaultValue;
            }

            public static string Spell(string culture, string key)
            {
                return Spell(culture, key, key);
            }

            public static string Spell(string culture,string key, string defaultValue)
            {
                try
                {

                    if (culture == null)
                    {
                        return defaultValue;
                    }
                    string cultureKey = GetCultureKey(culture);
                    NameValueCollection col = (NameValueCollection)HttpContext.Current.Cache.Get(cultureKey);

                    if (col == null)
                        return defaultValue;
                    return col.Get(key);

                }
                catch (Exception)
                {
                    return defaultValue;
                }
            }

            public static string[] Spell(string culture, string[] keys)
            {
                try
                {

                    if (culture == null)
                    {
                        return keys;
                    }
                    string cultureKey = GetCultureKey(culture);
                    NameValueCollection col = (NameValueCollection)HttpContext.Current.Cache.Get(cultureKey);

                    if (col == null)
                        return keys;
                    string[] spells=new string[keys.Length];
                    for (int i=0;i<keys.Length;i++)
                    {
                        spells[i] =  col.Get(keys[i]) ?? keys[i];
                    }
                    return spells;
                }
                catch (Exception)
                {
                    return keys;
                }
            }

            //a45
            internal string RenderDictionary()
            {
                //NameValueCollection[] valuesArray = this.CollectionArray();
                NameValueCollection values = this.CollectionArray(); //(valuesArray != null) ? valuesArray[0] : null;
                if ((values == null) || (values.Count <= 0))
                {
                    return null;
                }
                bool flag = false;
                StringBuilder builder = new StringBuilder();
                builder.Append("setNetcellCulture(" + StringUtil.QuatStr(this.cultureName) + ",{");//a704
                int count = values.Count;
                for (int i = 0; i < count; i++)
                {
                    string key = values.GetKey(i);
                    string s = values[i];
                    flag = true;
                    if (i > 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append(StringUtil.QuatStr(key) + ":" + StringUtil.QuatStr(s));
                }
                builder.Append("}); ");
                if (!flag)
                {
                    return null;
                }
                return builder.ToString();
            }

            private void Init(HttpContext context, string clientPath, string cultureName, CultureSetting setting)
            {
                this._HttpContext = context;
                this.clientPath = clientPath;
                this.cultureName = cultureName;
                try
                {
                    switch (setting)
                    {
                        case CultureSetting.Custom:
                            goto Label_008B;

                        case CultureSetting.ClientUI:
                            this.cultureName = context.Request.UserLanguages[0];
                            goto Label_008B;

                        case CultureSetting.Cookie:
                            this.cultureName = CookieItem.Get( context.Request).Culture;
                            goto Label_008B;

                        case CultureSetting.Server:
                            this.cultureName = Thread.CurrentThread.CurrentCulture.Name;
                            goto Label_008B;

                        case CultureSetting.ServerUI:
                            this.cultureName = Thread.CurrentThread.CurrentUICulture.Name;
                            goto Label_008B;
                    }
                }
                catch
                {
                    this.cultureName = Thread.CurrentThread.CurrentCulture.Name;
                }
            Label_008B:
                this.CollectionArray();
            }

            public static string GetResourceString(Page p,string culture,string key, string defaultValue)
            {
                try
                {

                    string culturekey = CultureUtil.GetCultureKey(culture);
                    NameValueCollection col = (NameValueCollection)p.Cache.Get(key);
                    if (col == null)
                        return defaultValue;
                    return col.Get(culturekey);
                   
                }
                catch (Exception)
                {
                    return defaultValue;
                }
            }

            public static void ClearCultureCache(Page p, params string[] cultures)
            {
                foreach (string culture in cultures)
                {
                    string cacheKey = CultureConfig.CultureCacheKey + CultureConfig.SiteName + ":" + culture;
                    p.Cache.Remove(cacheKey);
                }
            }

            public static string GetCultureKey(string culture)
            {
                return CultureConfig.CultureCacheKey + CultureConfig.SiteName + ":" + culture;
            }

            internal NameValueCollection CollectionArray()
            {
                if (this._CollectionArray == null)
                {
                    string str = this.cultureName + ".xml";
                    string filename = this.clientPath + "Languages/";
                    if (!File.Exists(filename + str))
                    {
                        str = "en.xml";
                        if (!File.Exists(filename + str))
                        {
                            return null;
                        }
                    }
                    filename = filename + str;
                    this._key = GetCultureKey(cultureName);// "Nistec:" + cultureName + str.GetHashCode().ToString();
                    if (_clearCache)
                    {
                        this._HttpContext.Cache.Remove(this._key);
                    }
                    this._CollectionArray = (NameValueCollection)this._HttpContext.Cache[this._key];
                    if (this._CollectionArray == null)
                    {
                        this._CollectionArray = null;
                        XmlDocument document = new XmlDocument();
                        document.Load(filename);
                        XmlNodeList childNodes = document.DocumentElement.ChildNodes;
                        NameValueCollection values = null;
                        NameValueCollection c = null;
                        values = new NameValueCollection();
                        foreach (XmlNode node in childNodes)
                        {
                            if (node.NodeType != XmlNodeType.Element)
                            {
                                continue;
                            }
                            XmlElement element = (XmlElement)node;
                            if (((element.Name == "ToolbarInfo") || (element.Name == "DialogInfo")) || (element.Name == "CommonInfo"))
                            {
                                XmlElement element2;
                                string str3;
                                string attribute;
                                XmlNodeList list2 = element.ChildNodes;
                                
                                    c = new NameValueCollection();
                                    
                                    foreach (XmlNode node2 in list2)
                                    {
                                        if (node2.NodeType == XmlNodeType.Element)
                                        {
                                            element2 = (XmlElement)node2;
                                            attribute = element2.GetAttribute("value");
                                            if (!StringUtil.IsNullOrEmptyString(attribute))
                                            {
                                                str3 = element2.GetAttribute("id");
                                                string s = element2.GetAttribute("name");
                                                if (!StringUtil.IsNullOrEmptyString(s))
                                                {
                                                    c[s] = attribute;
                                                }
                                                //if (!StringUtil.IsNullOrEmptyString(str3))
                                                //{
                                                //    values4[str3] = attribute;
                                                //}
                                            }
                                        }
                                    }
                                    values.Add(c);
                                    continue;

                            }
                        }

                       
                        this._CollectionArray = values;

                        
                        this._HttpContext.Cache.Insert(this._key, this._CollectionArray, null, DateTime.MaxValue, TimeSpan.FromMinutes(10.0), CacheItemPriority.NotRemovable, null);
                    }
                }
                return this._CollectionArray;
            }

        }
    


}
