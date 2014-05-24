using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Net;
using System.Text;

namespace WebHelpEditor.Helper
{
    public class FtpHelper
    {

        public static void Upload(string serverPath, string ftpPath)
        {
            // TODO temp
            //string fileName = "test.txt";
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://ftp.robincawood.com" + @"/" + ftpPath);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("w00110453", "pass@word1");
            
            // Copy the contents of the file to the request stream.
            StreamReader sourceStream = new StreamReader(serverPath);

            byte [] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
    
            Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
    
            response.Close();
        }
    }

}