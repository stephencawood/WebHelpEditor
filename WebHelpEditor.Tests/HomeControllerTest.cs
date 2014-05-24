using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Web.Mvc;
//using System.Collections;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using WebHelpEditor.Controllers;

// Example asserts
//Assert.That(result["Status"], Is.True);
//Assert.AreEqual(pathEnglish, result.Data);
//.AssertResultIs<JsonResult>()
//var jsonResult = yourController.YourAction(params);
//var js = new JavaScriptSerializer();
//var deserializedTarget = (object[])js.DeserializeObject(result.Data.ToString());
//var pathEnglish = deserializedTarget["PathEnglish"].ToString();

//object parsedJson = JsonConvert.DeserializeObject(result.Data.ToString());
//string test = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);


namespace WebHelpEditor.Tests
{
    [TestClass]
    public class HomeControllerTest
    {
        // Format for file path string passed to Home "S%3a%5cSource%5cFileEditor%5cFileEditor%5cTest%5chelp-fr%5ctestDoNotEdit.htm"
        // maps to: "S:\Source\WebHelpEditor\WebHelpEditor\Test\help-fr\testDoNotEdit.htm";
        string htmFileEncodedPath = "S%3a%5cSource%5cFileEditor%5cFileEditor%5cTest%5chelp-fr%5ctestDoNotEdit.htm";
        string fileContent = @"<html><head><title>Test content for file editor</title><link rel=""stylesheet"" type=""text/css"" href=""/AQUARIUS/help-en/include/templates/wwhelp.css"" /></head><body><p>This is an HTML test</p></body></html>";
            
        [TestMethod]
        public void GetFileContentTest()
        {
            var home = new HomeController();
            JsonResult result = (JsonResult)home.GetFileContent(htmFileEncodedPath);

            // TODO validate result against expected content. Add actual testable content that cant' be edited
            string expected = "{ BodyContent = <html";
            string resultStart = result.Data.ToString().Substring(0, 21);

            // Assert
            Assert.AreEqual(expected, resultStart);
        }

        [TestMethod]
        public void SaveFileContentTest()
        {
            // Format for file path string passed to Home "S%3a%5cSource%5cFileEditor%5cFileEditor%5cTest%5cSaveFileTest"
            // maps to: "S:\Source\WebHelpEditor\WebHelpEditor\Test\SaveFileTest";

            var home = new HomeController();
            home.SaveFileContent("%5cTestOutput", fileContent, "TestFileWrite");

            // Assert
            // todo check if file is there
            //Assert.AreEqual(expected, resultStart);
        }

        [TestMethod]
        public void GetTreeDataTest()
        {
            var home = new HomeController();
            //JsonResult result = (JsonResult)home.GetTreeData();
        }
    }
}
