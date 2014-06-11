using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace WebHelpEditor.Models
{
    public class Language
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public static List<Language> GetLanguages(string path)
        {
            try
            {
                XDocument doc = XDocument.Load(path);
                var query = from site in doc.Root.Elements("Site")
                            select new Language
                            {
                                Id = (string)site.Element("Id"),
                                Name = (string)site.Element("Name"),
                                Path = (string)site.Element("Path"),
                            };

                var sites = query.ToList();
                return sites;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}