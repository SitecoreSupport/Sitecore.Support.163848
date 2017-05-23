using Sitecore.Diagnostics;
using Sitecore.Text;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Shell.Applications.Rules.RulesEditor
{
    public class RulesEditorPage : Sitecore.Shell.Applications.Rules.RulesEditor.RulesEditorPage
    {
        protected new void Edit(string uid, string name, string control, string parameters)
        {
            Assert.ArgumentNotNull(parameters, "parameters");           
            if (name == "operatorid" && control == "StringOperator")
            {
                UrlHandle handle = UrlHandle.Get();
                string p = handle["hideOperators"];
                if (!string.IsNullOrEmpty(p))
                {
                    UrlString url = new UrlString(parameters);
                    url["hideOperators"] = p;
                    parameters = url.ToString();
                }
            }

            base.Edit(uid, name, control, parameters);
           
        }
    }
}