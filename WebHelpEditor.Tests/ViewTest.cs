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
                var home = new HomeController();
                FakeHttpContextHelper.SetFakeControllerContext(home);
                home.Session["AlreadyPopulated"] = false;

                // Act
                var result = home.Index("www.test.com") as ViewResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.ViewName);
            }
        }
    }
}
