using System.IO;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebHelpEditor.Controllers;

namespace WebHelpEditor.Tests
{
    [TestClass]
    public class HomeControllerTest
    {
        private const string RootPath = @"S:\GitHub\WebHelpEditor\WebHelpEditor.Tests\TestFiles\";
        private const string FileContent = @"<html><head><title>Test content for file editor</title><link rel=""stylesheet"" type=""text/css"" href=""/AQUARIUS/help-en/include/templates/wwhelp.css"" /></head><body><p>This is an HTML test</p></body></html>";

        [TestMethod]
        public void GetFileContentTest()
        {
            // Arrange
            var home = new HomeController();
            FakeHttpContextHelper.SetFakeControllerContext(home);

            // Act
            var result = (JsonResult)home.GetFileContent(RootPath + "\\testDoNotEdit.htm", "1");

            // Assert
            const string expected = "{ BodyContent = <html";
            var resultStart = result.Data.ToString().Substring(0, 21);
            Assert.AreEqual(expected, resultStart);
        }

        [TestMethod]
        public void GetFileContentMultilingualTest()
        {
            // Arrange
            var home = new HomeController();
            FakeHttpContextHelper.SetFakeControllerContext(home);

            // Act
            var result = (JsonResult)home.GetFileContent(RootPath + "\\testDoNotEdit.htm", "2");

            // Assert
            const string expected = "{ BodyContent = <html";
            var resultStart = result.Data.ToString().Substring(0, 21);
            Assert.AreEqual(expected, resultStart);
        }

        [TestMethod]
        public void SaveFileContentTest()
        {
            // Arrange
            const string fileName = "TestFileName.htm";
            var home = new HomeController();
            FakeHttpContextHelper.SetFakeControllerContext(home);
            home.Session["AlreadyPopulated"] = false;
            File.Create(RootPath + fileName).Close();
            
            // Act
            home.SaveFileContent(RootPath + fileName, "1", FileContent, "TestHtmlTitle");

            // Assert
            Assert.IsTrue(File.Exists(RootPath + fileName));

            // Cleanup
            File.Delete(RootPath + fileName);
        }

        [TestMethod]
        public void GetTreeDataTest()
        {
            // Arrange
            var home = new HomeController();
            FakeHttpContextHelper.SetFakeControllerContext(home); 
            home.Session["AlreadyPopulated"] = false;

            // Act
            var result = (JsonResult) home.GetTreeData();

            // Assert
            Assert.IsFalse(result.Data.ToString().Contains("Error retrieving file tree:"));
        }

        public class MockHttpSession : System.Web.HttpSessionStateBase
        {
            readonly System.Collections.Generic.Dictionary<string, object> _sessionDictionary = new System.Collections.Generic.Dictionary<string, object>();
            public override object this[string name]
            {
                get { return _sessionDictionary[name]; }
                set { _sessionDictionary[name] = value; }
            }
        }

}
}

