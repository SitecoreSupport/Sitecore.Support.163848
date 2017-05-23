using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Sitecore.Extensions.XElementExtensions;
using Sitecore.Text;
using Sitecore.Web;

namespace Sitecore.Support.Shell.Applications.Analytics.SegmentBuilder
{
    public class SegmentBuilderForm: Sitecore.Shell.Applications.Analytics.SegmentBuilder.SegmentBuilderForm
    {
        protected new void EditFilterCondition(Sitecore.Web.UI.Sheer.ClientPipelineArgs args)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(args, "args");
            string conditionId;
            if (!string.IsNullOrEmpty(args.Parameters["id"]))
            {
                conditionId = Sitecore.Data.ID.Decode(args.Parameters["id"]).ToString();
            }
            else
            {
                conditionId = Sitecore.Data.ID.NewID.ToString();
            }
            if (!args.IsPostBack)
            {
                Sitecore.Shell.Applications.Dialogs.RulesEditor.RulesEditorOptions rulesEditorOptions = new Sitecore.Shell.Applications.Dialogs.RulesEditor.RulesEditorOptions
                {
                    IncludeCommon = false,
                    RulesPath = "/sitecore/system/Settings/Rules/Segment Builder",
                    AllowMultiple = false,
                    HideActions = true,
                    PreviewRulesExecutionResults = SegmentBuilderSettings.VisitorCountsEnabled
                };
                if (rulesEditorOptions.PreviewRulesExecutionResults)
                {
                    rulesEditorOptions.RulesExecutionPreviewerType = SegmentBuilderSettings.RulesExecutionPreviewer;
                }
                XElement xElement = (from node in this.FilterSet.Elements("rule")
                                     where node.GetAttributeValue("uid") == conditionId
                                     select node).FirstOrDefault<XElement>();
                if (xElement != null)
                {
                    rulesEditorOptions.Value = "<ruleset>" + xElement + "</ruleset>";
                }

                //add parameters to handle
                UrlString url = rulesEditorOptions.ToUrlString();
                UrlHandle handle= UrlHandle.Get(url, UrlHandle.DefaultHandleName,true);
                handle["hideOperators"] = "{537244C2-3A3F-4B81-A6ED-02AF494C0563}|{6A7294DF-ECAE-4D5F-A8D2-C69CB1161C09}";
                handle.Add(url);
                //end
                Sitecore.Web.UI.Sheer.SheerResponse.ShowModalDialog(url.ToString(), true);
                args.WaitForPostBack();
                return;
            }
            if (args.HasResult && string.Compare(args.Result.Trim(), "-", true) != 0)
            {
                string result = args.Result;
                XElement xElement2 = XElement.Parse(result).Element("rule");
                XElement filterSet = this.FilterSet;
                if (xElement2 != null)
                {
                    XElement xElement3 = (from node in filterSet.Elements("rule")
                                          where node.GetAttributeValue("uid") == conditionId
                                          select node).FirstOrDefault<XElement>();
                    if (xElement3 != null)
                    {
                        xElement3.ReplaceWith(xElement2);
                        this.FilterSet = filterSet;
                        Sitecore.Web.UI.Sheer.SheerResponse.SetInnerHtml(args.Parameters["id"] + "_rule", this.GetRuleConditionsHtml(xElement2));
                        Sitecore.Web.UI.Sheer.SheerResponse.SetInnerHtml(conditionId + "_count", Sitecore.Globalization.Translate.Text("Calculating..."));
                        IEnumerable<XElement> source = filterSet.Elements("rule");
                        int num = 0;
                        while (num < source.Count<XElement>() && string.Compare(source.ElementAt(num).GetAttributeValue("uid"), conditionId, true) != 0)
                        {
                            num++;
                        }
                        this.RecalculateVisitorCount(num);
                    }
                    else
                    {
                        XElement xElement4 = new XElement("rule");
                        xElement4.SetAttributeValue("uid", conditionId);
                        xElement4.Add(from el in xElement2.Elements()
                                      select el);
                        filterSet.Add(xElement4);
                        this.FilterSet = filterSet;
                        string filterSectionHtml = this.GetFilterSectionHtml(xElement4, false);
                        Sitecore.Web.UI.Sheer.SheerResponse.Insert("non-default-container", "append", filterSectionHtml);
                        this.RecalculateVisitorCount(filterSet.Elements("rule").Count<XElement>() - 1);
                    }
                    this.UpdateTotalVisitorsInSegment();
                }
            }
        }
    }
}