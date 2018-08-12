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

    public class CmsPageEdit
    {
        IList<CmsItem> items;
        //StringBuilder sb;

        string _Script;
        public string Script
        {
            get { return _Script; }
        }

        string _Body;
        public string Body
        {
            get { return _Body; }
        }

        public string Page
        {
            get { return Body + "\r\n" + Script; }
        }

        public CmsPageEdit(int siteId,int pageId)
        {
            items = CmsItem.ViewPage(siteId,pageId);
        }

        public void CreatePage()
        {
            int count = items.Count;
            CreateScript(count);
            CreateBody(count);
        }

        private void CreateScript(int count)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("$(document).ready(function () {");

            for(int i=0;i<count;i++)
            {
                sb.AppendFormat("$('#editor{0}').jqxEditor({ tools: 'bold italic underline | format font size | color background | left center right' });",i);
                sb.AppendLine();
            }

            sb.AppendLine("</script>");
            _Script =sb.ToString();
        }

        private void CreateBody(int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                var item = items[i];
                //sb.AppendFormat("<div style=\"width: 45%; float: left;\" contenteditable=\"true\" id=\"editor{0}\">",i);
                sb.AppendFormat("<div style=\"width: 80%;\" contenteditable=\"true\" id=\"editor{0}\">", i);
                sb.AppendLine(item.ItemContent);
                sb.AppendLine("</div>");
            }
            _Body = sb.ToString();
        }

    }
}
