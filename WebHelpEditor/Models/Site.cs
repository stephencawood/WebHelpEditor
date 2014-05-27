using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using System.Xml.Linq;

namespace WebHelpEditor.Models
{
    public class Site
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public static List<Site> GetSites()
        {
            XDocument doc = XDocument.Load(HttpRuntime.AppDomainAppPath + "\\SitesConfig.xml");
            var query = from site in doc.Root.Elements("Site")
                        select new Site
                        {
                            ID = (string)site.Element("ID"),
                            Name = (string)site.Element("Name"),
                            Path = (string)site.Element("Path"),
                        };

            var sites = query.ToList();
            return sites;
        }
    }
}