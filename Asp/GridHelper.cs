//#define TEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

namespace Nistec.Web.Asp
{


    /// <summary>
    /// Summary description for PF
    /// </summary>
    public class GridHelper
    {
 
         public static Unit GetGridHeight(int rows, int rowHeight, int maxRows, int defaultHeight)
        {
            if (rowHeight == 0)
                rowHeight = 36;

            if (rows < maxRows)
            {
                int h = rows * rowHeight;
                if (h < 80)
                    h = 80;
                return Unit.Parse(h.ToString());
            }
            else
            {
                return Unit.Parse(defaultHeight.ToString());
            }

        }

        public static BoundField CreateGridField(string DataField, string HeaderText, string DataFormatString, string SortExpression)
        {
            BoundField field = new BoundField();
            field.DataField = DataField;
            field.HeaderText = HeaderText;
            if (!string.IsNullOrEmpty(DataFormatString))
            {
                field.DataFormatString = DataFormatString;
            }
            if (!string.IsNullOrEmpty(SortExpression))
            {
                field.SortExpression = SortExpression;
            }
            return field;
        }
        public static HyperLinkField CreateGridLinkField(string DataField, string HeaderText, string DataUrlFormatString, string SortExpression, string Text)
        {

            HyperLinkField field = new HyperLinkField();
            field.DataNavigateUrlFields = new string[] { DataField };
            field.HeaderText = HeaderText;
            field.Target = "_blank";
            field.Text = Text;

            if (!string.IsNullOrEmpty(DataUrlFormatString))
            {
                field.DataNavigateUrlFormatString = DataUrlFormatString;
            }
            if (!string.IsNullOrEmpty(SortExpression))
            {
                field.SortExpression = SortExpression;
            }
            return field;
        }
        public static ButtonField CreateGridButtonField(string DataTextField, string HeaderText, string DataFormatString, string SortExpression, ButtonType buttonType, string commandName, string text)
        {
            //commands :     "Cancel" "Delete" "Edit" "Insert" "New" "Page" "Select" "Sort" "Update" 

            ButtonField field = new ButtonField();
            field.ButtonType = buttonType;
            field.HeaderText = HeaderText;
            field.CommandName = commandName;
            field.DataTextField = DataTextField;
            field.Text = text;
            if (!string.IsNullOrEmpty(DataFormatString))
            {
                field.DataTextFormatString = DataFormatString;
            }
            if (!string.IsNullOrEmpty(SortExpression))
            {
                field.SortExpression = SortExpression;
            }
            return field;
        }
        public static ButtonField CreateGridImageField(string DataTextField, string HeaderText, string DataFormatString, string SortExpression, string commandName, string imageUrl)
        {
            //commands :     "Cancel" "Delete" "Edit" "Insert" "New" "Page" "Select" "Sort" "Update" 

            ButtonField field = new ButtonField();
            field.ButtonType = ButtonType.Image;
            field.HeaderText = HeaderText;
            field.CommandName = commandName;
            field.DataTextField = DataTextField;
            field.ImageUrl = imageUrl;
            if (!string.IsNullOrEmpty(DataFormatString))
            {
                field.DataTextFormatString = DataFormatString;
            }
            if (!string.IsNullOrEmpty(SortExpression))
            {
                field.SortExpression = SortExpression;
            }
            return field;
        }
        public static void ChangeGridField(BoundField field, string DataField, string HeaderText, string DataFormatString, string SortExpression)
        {
            //BoundField field = new BoundField();
            field.DataField = DataField;
            field.HeaderText = HeaderText;
            if (!string.IsNullOrEmpty(DataFormatString))
            {
                field.DataFormatString = DataFormatString;
            }
            if (!string.IsNullOrEmpty(SortExpression))
            {
                field.SortExpression = SortExpression;
            }
        }

      

    }
}
