using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using WebHelpEditor.Helper;

namespace WebHelpEditor.Models
{
    public class File
    {
        public string FilePath { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string LoadLink { get; set; }
   
        public File(string filePath)
        {
                this.FilePath = filePath;
                this.Name = Path.GetFileName(filePath);
                this.Content = System.IO.File.ReadAllText(filePath);
        }

        public static IList<File> GetFiles(string path)
        {
            try
            {
                IList<File> fileList = new List<File>();
                
                foreach(string filePath in Directory.GetFiles(path, "*.htm"))
                {
                    File temp = new File(filePath);
                    fileList.Add(temp);
                }

                return fileList;
            }
            catch (Exception ex)
            {
                LogFile log = new LogFile();
                log.LogLine(" Exception Caught", " GetFiles: " + ex.Message + "Source: " + ex.Source);
                return null;
            }
        }
    }

}