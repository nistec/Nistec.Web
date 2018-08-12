using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web.UI.WebControls;


namespace Nistec.Web
{
    public partial class ListDataHelper
    {

        #region static AddChoose

        public static void AddChoose(DataTable dt, int textField, int valueField, string text, object value)
        {
            DataRow dr = dt.NewRow();
            dr[textField] = text;
            dr[valueField] = value;
            dt.Rows.InsertAt(dr, 0);
        }
        public static void AddChoose(DataTable dt, string textField, string valueField, string text, object value)
        {
            DataRow dr = dt.NewRow();
            dr[textField] = text;
            dr[valueField] = value;
            dt.Rows.InsertAt(dr, 0);
        }

        public static void AddChoose(ListControl ctl, string text, string value)
        {
            ctl.Items.Insert(0, new ListItem(text, value));
        }
        #endregion
       
        #region Bind list

        public static void BindList(ListControl ctl, string textField, string valueField, DataTable dt)
        {
            ctl.DataTextField = textField;
            ctl.DataValueField = valueField;
            ctl.DataSource = dt;
            ctl.DataBind();
        }
        public static void BindList(ListControl ctl, string textField, string valueField, DataTable dt, string addChoosText, object addChoosValue)
        {
            DataRow dr = dt.NewRow();
            dr[textField] = addChoosText;
            dr[valueField] = addChoosValue;
            dt.Rows.InsertAt(dr, 0);

            ctl.DataTextField = textField;
            ctl.DataValueField = valueField;
            ctl.DataSource = dt;
            ctl.DataBind();
        }
        public static void BindList(ListControl ctl, string textField, string valueField, DataTable dt, string selectedValue)
        {
            ctl.DataTextField = textField;
            ctl.DataValueField = valueField;
            ctl.DataSource = dt;
            ctl.DataBind();
            if (ctl.Items.Count > 0 && selectedValue != null)
            {
                ctl.Text = selectedValue;
            }
        }
        public static void BindList(ListControl ctl, ListItem[] items, string selectedValue)
        {
            ctl.Items.Clear();
            if (items != null)
            {
                ctl.Items.AddRange(items);
                if (ctl.Items.Count > 0 && selectedValue != null)
                {
                    ctl.Text = selectedValue;
                }
            }
        }
      

        public static void BindList(ListControl ctl, string textField, string valueField, object dt)
        {
            ctl.DataTextField = textField;
            ctl.DataValueField = valueField;
            ctl.DataSource = dt;
            ctl.DataBind();
        }

        public static void BindList(ListControl ctl, Type enumType)
        {
            ctl.Items.Clear();
            ctl.Items.AddRange(GetListItems(enumType));
        }
        #endregion

        #region Select item

        public static void SelectedItemByValue(DropDownList ctl, string value, string defaultValue)
        {
            ListItem item = ctl.Items.FindByValue(value);
            if (item != null)
            {
                ctl.ClearSelection();
                item.Selected = true;
            }
            else if (!string.IsNullOrEmpty(defaultValue))
            {
                item = ctl.Items.FindByValue(defaultValue);
                if (item != null)
                {
                    ctl.ClearSelection();
                    item.Selected = true;
                }
            }
        }
        public static void SelectedItemByText(DropDownList ctl, string value, string defaultValue)
        {
            ListItem item = ctl.Items.FindByText(value);
            if (item != null)
            {
                ctl.ClearSelection();
                item.Selected = true;
            }
            else if (!string.IsNullOrEmpty(defaultValue))
            {
                item = ctl.Items.FindByText(defaultValue);
                if (item != null)
                {
                    ctl.ClearSelection();
                    item.Selected = true;
                }
            }
        }
        #endregion

        #region Get List Items

        public  static ListItem[] GetListItems(params string[] args)
        {
            List<ListItem> items= new List<ListItem>();
            foreach(string s in args)
            {
                items.Add(new ListItem(s));
            }
            return items.ToArray();
        }

        public static ListItem[] GetListItems(int from, int to)
        {
            List<ListItem> items = new List<ListItem>();
            for (int i = from; i <= to; i++)
            {
                items.Add(new ListItem(i.ToString()));
            }
            return items.ToArray();
        }

        public static ListItem[] GetListItemsWithValue(params string[] args)
        {
            List<ListItem> items = new List<ListItem>();
            for (int i = 0; i < args.Length; i++)
            {
                items.Add(new ListItem(args[i], args[i + 1]));
                i++;
            }
            return items.ToArray();
        }


        public static ListItem[] GetListItems(Type enumType)
        {
            string[] list = Enum.GetNames(enumType);
            int[] values =(int[]) Enum.GetValues(enumType);

            List<ListItem> items = new List<ListItem>();
            int i=0;
            foreach (string s in list)
            {
                items.Add(new ListItem(s,values[i].ToString()));
                i++;
            }
            return items.ToArray();
        }
        #endregion
      
        #region Convert to

        public static ListItem[] DataTableToListItems(DataTable dt, string colText, string colValue, int topCount)
        {
            if (dt == null)
                return null;
            List<ListItem> items = new List<ListItem>();
            int index = 0;

            foreach (DataRow dr in dt.Rows)
            {
                items.Add(new ListItem(dr[colText].ToString(), dr[colValue].ToString()));
                if (topCount > 0)
                {
                    index++;
                    if (index > topCount)
                        break;
                }
            }
            return items.ToArray();
        }

        public static ListItem[] DataTableToListItems(DataTable dt, string colText,int topCount)
        {
            if (dt == null)
                return null;
            List<ListItem> items = new List<ListItem>();
            int index = 0;
            foreach (DataRow dr in dt.Rows)
            {
                items.Add(new ListItem(dr[colText].ToString()));
                if (topCount > 0)
                {
                    index++;
                    if (index > topCount)
                        break;
                }
            }
            return items.ToArray();
        }

        public static ListItem[] DataTableToListItems(DataTable dt, string colText, string colValue)
        {
            if (dt == null)
                return null;
            List<ListItem> items = new List<ListItem>();

            foreach (DataRow dr in dt.Rows)
            {
                items.Add(new ListItem(dr[colText].ToString(), dr[colValue].ToString()));
            }
            return items.ToArray();
        }

        public static ListItem[] DataTableToListItems(DataTable dt, string colText)
        {
            if (dt == null)
                return null;
            List<ListItem> items = new List<ListItem>();

            foreach (DataRow dr in dt.Rows)
            {
                items.Add(new ListItem(dr[colText].ToString()));
            }
            return items.ToArray();
        }
        #endregion

        public static DataTable Dayes()
        {
            DataTable dt = new DataTable("Dayes");
            dt.Columns.Add("Value");
            dt.Columns.Add("Text");
            dt.Rows.Add(new object[] { "0", "א" });
            dt.Rows.Add(new object[] { "1", "ב" });
            dt.Rows.Add(new object[] { "2", "ג" });
            dt.Rows.Add(new object[] { "3", "ד" });
            dt.Rows.Add(new object[] { "4", "ה" });
            dt.Rows.Add(new object[] { "5", "ו" });
            dt.Rows.Add(new object[] { "6", "ש" });

            return dt;
        }
        public static DataTable TimeMode()
        {
            DataTable dt = new DataTable("TimeMode");
            dt.Columns.Add("Value");
            dt.Columns.Add("Text");
            dt.Rows.Add(new object[] { "0", "דקות" });
            dt.Rows.Add(new object[] { "1", "שעות" });
            return dt;
        }

    }
}
