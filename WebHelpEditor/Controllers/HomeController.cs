using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using System.Web.Security;
//using System.Web.Helpers;
//using BootstrapSupport;
using WebHelpEditor.Helper;
using WebHelpEditor.Models;

namespace WebHelpEditor.Controllers
{
    public class HomeController : Controller
    {        
        public ActionResult Index(string returnUrl)
        {
            Session["AlreadyPopulated"] = false;
            ViewBag.ReturnUrl = returnUrl;
            var path = Request.PhysicalApplicationPath;
            ViewBag.Languages = IndexViewModel.GetLanguages(path + "LanguagesConfig.xml");
            //ViewBag.EditLanguage = "english";
            return View("Index");
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [NoCache]
        [HttpGet]
        public ActionResult GetFileContent(string filePathEnglish, string languageCode)
        {
            try
            {
                if (!System.IO.File.Exists(HttpUtility.UrlDecode(filePathEnglish)))
                    return Json
                    (
                        new
                        {
                            BodyContent = "No File Selected",
                            BodyContentEnglish = "No File Selected",
                            Path = "",
                            PathEnglish = "",
                            HtmlTitle = "No File Selected",
                        }
                    , JsonRequestBehavior.AllowGet);


                // Get the english path from the multilingual path if necessary
                List<Language> languageList = Language.GetLanguages(Request.PhysicalApplicationPath + "LanguagesConfig.xml");
                //var englishLanguage = languageList.Find(i => i.Name.ToLower() == "english");
                //var pathEnglish = englishLanguage.PathWeb;
                var selectedLanguage = languageList.Find(i => i.Id == languageCode);
                var pathEditFile = "";

                if (selectedLanguage.Name.ToLower() == "english")
                {
                    pathEditFile = filePathEnglish;
                }
                else
                {
                    // Set the non-English file path
                    if (selectedLanguage.Name.ToLower() == "francais")
                    { 
                        var temp = filePathEnglish.Replace("\\EN\\", "\\FR\\");
                        pathEditFile = temp.Replace("\\help-en\\", "\\help-fr\\");
                    }
                    else if (selectedLanguage.Name.ToLower() == "espanol")
                    {
                        var temp = filePathEnglish.Replace("\\EN\\", "\\ES\\");
                        pathEditFile = temp.Replace("\\help-en\\", "\\help-es\\");
                    }
                }
 
                var htmlTitle = "";
                var htmlTitleEnglish = "";
                var bodyContent = "";
                var bodyContentEnglish = "";

                if (System.IO.File.Exists(HttpUtility.UrlDecode(pathEditFile)))
                {
                    bodyContent = System.IO.File.ReadAllText(pathEditFile);
                    htmlTitle = HtmlFileHelper.GetTitle(bodyContent);
                    // Only load the English read only content if necessary
                    if (selectedLanguage.Name.ToLower() != "english")
                    {
                        bodyContentEnglish = System.IO.File.ReadAllText(filePathEnglish);
                        htmlTitleEnglish = HtmlFileHelper.GetTitle(bodyContentEnglish);
                    }
                }
                else
                {
                    bodyContent = "Note: Spanish is not ready yet. ERROR: Multilingual file does not exist: " + pathEditFile;
                }
                
                return Json
                    (
                        new
                        {
                            BodyContent = bodyContent,
                            BodyContentEnglish = bodyContentEnglish,
                            Path = pathEditFile,
                            PathEnglish = filePathEnglish,
                            HtmlTitle = htmlTitle,
                            HtmlTitleEnglish = htmlTitleEnglish,
                        }
                    , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var log = new LogFile();
                log.LogLine("Home Controller GetFileContent Error: ", "Message: " + ex.Message + "Source: " + ex.Source);

                return Json
                    (
                        new
                        {
                            BodyContent = "Error loading file content: " + filePathEnglish,
                        }
                    , JsonRequestBehavior.AllowGet);
            }
        }

 
        [HttpPost]
        public ActionResult SaveFileContent(string filePath, string content, string title)
        {
            try
            {
                // Fix up file header -- custom for wyzz editor control issues
                var divider = "css\">";
                int index = content.IndexOf(divider);
                var fixedContent = content.Remove(0, index + divider.Length);
                char[] arr = new char[] { '\t', ',', ' ', '\n' };
                fixedContent = fixedContent.TrimStart(arr);
                fixedContent = fixedContent.TrimEnd(arr);

                fixedContent = "<html>\n\t<head>\n\t\t<title>" + title + "</title>\n\t\t<link rel=\"stylesheet\" type=\"text/css\" href=\"/AQUARIUS/help-en/include/templates/wwhelp.css\"/>\n\t</head>\n\t<body>\n" + fixedContent;

                // Fix up file footer
                fixedContent += "\n\t</body>\n</html>";

                // Write a temp version of the old file. Using create to overwrite any previous temp files
                System.IO.File.Create(filePath + "_temp").Close();
                using (var sw = new System.IO.StreamWriter(filePath + "_temp"))
                {
                    sw.WriteLine(fixedContent);
                }

                // TODO don't need the backup anymore
                // Backup the old file and replace it with the temp file
                //var backupFilePath = filePath + "_backup_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".backup";
                //System.IO.File.Replace(filePath + "_temp", filePath, backupFilePath);             

                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                System.IO.File.Move(filePath + "_temp", filePath);
                if (System.IO.File.Exists(filePath + "_temp")) System.IO.File.Delete(filePath + "_temp");

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
                var log = new LogFile();
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

        // Begin JSTree (Controller code courtesy desalbres: http://www.codeproject.com/Articles/176166/Simple-FileManager-width-MVC-3-and-jsTree)
        [HttpPost]
        public ActionResult GetTreeData()
        {
            try
            {
                var dataPath = ConfigurationManager.AppSettings["EditFilePath"]; 
                var rootNode = new JsTreeModel();
                var serverPath = Server.MapPath(dataPath);
                rootNode.attr = new JsTreeAttribute();
                rootNode.data = "Error: No files found";

                if (!Directory.Exists(serverPath))
                {
                    var log = new LogFile();
                    log.LogLine("Home Controller GetTreeData Error", "Message: Root directory not found at: " + serverPath);

                    return Json(rootNode);
                }

                if (AlreadyPopulated == false)
                {
                    rootNode.data = "Help";
                    var rootPath = Request.MapPath(dataPath);  
                    rootNode.attr.id = rootPath;
                    PopulateTree(rootPath, rootNode);
                    AlreadyPopulated = true;
                    return Json(rootNode);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return Json
                (
                    new
                    {
                        Result = "Error retrieving file tree: " + ex.ToString(),
                    }
                );
            }
        }

        /// <summary>
        /// A method to populate a TreeView with directories, subdirectories, and files
        /// </summary>
        /// <param name="dir">The path of the directory</param>
        /// <param name="node">The "master" node, to populate</param>
        public void PopulateTree(string dir, JsTreeModel node)
        {
            if (node.children == null)
            {
                node.children = new List<JsTreeModel>();
            }
            // get the information of the directory
            var directory = new DirectoryInfo(dir);
            // loop through each subdirectory
            foreach (DirectoryInfo d in directory.GetDirectories())
            {
                // create a new node
                JsTreeModel t = new JsTreeModel();
                t.attr = new JsTreeAttribute();
                t.attr.id = d.FullName;
                t.data = d.Name.ToString();
                // populate the new node recursively
                PopulateTree(d.FullName, t);
                node.children.Add(t); // add the node to the "master" node
            }
            // loop through each file in the directory, and add these as nodes
            foreach (FileInfo f in directory.GetFiles("*.htm"))
            {
                // create a new node
                var t = new JsTreeModel();
                t.attr = new JsTreeAttribute();
                t.attr.id = f.FullName;
                t.data = f.Name.ToString();
                // add it to the "master"
                node.children.Add(t);
            }
        }

        // Don't load the jsTree treeview again if it has already been populated.
        // TODO this causes a bug where the tree won't repaint on browser refresh
        public bool AlreadyPopulated
        {
            get
            {
                return (Session["AlreadyPopulated"] == null ? false : (bool)Session["AlreadyPopulated"]); //false); //
            }
            set
            {
                Session["AlreadyPopulated"] = (bool)value;
            }

        }
        // End JSTree

        // Diable caching
        public class NoCacheAttribute : ActionFilterAttribute
        {
            public override void OnResultExecuting(ResultExecutingContext filterContext)
            {
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
                filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.HttpContext.Response.Cache.SetNoStore();

                base.OnResultExecuting(filterContext);
            }
        }

        //// Test code
        //[HttpPost]
        //public ActionResult TestAjax()
        //{
        //    return Json
        //           (
        //               new
        //               {
        //                   Result = "Successfully tested ajax",
        //               }
        //           );
        //}

    }
}