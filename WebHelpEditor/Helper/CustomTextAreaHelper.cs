using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//namespace to use HtmlHelper class
using System.Web.Mvc;
//namespace to use TagBuilder class
using System.Text;
//namespace to use RouteValueDictionary class
using System.Web.Routing;
using System.Web.Mvc.Html;
using System.IO;

///
///
/// usage: @Html.Custom_TextArea("textarea", "This is Custom TextArea", new { style = "color:Red;", id = "textarea" })
/// 
///
namespace WebHelpEditor.Helper
{
    public static class CustomTextAreaHelper
    {
        //Created a static method which accepts name as parameter. This method is extension method.
        //We can access this method using @html. 
        //This method in turn calls another method which is our second overload.
        public static HtmlString Custom_TextArea(this HtmlFileHelper helper, string name)
        {
            return Custom_TextArea(helper, name, null);
        }


        //Created a static method which accepts name and value as parameter. This method is extension method.
        //We can access this method using @html. 
        //This method in turn calls another method which is our third overload.
        public static HtmlString Custom_TextArea(this HtmlFileHelper helper, string name, string value)
        {
            return Custom_TextArea(helper, name, value, null);
        }


        //Created a static method which accepts name, value and htmlAttributes as parameter. This method is extension method.
        //We can access this method using @html.
        public static HtmlString Custom_TextArea(this HtmlFileHelper helper, string name, string value, object htmlAttributes)
        {
            //Created a textarea tag using TagBuilder class.
            TagBuilder textarea = new TagBuilder("textarea");
            //Setting its name attribute
            textarea.Attributes.Add("name", name);
            if (!string.IsNullOrEmpty(value))
            {
                //assigning the value passed as parameter. This valus is shown inside the TextArea.
                textarea.InnerHtml = value;
            }
            //Merging the attribute object.
            textarea.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(textarea.ToString());
        }

        //// Get the content of a file as XHTML/text
        //public static HtmlString GetFileBodyContent(this HtmlHelper helper, string text)
        //{
        //    // TODO - get just the body content

        //    return new HtmlString(text);
        //}


        //// Return the directory and file structure in HTML
        //public static HtmlString GetFilesInfo(this HtmlHelper helper, string directory)
        //{
        //    string text = "";

        //    try
        //    {
        //        foreach (var path in Directory.GetFiles(directory))
        //        {
        //            //TODO Created a textarea tag using TagBuilder class.
        //            //TagBuilder textarea = new TagBuilder("textarea");
        //            //text += "<a href=\"\" ng-click=\"loadContent()\"" + ">";
        //            text += "<a href=\"\" onclick=\"loadContent()\"" + ">";//" ng-cloak>";
        //            text += System.IO.Path.GetFileName(path);
        //            text += "</a><br/>";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogFile log = new LogFile();
        //        log.LogLine("Exception Caught", "GetFilesInfo: " + ex.Message + "Source: " + ex.Source);
        //    }

        //    return new HtmlString(text);
        //}   
    }
}