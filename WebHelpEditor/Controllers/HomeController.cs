using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using WebHelpEditor.Helper;
using WebHelpEditor.Models;

namespace WebHelpEditor.Controllers
{
    public class HomeController : Controller
    {
        public static string dataPath = "/AQUARIUS/help-fr/help/8WSCToolboxes";
        
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            Session["AlreadyPopulated"] = false;
            return View();
        }

        public ActionResult Test(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            Session["AlreadyPopulated"] = false;
            return View();
        }

        [NoCache]
        [HttpGet]
        public ActionResult GetFileContent(string filePath)
        {
            try
            {
                string path = HttpUtility.UrlDecode(filePath);

                if (!System.IO.File.Exists(path))
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

                string bodyContent = "";
                string bodyContentEnglish = "";
                string pathEnglish = path.Replace("help-fr", "help-en");

                // Format: C:\DWASFiles\Sites\fileeditor\VirtualDirectory0\site\wwwroot\\Test\help-fr\3UserAccessTabWSCLocMan.htm
                // Remove extra slash and make path shorter by splitting on "\wwwroot\\Test\"
                // Fix up file header -- custom for wyzz editor control issues
                //string divider = "\\\\Test\\";
                //int index = pathEnglish.IndexOf(divider);
                //string pathShort = path.Remove(0, index + divider.Length);
                //string pathEnglishShort = pathEnglish.Remove(0, index + divider.Length);
                string pathShort = path;
                string pathEnglishShort = pathEnglish;

                string htmlTitle = "";
                if (System.IO.File.Exists(path))
                {
                    bodyContent = System.IO.File.ReadAllText(path);
                    bodyContentEnglish = System.IO.File.ReadAllText(pathEnglish);
                    htmlTitle = HtmlFileHelper.GetTitle(bodyContent);
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
                            Path = pathShort,
                            PathEnglish = pathEnglishShort,
                            HtmlTitle = htmlTitle,
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
                            BodyContent = "Error loading file content: " + filePath,
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
                string divider = "css\">";
                int index = content.IndexOf(divider);
                string fixedContent = content.Remove(0, index + divider.Length);
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

                // Backup the old file and replace it with the temp file
                var backupFilePath = filePath + "_backup_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".backup";
                System.IO.File.Replace(filePath + "_temp", filePath, backupFilePath);

                // TODO temp
                //string deployment = "debug";

                // Write a backup to an FTP server
                // Convert the file path to an FTP path
                #if DEBUG
                    divider = "\\WebHelpEditor\\WebHelpEditor\\"; //local test version
                #else
                    divider = "\\wwwroot\\"; //deployed version
                    //deployment = "production";
                #endif

                index = filePath.IndexOf(divider);
                string ftpPath = filePath.Remove(0, index + divider.Length);
                string ftpPathBackup = backupFilePath.Remove(0, index + divider.Length);
                // TODO probably don't need the first replace
                //ftpPath.Replace("\\\\", "/");
                //ftpPath.Replace("\\", "/");
                
                // TODO temp debuging
                //var log = new LogFile();
                //log.LogLine("FTP debug: ", "FilePath: " + filePath + " FTPPath: " + ftpPath + " Deployment: " + deployment);

                // TODO temporarily backup the files to an FTP server b/c deploy is currently erasing all files on target
                FtpHelper.Upload(filePath, ftpPath);
                FtpHelper.Upload(backupFilePath, ftpPathBackup);

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
            if (AlreadyPopulated == false)
            {
                JsTreeModel rootNode = new JsTreeModel();
                rootNode.attr = new JsTreeAttribute();
                rootNode.data = "Root";
                string rootPath = Request.MapPath(dataPath);
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
            DirectoryInfo directory = new DirectoryInfo(dir);
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
                JsTreeModel t = new JsTreeModel();
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