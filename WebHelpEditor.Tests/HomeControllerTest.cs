using System;
using System.Web.UI.WebControls.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Web;
using System.Web.Mvc;
using WebHelpEditor.Controllers;
using Moq;

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
        private const string FilePath = @"S:\GitHub\WebHelpEditor\WebHelpEditor.Tests\TestFiles\testDoNotEdit.htm";

        private const string FileContent =
            @"<html><head><title>Test content for file editor</title><link rel=""stylesheet"" type=""text/css"" href=""/AQUARIUS/help-en/include/templates/wwhelp.css"" /></head><body><p>This is an HTML test</p></body></html>";

        [TestMethod]
        public void GetFileContentTest()
        {
            var home = new HomeController();
            JsonResult result = (JsonResult) home.GetFileContent(FilePath);

            // TODO validate result against expected content. Add actual testable content that cant' be edited
            string expected = "{ BodyContent = <html";
            string resultStart = result.Data.ToString().Substring(0, 21);

            // Assert
            Assert.AreEqual(expected, resultStart);
        }

        [TestMethod]
        public void SaveFileContentTest()
        {
            // Format for file path string passed to Home "S%3a%5cSource%5cWebHelpEditor%5cWebHelpEditor%5cTest%5cSaveFileTest"
            // maps to: "S:\Source\WebHelpEditor\WebHelpEditor\Test\SaveFileTest";

            var home = new HomeController();
            home.SaveFileContent("\\TestFiles", FileContent, "TestFileWrite");

            // Assert
            // todo check if file is there
            //Assert.AreEqual(expected, resultStart);
        }

        [TestMethod]
        public void GetTreeDataTest()
        {
            // Arrange
            var session = new MockHttpSession();
            var context = new Mock<ControllerContext>();
            context.Setup(m => m.HttpContext.Session).Returns(session);
            //var files = new Mock<HttpFileCollectionBase>();
            var request = new Mock<HttpRequestBase>(); 
            //request.Setup(req => req.Files).Returns(files.Object);
            request.Setup(req => req.ApplicationPath).Returns("~/"); 
            var response = new Mock<HttpResponseBase>(); 
            response.Setup(res => res.ApplyAppPathModifier(It.IsAny<string>())).
                Returns((string virtualPath) => virtualPath);

            string resultTest;
            var home = new HomeController();
            home.ControllerContext = context.Object;
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

