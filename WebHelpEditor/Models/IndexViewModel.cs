using System.Linq;

using System.Web.Mvc;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;

namespace WebHelpEditor.Models
{
    public class IndexViewModel
    {
        public string DropdownName = "LanguageDropdown";
        public static SelectList GetLanguages()
        {
            var sites = WebHelpEditor.Models.Language.GetLanguages();

            return new SelectList(sites.ToList(), "Id", "Name");
        }
    }
}