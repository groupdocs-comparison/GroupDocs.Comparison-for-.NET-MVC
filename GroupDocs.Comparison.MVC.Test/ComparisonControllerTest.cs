using GroupDocs.Comparison.MVC.Controllers;
using NUnit.Framework;
using System.Web.Routing;
using MvcContrib.TestHelper;
using Huygens;
using System;

namespace GroupDocs.Comparison.MVC.Test
{
    [TestFixture]
    public class ComparisonControllerTest
    {
        
        [SetUp]
        public void TestInitialize()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        [TearDown]
        public void TearDown()
        {
            RouteTable.Routes.Clear();
        }

        [Test]
        public void ViewStatusTest()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "/../../../src";            
            using (var server = new DirectServer(path))
            {
                var request = new SerialisableRequest
                {
                    Method = "GET",
                    RequestUri = "/comparison",
                    Content = null
                };

                var result = server.DirectCall(request);
                Assert.That(result.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public void ViewMapControllerTest()
        {
            "~/comparison".Route().ShouldMapTo<ComparisonController>(x => x.Index());
        }
    }
}
