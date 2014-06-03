using System;
using System.Web.Routing;
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
            var session = new Mock<HttpSessionStateBase>();
            var context = new Mock<HttpContextBase>(MockBehavior.Strict); 
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);

            request.Setup(x => x.ApplicationPath).Returns("/");
            request.Setup(x => x.MapPath(It.IsAny<string>())).Returns<string>(path => @"S:\GitHub\WebHelpEditor\WebHelpEditor.Tests\TestFiles\AQUARIUS\help-en\help");
            request.Setup(x => x.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());
            context.SetupGet(m => m.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            
            string resultTest;
            var home = new HomeController();
            home.ControllerContext = new ControllerContext(context.Object, new RouteData(), home);
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

