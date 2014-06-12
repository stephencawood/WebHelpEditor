using System.IO;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebHelpEditor.Controllers;

namespace WebHelpEditor.Tests
{
    [TestClass]
    public class HomeControllerTest
    {
        private const string FilePath = @"S:\GitHub\WebHelpEditor\WebHelpEditor.Tests\TestFiles\testDoNotEdit.htm";

        private const string FileContent =
            @"<html><head><title>Test content for file editor</title><link rel=""stylesheet"" type=""text/css"" href=""/AQUARIUS/help-en/include/templates/wwhelp.css"" /></head><body><p>This is an HTML test</p></body></html>";

        [TestMethod]
        public void GetFileContentTest()
        {
            var home = new HomeController();
            JsonResult result = (JsonResult) home.GetFileContent(FilePath);

            // TODO validate result against expected content. Add actual testable content that cant' be edited
            const string expected = "{ BodyContent = <html";
            var resultStart = result.Data.ToString().Substring(0, 21);

            // Assert
            Assert.AreEqual(expected, resultStart);
        }

        [TestMethod]
        public void SaveFileContentTest()
        {
            // Arrange
            var home = new HomeController();
            FakeHttpContextHelper.SetFakeControllerContext(home);
            home.Session["AlreadyPopulated"] = false;
            
            // Act
            home.SaveFileContent(@"S:\GitHub\WebHelpEditor\WebHelpEditor.Tests\TestFiles\TestFileWrite", FileContent, "TestHtmlTitle");

            // Assert
            // todo check if file is there
            Assert.IsTrue(File.Exists(@"S:\GitHub\WebHelpEditor\WebHelpEditor.Tests\TestFiles\TestFileWrite"));

            // Cleanup
            //File.Delete("\\TestFiles\\TestFileWrite");

        }

        [TestMethod]
        public void GetTreeDataTest()
        {
            // Arrange
            var home = new HomeController();
            FakeHttpContextHelper.SetFakeControllerContext(home); 
            home.Session["AlreadyPopulated"] = false;

            // Act
            JsonResult result = (JsonResult) home.GetTreeData();

            // Assert
            Assert.IsFalse(result.Data.ToString().Contains("Error retrieving file tree:"));
        }

        public class MockHttpSession : System.Web.HttpSessionStateBase
        {
            System.Collections.Generic.Dictionary<string, object> _sessionDictionary = new System.Collections.Generic.Dictionary<string, object>();
            public override object this[string name]
            {
                get { return _sessionDictionary[name]; }
                set { _sessionDictionary[name] = value; }
            }
        }

}
}

