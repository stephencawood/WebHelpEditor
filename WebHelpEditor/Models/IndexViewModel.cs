using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace WebHelpEditor.Models
{
    public class IndexViewModel
    {
        public static SelectList GetSites()
        {
            var sites = WebHelpEditor.Models.Site.GetSites();

            return new SelectList(sites.ToList(), "ID", "Name");
        }
    }
}