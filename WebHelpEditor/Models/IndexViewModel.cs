using System.Linq;
using System.Web.Mvc;

namespace WebHelpEditor.Models
{
    public class IndexViewModel
    {
        public string DropdownName = "LanguageDropdown";
        public static SelectList GetLanguages(string configpath)
        {
            var sites = WebHelpEditor.Models.Language.GetLanguages(configpath);

            return new SelectList(sites.ToList(), "Id", "Name");
        }
    }
}