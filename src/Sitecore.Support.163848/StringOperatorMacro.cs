using Sitecore.Rules.RuleMacros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Text;
using System.Xml.Linq;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Data.Items;
using Sitecore.Web.UI.Sheer;
using Sitecore.Rules;

namespace Sitecore.Support.Rules.RuleMacros
{
    public class StringOperatorMacro : IRuleMacro
    {
        public void Execute(XElement element, string name, UrlString parameters, string value)
        {
            Assert.ArgumentNotNull(element, "element");
            Assert.ArgumentNotNull(name, "name");
            Assert.ArgumentNotNull(parameters, "parameters");
            Assert.ArgumentNotNull(value, "value");
            SelectItemOptions selectItemOptions = new SelectItemOptions();
            Item item = null;
            if (!string.IsNullOrEmpty(value))
            {
                item = Client.ContentDatabase.GetItem(value);
            }
            string value2 = XElement.Parse(element.ToString()).FirstAttribute.Value;
            if (!string.IsNullOrEmpty(value2))
            {
                Item item2 = Client.ContentDatabase.GetItem(value2);
                if (item2 != null)
                {
                    selectItemOptions.FilterItem = item2;
                }
            }
            selectItemOptions.Root = Client.ContentDatabase.GetItem(RuleIds.StringOperatorsFolder);
            selectItemOptions.SelectedItem = (item ?? ((selectItemOptions.Root != null) ? selectItemOptions.Root.Children.FirstOrDefault<Item>() : null));
            selectItemOptions.Title = "Select Comparison";
            selectItemOptions.Text = "Select the alphabetical comparison to use for in rule.";
            selectItemOptions.Icon = "applications/32x32/media_stop.png";
            selectItemOptions.ShowRoot = false;
            UrlString url = selectItemOptions.ToUrlString();
            url.Append(parameters);
            SheerResponse.ShowModalDialog(url.ToString(), "1200px", "700px", string.Empty, true);
        }
    }
}