using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data;
using System.IO;
using Nistec;


namespace Nistec.Web.Asp
{



    /// <summary>
    /// Summary description for ExportUtil
    /// </summary>
    public class Export
    {
        #region Export


        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="gv">GridView control</param>
        public static void ExportToExcel(Page page, ListView gv, string filename)
        {
            ExportToExcelInternal(page, gv, "Windows-1255", filename, true);
        }

        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="gv">GridView control</param>
        public static void ExportToExcel(Page page, ListView gv, string encoding, string filename, bool addTime)
        {

            ExportToExcelInternal(page, gv, encoding, filename, addTime);
        }

        private static void ExportToExcelInternal(Page page, System.Web.UI.Control gv, string encoding, string filename, bool addTime)
        {
            string style = @"<style> .text { mso-number-format:\@; } </style> ";

            //Response.Buffer = true;
            page.Response.ClearContent();

            page.Response.ContentEncoding = Encoding.GetEncoding(encoding);//"windows-1255");
            page.Response.Charset = "iso8859-8";// encoding;// "windows-1255";

            string file_name = filename + ".xls";
            if (addTime)
            {
                file_name = filename + "{" + DateTime.Today.ToString("dd-MM-yyyy") + "}.xls";
            }
            string attachment = string.Format("attachment; filename={0}", file_name);
            page.Response.AddHeader("content-disposition", attachment);
            page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //page.Response.ContentType = "application/excel";//"application/vnd.xls";
            page.Response.ContentType = "application/vnd.ms-excel";

            //this.EnableViewState = false;

            //DisableControls(gv);
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            // Style is added dynamically
            page.Response.Write(style);
            page.Response.Write(sw.ToString());

            page.Response.End();
        }

        public static void DisableControls(Control gv)
        {
            LinkButton lb = new LinkButton();
            Literal l = new Literal();

            string name = String.Empty;

            for (int i = 0; i < gv.Controls.Count; i++)
            {
                Type type = gv.Controls[i].GetType();
                if (type == typeof(LinkButton))
                {
                    l.Text = (gv.Controls[i] as LinkButton).Text;
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (type == typeof(DropDownList))
                {
                    l.Text = (gv.Controls[i] as DropDownList).SelectedItem.Text;
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (type == typeof(CheckBox))
                {
                    l.Text = (gv.Controls[i] as CheckBox).Checked ? "yes" : "no";
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }

                if (gv.Controls[i].HasControls())
                {
                    DisableControls(gv.Controls[i]);
                }
            }

        }

        public static void VerifyRenderingInServerForm(Control control)
        {

        }

        //public static void HandleRowDataBound(AspGridRowEventArgs e)
        //{

        //    if (e.Row.RowType == AspDataControlRowType.DataRow)
        //    {
        //        e.Row.Cells[1].Attributes.Add("class", "text");
        //    }
        //}


        public static void ExportData(Page p, ObjectDataSource dataSource, string[] headers, string filename, bool addTime)
        {
            DataView dv = (DataView)dataSource.Select();
            if (dv == null) return;
            ExportData(p, dv.Table, headers, filename, addTime);
        }

        public static void ExportData(Page p, System.Collections.IEnumerable dataSource, string[] headers, string filename, bool addTime)
        {
            DataView dv = (DataView)dataSource;
            if (dv == null) return;
            ExportData(p, dv.Table, headers, filename, addTime);
        }

        public static void ExportData(Page p, DataTable dt, string[] headers, string filename, bool addTime)
        {
            //string style = @"<style> .text { mso-number-format:\@; } </style> ";
            if (dt == null || dt.Rows.Count == 0)
                return;
            //DataTable dt = GetData();
            HttpResponse Response = p.Response;
            string file_name = filename + ".xls";
            if (addTime)
            {
                file_name = filename + "{" + DateTime.Today.ToString("dd-MM-yyyy") + "}.xls";
            }
            string attachment = string.Format("attachment; filename={0}", file_name);

            //string attachment = "attachment; filename=Employee.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.GetEncoding("windows-1255");//"windows-1255");
            Response.Charset = "utf-8";// "iso8859-8";// encoding;// "windows-1255";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //Response.Write(style);

            string tab = "";
            if (headers != null)
            {
                foreach (string s in headers)
                {
                    Response.Write(tab + s);
                    tab = "\t";
                }
            }
            else
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    Response.Write(tab + dc.ColumnName);
                    tab = "\t";
                }
            }
            Response.Write("\n");

            int i;
            foreach (DataRow dr in dt.Rows)
            {
                tab = "";
                for (i = 0; i < dt.Columns.Count; i++)
                {
                    Response.Write(tab + dr[i].ToString());
                    tab = "\t";
                }
                Response.Write("\n");
            }
            Response.End();
        }


        public static void ExportData(Page p, ObjectDataSource dataSource, string[] columns, string[] headers, string filename, bool addTime)
        {
            DataView dv = (DataView)dataSource.Select();
            if (dv == null) return;
            ExportData(p, dv.Table, columns, headers, filename, addTime);
        }
        public static void ExportData(Page p, System.Collections.IEnumerable dataSource, string[] columns, string[] headers, string filename, bool addTime)
        {
            DataView dv = (DataView)dataSource;
            if (dv == null) return;
            ExportData(p, dv.Table, columns, headers, filename, addTime);
        }
        public static void ExportData(Page p, DataTable dt, string[] columns, string[] headers, string filename, bool addTime)
        {
            //string style = @"<style> .text { mso-number-format:\@; } </style> ";
            if (dt == null || dt.Rows.Count == 0)
                return;
            //DataTable dt = GetData();
            HttpResponse Response = p.Response;
            string file_name = filename + ".xls";
            if (addTime)
            {
                file_name = filename + "{" + DateTime.Today.ToString("dd-MM-yyyy") + "}.xls";
            }
            string attachment = string.Format("attachment; filename={0}", file_name);

            //string attachment = "attachment; filename=Employee.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.GetEncoding("windows-1255");//"windows-1255");
            Response.Charset = "utf-8";// "iso8859-8";// encoding;// "windows-1255";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //Response.Write(style);

            string tab = "";
            if (headers != null)
            {
                foreach (string s in headers)
                {
                    Response.Write(tab + s);
                    tab = "\t";
                }
            }
            else
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    Response.Write(tab + dc.ColumnName);
                    tab = "\t";
                }
            }
            Response.Write("\n");



            int i;
            if (columns != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tab = "";
                    foreach (string c in columns)
                    {
                        Response.Write(tab + dr[c].ToString());
                        tab = "\t";
                    }
                    Response.Write("\n");
                }
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tab = "";
                    for (i = 0; i < dt.Columns.Count; i++)
                    {
                        Response.Write(tab + dr[i].ToString());
                        tab = "\t";
                    }
                    Response.Write("\n");
                }
            }
            Response.End();
        }

        public static void ExportDataFields(Page p, System.Collections.IEnumerable dataSource, DataControlFieldCollection columns)
        {
            DataView dv = (DataView)dataSource;
            if (dv == null) return;
            ExportDataFields(p, dv.Table, columns, "report_", true);
        }

        public static void ExportDataFields(Page p, System.Collections.IEnumerable dataSource, DataControlFieldCollection columns, string filename)
        {
            DataView dv = (DataView)dataSource;
            if (dv == null) return;
            ExportDataFields(p, dv.Table, columns, filename, true);
        }

        private static void ExportDataFields(Page p, DataTable dt, DataControlFieldCollection columns, string filename, bool addTime)
        {
            //string style = @"<style> .text { mso-number-format:\@; } </style> ";
            if (dt == null || dt.Rows.Count == 0)
                return;
            //DataTable dt = GetData();
            HttpResponse Response = p.Response;
            string file_name = filename + ".xls";
            if (addTime)
            {
                file_name = filename + "{" + DateTime.Today.ToString("dd-MM-yyyy") + "}.xls";
            }
            string attachment = string.Format("attachment; filename={0}", file_name);

            //string attachment = "attachment; filename=Employee.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.GetEncoding("windows-1255");//"windows-1255");
            Response.Charset = "utf-8";// "iso8859-8";// encoding;// "windows-1255";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //Response.Write(style);

            string tab = "";


            if (columns != null)
            {
                foreach (object o in columns)
                {
                    if (o is BoundField)
                    {
                        Response.Write(tab + ((BoundField)o).HeaderText);
                        tab = "\t";
                    }
                }
            }
            else
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    Response.Write(tab + dc.ColumnName);
                    tab = "\t";
                }
            }
            Response.Write("\n");



            int i;
            if (columns != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tab = "";
                    foreach (object o in columns)
                    {
                        if (o is BoundField)
                        {
                            Response.Write(tab + dr[((BoundField)o).DataField].ToString());
                            tab = "\t";
                        }
                    }
                    Response.Write("\n");
                }
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tab = "";
                    for (i = 0; i < dt.Columns.Count; i++)
                    {
                        Response.Write(tab + dr[i].ToString());
                        tab = "\t";
                    }
                    Response.Write("\n");
                }
            }
            Response.End();
        }

        public static string[] GetColumnNames(DataTable dt)
        {
            return dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
       }

        public static DataControlFieldCollection GetColumns(ListView lv, int count, string prefix)
        {
            DataControlFieldCollection col = new DataControlFieldCollection();
            for (int i = 1; i <= count; i++)
            {
                LinkButton link = (LinkButton)lv.FindControl(prefix + i.ToString());
                if (link != null)
                {
                    col.Add(GridHelper.CreateGridField(link.CommandArgument, link.Text, "", ""));
                }
            }
            return col;
        }
        public static DataControlFieldCollection GetColumns(ListView lv, int count, string prefix, bool allowSort, Dictionary<string, string> exFields)
        {
            DataControlFieldCollection col = new DataControlFieldCollection();
            for (int i = 1; i <= count; i++)
            {
                LinkButton link = (LinkButton)lv.FindControl(prefix + i.ToString());
                col.Add(GridHelper.CreateGridField(link.CommandArgument, link.Text, "", allowSort ? link.CommandArgument : ""));

            }
            if (exFields != null)
            {
                foreach (KeyValuePair<string, string> c in exFields)
                {
                    col.Add(GridHelper.CreateGridField(c.Key, c.Value, "", allowSort ? c.Key : ""));
                }
            }
            return col;
        }
        public static DataControlFieldCollection GetValidColumns(DataControlFieldCollection fields)
        {
            DataControlFieldCollection col = new DataControlFieldCollection();
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i] is BoundField)
                    col.Add(fields[i]);
            }
            return col;
        }

        public static DataControlFieldCollection GetValidColumns(DataControlFieldCollection fields, ListItemCollection templateFields)
        {
            DataControlFieldCollection col = new DataControlFieldCollection();
            for (int i = 0; i < templateFields.Count; i++)
            {
                BoundField bf = new BoundField();
                bf.DataField = templateFields[i].Value;
                bf.HeaderText = templateFields[i].Text;
                col.Add(bf);
            }
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i] is BoundField)
                    col.Add(fields[i]);
            }
            return col;
        }


        #endregion

    }

    /*
    /// <summary>
    /// Summary description for ExportUtil
    /// </summary>
    public class Export
    {
        #region Export


        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="gv">GridView control</param>
        public static void ExportToExcel(Page page, ListView gv, string filename)
        {
            ExportToExcelInternal(page, gv, "Windows-1255", filename, true);
        }

        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="gv">GridView control</param>
        public static void ExportToExcel(Page page, ListView gv, string encoding, string filename, bool addTime)
        {

            ExportToExcelInternal(page, gv, encoding, filename, addTime);
        }

        private static void ExportToExcelInternal(Page page, System.Web.UI.Control gv, string encoding, string filename, bool addTime)
        {
            string style = @"<style> .text { mso-number-format:\@; } </style> ";

            //Response.Buffer = true;
            page.Response.ClearContent();

            page.Response.ContentEncoding = Encoding.GetEncoding(encoding);//"windows-1255");
            page.Response.Charset = "iso8859-8";// encoding;// "windows-1255";

            string file_name = filename + ".xls";
            if (addTime)
            {
                file_name = filename + "{" + DateTime.Today.ToString("dd-MM-yyyy") + "}.xls";
            }
            string attachment = string.Format("attachment; filename={0}", file_name);
            page.Response.AddHeader("content-disposition", attachment);
            page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //page.Response.ContentType = "application/excel";//"application/vnd.xls";
            page.Response.ContentType = "application/vnd.ms-excel";

            //this.EnableViewState = false;

            //DisableControls(gv);
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            // Style is added dynamically
            page.Response.Write(style);
            page.Response.Write(sw.ToString());

            page.Response.End();
        }

        public static void DisableControls(Control gv)
        {
            LinkButton lb = new LinkButton();
            Literal l = new Literal();

            string name = String.Empty;

            for (int i = 0; i < gv.Controls.Count; i++)
            {
                Type type = gv.Controls[i].GetType();
                if (type == typeof(LinkButton))
                {
                    l.Text = (gv.Controls[i] as LinkButton).Text;
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (type == typeof(DropDownList))
                {
                    l.Text = (gv.Controls[i] as DropDownList).SelectedItem.Text;
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (type == typeof(CheckBox))
                {
                    l.Text = (gv.Controls[i] as CheckBox).Checked ? "yes" : "no";
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }

                if (gv.Controls[i].HasControls())
                {
                    DisableControls(gv.Controls[i]);
                }
            }

        }

        public static void VerifyRenderingInServerForm(Control control)
        {

        }

        //public static void HandleRowDataBound(AspGridRowEventArgs e)
        //{

        //    if (e.Row.RowType == AspDataControlRowType.DataRow)
        //    {
        //        e.Row.Cells[1].Attributes.Add("class", "text");
        //    }
        //}


        public static void ExportData(Page p, ObjectDataSource dataSource, string[] headers, string filename, bool addTime)
        {
            DataView dv = (DataView)dataSource.Select();
            if (dv == null) return;
            ExportData(p, dv.Table, headers, filename, addTime);
        }

        public static void ExportData(Page p, System.Collections.IEnumerable dataSource, string[] headers, string filename, bool addTime)
        {
            DataView dv = (DataView)dataSource;
            if (dv == null) return;
            ExportData(p, dv.Table, headers, filename, addTime);
        }

        public static void ExportData(Page p, DataTable dt, string[] headers, string filename, bool addTime)
        {
            //string style = @"<style> .text { mso-number-format:\@; } </style> ";
            if (dt == null || dt.Rows.Count == 0)
                return;
            //DataTable dt = GetData();
            HttpResponse Response = p.Response;
            string file_name = filename + ".xls";
            if (addTime)
            {
                file_name = filename + "{" + DateTime.Today.ToString("dd-MM-yyyy") + "}.xls";
            }
            string attachment = string.Format("attachment; filename={0}", file_name);

            //string attachment = "attachment; filename=Employee.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.GetEncoding("windows-1255");//"windows-1255");
            Response.Charset = "utf-8";// "iso8859-8";// encoding;// "windows-1255";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //Response.Write(style);

            string tab = "";
            if (headers != null)
            {
                foreach (string s in headers)
                {
                    Response.Write(tab + s);
                    tab = "\t";
                }
            }
            else
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    Response.Write(tab + dc.ColumnName);
                    tab = "\t";
                }
            }
            Response.Write("\n");

            int i;
            foreach (DataRow dr in dt.Rows)
            {
                tab = "";
                for (i = 0; i < dt.Columns.Count; i++)
                {
                    Response.Write(tab + dr[i].ToString());
                    tab = "\t";
                }
                Response.Write("\n");
            }
            Response.End();
        }


        public static void ExportData(Page p, ObjectDataSource dataSource, string[] columns, string[] headers, string filename, bool addTime)
        {
            DataView dv = (DataView)dataSource.Select();
            if (dv == null) return;
            ExportData(p, dv.Table, columns, headers, filename, addTime);
        }
        public static void ExportData(Page p, System.Collections.IEnumerable dataSource, string[] columns, string[] headers, string filename, bool addTime)
        {
            DataView dv = (DataView)dataSource;
            if (dv == null) return;
            ExportData(p, dv.Table, columns, headers, filename, addTime);
        }
        public static void ExportData(Page p, DataTable dt, string[] columns, string[] headers, string filename, bool addTime)
        {
            //string style = @"<style> .text { mso-number-format:\@; } </style> ";
            if (dt == null || dt.Rows.Count == 0)
                return;
            //DataTable dt = GetData();
            HttpResponse Response = p.Response;
            string file_name = filename + ".xls";
            if (addTime)
            {
                file_name = filename + "{" + DateTime.Today.ToString("dd-MM-yyyy") + "}.xls";
            }
            string attachment = string.Format("attachment; filename={0}", file_name);

            //string attachment = "attachment; filename=Employee.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.GetEncoding("windows-1255");//"windows-1255");
            Response.Charset = "utf-8";// "iso8859-8";// encoding;// "windows-1255";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //Response.Write(style);

            string tab = "";
            if (headers != null)
            {
                foreach (string s in headers)
                {
                    Response.Write(tab + s);
                    tab = "\t";
                }
            }
            else
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    Response.Write(tab + dc.ColumnName);
                    tab = "\t";
                }
            }
            Response.Write("\n");



            int i;
            if (columns != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tab = "";
                    foreach (string c in columns)
                    {
                        Response.Write(tab + dr[c].ToString());
                        tab = "\t";
                    }
                    Response.Write("\n");
                }
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tab = "";
                    for (i = 0; i < dt.Columns.Count; i++)
                    {
                        Response.Write(tab + dr[i].ToString());
                        tab = "\t";
                    }
                    Response.Write("\n");
                }
            }
            Response.End();
        }



        public static void ExportDataFields(Page p, System.Collections.IEnumerable dataSource, DataControlFieldCollection columns)
        {
            DataView dv = (DataView)dataSource;
            if (dv == null) return;
            ExportDataFields(p, dv.Table, columns, "ephone_", true);
        }
        private static void ExportDataFields(Page p, DataTable dt, DataControlFieldCollection columns, string filename, bool addTime)
        {
            //string style = @"<style> .text { mso-number-format:\@; } </style> ";
            if (dt == null || dt.Rows.Count == 0)
                return;
            //DataTable dt = GetData();
            HttpResponse Response = p.Response;
            string file_name = filename + ".xls";
            if (addTime)
            {
                file_name = filename + "{" + DateTime.Today.ToString("dd-MM-yyyy") + "}.xls";
            }
            string attachment = string.Format("attachment; filename={0}", file_name);

            //string attachment = "attachment; filename=Employee.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.GetEncoding("windows-1255");//"windows-1255");
            Response.Charset = "utf-8";// "iso8859-8";// encoding;// "windows-1255";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //Response.Write(style);

            string tab = "";


            if (columns != null)
            {
                foreach (object o in columns)
                {
                    if (o is BoundField)
                    {
                        Response.Write(tab + ((BoundField)o).HeaderText);
                        tab = "\t";
                    }
                }
            }
            else
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    Response.Write(tab + dc.ColumnName);
                    tab = "\t";
                }
            }
            Response.Write("\n");



            int i;
            if (columns != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tab = "";
                    foreach (object o in columns)
                    {
                        if (o is BoundField)
                        {
                            Response.Write(tab + dr[((BoundField)o).DataField].ToString());
                            tab = "\t";
                        }
                    }
                    Response.Write("\n");
                }
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tab = "";
                    for (i = 0; i < dt.Columns.Count; i++)
                    {
                        Response.Write(tab + dr[i].ToString());
                        tab = "\t";
                    }
                    Response.Write("\n");
                }
            }
            Response.End();
        }

        public static DataControlFieldCollection GetColumns(ListView lv, int count, string prefix)
        {
            DataControlFieldCollection col = new DataControlFieldCollection();
            for (int i = 1; i <= count; i++)
            {
                LinkButton link = (LinkButton)lv.FindControl(prefix + i.ToString());
                col.Add(GridHelper.CreateGridField(link.CommandArgument, link.Text, "", ""));

            }
            return col;
        }

        public static DataControlFieldCollection GetValidColumns(DataControlFieldCollection fields)
        {
            DataControlFieldCollection col = new DataControlFieldCollection();
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i] is BoundField)
                    col.Add(fields[i]);
            }
            return col;
        }

        public static DataControlFieldCollection GetValidColumns(DataControlFieldCollection fields, ListItemCollection templateFields)
        {
            DataControlFieldCollection col = new DataControlFieldCollection();
            for (int i = 0; i < templateFields.Count; i++)
            {
                BoundField bf = new BoundField();
                bf.DataField = templateFields[i].Value;
                bf.HeaderText = templateFields[i].Text;
                col.Add(bf);
            }
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i] is BoundField)
                    col.Add(fields[i]);
            }
            return col;
        }


        #endregion

    }
    */
}