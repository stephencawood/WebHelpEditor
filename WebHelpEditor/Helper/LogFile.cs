using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;

namespace WebHelpEditor.Helper
{
    class LogFile
    {
        private string fileName;

        public LogFile()
        {
            fileName = HttpRuntime.AppDomainAppPath + @"\Log.txt";
        }

        public LogFile(string fileName)
        {
            this.fileName = fileName;
        }
        /// <summary>
        /// LogLine method is used to write details to a log
        /// </summary>
        public void LogLine(string category, string message)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(fileName, FileMode.Append)))
            {

                writer.WriteLine("{0} {1} {2}", DateTime.Now.ToString(), category, message);
                writer.Close();
            }
        }

    }
}
