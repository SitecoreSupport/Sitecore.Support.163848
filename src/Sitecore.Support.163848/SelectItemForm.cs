using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sitecore.Support.Shell.Applications.Dialogs.SelectItem
{
    public class SelectItemForm: Sitecore.Shell.Applications.Dialogs.SelectItem.SelectItemForm
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if(this.DataContext!=null&& string.IsNullOrEmpty(this.DataContext.Filter))
            {
                string ids = WebUtil.GetQueryString("hideOperators");
               
                if (!string.IsNullOrEmpty(ids))
                {
                    ids = HttpUtility.UrlDecode(ids);
                    StringBuilder builder = new StringBuilder();
                    bool first = true;
                    foreach (string id in ids.Split(new char[] { '|' }))
                    {
                        if (!first)
                        {
                            builder.Append(" and ");
                        }
                        builder.Append("@@ID!='" + id + "'");

                        first = false;
                    }

                    this.DataContext.Filter = builder.ToString();
                }
                
            }
        }
    }
}