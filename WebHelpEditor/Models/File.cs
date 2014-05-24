using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using WebHelpEditor.Helper;

namespace WebHelpEditor.Models
{
    public class FileToEdit
    {
        public string FilePath { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string LoadLink { get; set; }
   
        public FileToEdit(string filePath)
        {
                this.FilePath = filePath;
                this.Name = Path.GetFileName(filePath);
                this.Content = File.ReadAllText(filePath);
        }

        public static IList<FileToEdit> GetFiles(string path)
        {
            try
            {
                IList<FileToEdit> fileList = new List<FileToEdit>();
                
                foreach(string filePath in Directory.GetFiles(path, "*.htm"))
                {
                    FileToEdit temp = new FileToEdit(filePath);
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