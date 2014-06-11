using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebHelpEditor.Controllers;
using System.Web.Mvc;

namespace WebHelpEditor.Tests
{
    class ViewTest
    {
        [TestClass]
        public class HomeViewsTest
        {
            [TestMethod]
            public void IndexTest()
            {
                // Arrange

                //var view = new controller.Index();
                //view.ViewBag.Message = "Testing";

                var controller = new HomeController();
                FakeHttpContextHelper.SetFakeControllerContext(controller);
                controller.Session["AlreadyPopulated"] = false;

                // Act
                var result = controller.Index("www.test.com") as ContentResult;

                //HtmlDocument doc = view.RenderAsHtml();
                //HtmlNode node = doc.DocumentNode.Element("h2");
                
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("Hello World!.", result.Content);

                //Assert.AreEqual("Testing", node.InnerHtml.Trim());
            }
        }
    }
}
