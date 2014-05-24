using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
// Added for cookies
using System.Web;
using System.IO;
//using Newtonsoft.Json;
using FileEditor.Helper;

namespace HelpEditor.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Test()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetFileContent(string filePath)
        {
            try
            {
                string path = Server.UrlDecode(filePath);
                string bodyContent = "";
                string bodyContentEnglish = "";
                string pathEnglish = path.Replace("help-fr", "help-en");

                if (System.IO.File.Exists(path))
                {
                    bodyContent = System.IO.File.ReadAllText(path);
                    bodyContentEnglish = System.IO.File.ReadAllText(pathEnglish);
                }
                else
                {
                    bodyContent = "ERROR. File does not exist: " + path + " or " + pathEnglish;
                }

                return Json
                    (
                        new
                        {
                            BodyContent = bodyContent,
                            BodyContentEnglish = bodyContentEnglish,
                            Path = path,
                            PathEnglish = pathEnglish,
                        }
                    , JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json
                    (
                        new
                        {
                            BodyContent = "Error loading file content: " + filePath,
                        }
                    , JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveFileContent(string filePath, string content)
        {
            var log = new LogFile();
            try
            {
                //log.LogLine("FilePath: " + filePath, " Date: " + DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss"));
                // Save new content to temp file
                //System.IO.File.Copy(filePath, filePath + "_backup_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".htm");

                // Fix up file header -- custom for wyzz editor control issues
                string divider = "css\">";
                int index = content.IndexOf(divider);
                string fixedContent = content.Remove(0, index + divider.Length);
                char[] arr = new char[] { '\t', ',', ' ', '\n' };
                fixedContent = fixedContent.TrimStart(arr);
                fixedContent = fixedContent.TrimEnd(arr);

                // TODO get title fix title
                string title = "TODO";

                fixedContent = "<html>\n\t<head>\n\t\t<title>" + title + "</title>\n\t\t<link rel=\"stylesheet\" type=\"text/css\" href=\"/AQUARIUS/help-en/include/templates/wwhelp.css\"/>\n\t</head>\n\t<body>\n" + fixedContent;
                
                // Fix up file footer
                fixedContent += "\n\t</body>\n</html>";

                // Write a temp version of the old file. Using create to overwrite any previous temp files
                System.IO.File.Create(filePath + "_temp").Close();
                using (var sw = new System.IO.StreamWriter(filePath + "_temp"))
                {
                    sw.WriteLine(fixedContent);
                }                

                // Backup the old file and replace it with the temp file
                System.IO.File.Replace(filePath + "_temp", filePath, filePath + "_backup_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".backup");

                return Json
                    (
                        new
                        {
                            Result = "Success",
                        }
                    );
            }
            catch (Exception ex)
            {
                log.LogLine("Home Controller SaveFileContent Error: ", "Message: " + ex.Message + "Source: " + ex.Source);

                return Json
                    (
                        new
                        {
                            Result = "Error saving content: " + ex.ToString(),
                        }
                    );
            }
        }

        //// temp test
        //public JsonResult TestAjax(string name, string location)
        //{
        //    //Save data
        //    var result = new BodyContent { TestReply = "User was saved!", Age = "40"};
        //    //Object data = null;
        //    //return Json(null, result);

        //    //JObject json = JObject.Parse(result);
        //    //string formatted = json.ToString();

        //    //Product product = new Product
        //    //{
        //    //    Name = "Apple",
        //    //    Expiry = new DateTime(2008, 12, 28),
        //    //    Price = 3.99M,
        //    //    Sizes = new[] { "Small", "Medium", "Large" }
        //    //};


        //public ActionResult CookieCreate(string name, string value)
        //{

        //    HttpCookie cookie = new HttpCookie(name);
        //    cookie.Value = value;
        //    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);

        //    /* TODO
        //    HttpCookie cookie = new HttpCookie("WelcomePatient");
        //    cookie.Value = "Created: " + DateTime.Now.ToShortTimeString();
        //    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
        //    cookie = new HttpCookie("DoxTreeMessage");
        //    cookie.Value = "Created: " + DateTime.Now.ToShortTimeString();
        //    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
        //    */

        //    return View();
        //}
    }
}
