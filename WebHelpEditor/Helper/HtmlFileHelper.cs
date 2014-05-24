using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;

namespace WebHelpEditor.Helper
{
    public class HtmlFileHelper
    {
        /// <summary>
        /// Get title from an HTML string.
        /// </summary>
        public static string GetTitle(string file)
        {
            Match m = Regex.Match(file, @"<title>\s*(.+?)\s*</title>");
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
            {
                return "Error: No Title";
            }
        }
    }
}