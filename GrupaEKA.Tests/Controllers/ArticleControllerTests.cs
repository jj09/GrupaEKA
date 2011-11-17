using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;
using GrupaEka;
using GrupaEka.Controllers;
using GrupaEka.Models;
using GrupaEka.Tests.Models;
using Rhino.Mocks;
using System.Web;
using System.Web.Routing;

namespace GrupaEka.Tests.Controllers
{
    [TestFixture]
    public class ArticleControllerTests
    {
        private ArticleController controller;

        [SetUp]
        public void ArticleControllerTestSetUp()
        {
            controller = new ArticleController(new MockGrupaEkaDB());

            var routeData = new RouteData();
            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            FormCollection formParameters = new FormCollection();
            ControllerContext controllerContext =
            MockRepository.GenerateStub<ControllerContext>(httpContext,
                                                                routeData,
                                                                controller);
            controller.ControllerContext = controllerContext;
            controller.ValueProvider = formParameters.ToValueProvider();
        }

        [Test]
        public void IndexTest()
        {
            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void CreateTest()
        {
            // Act
            ViewResult result = controller.Create() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void CreatePostTest()
        {
            // Act
            ArticleTimeViewModel newArticleTime = new ArticleTimeViewModel()
            {
                Hour = "5",
                Minutes = "23",
                Article = new Article() 
                {
                    ID = 1,
                    Author = "author", 
                    Date = new DateTime(2011,10,10), 
                    Title = "New article",
                    Text = "New article content."
                }
            };
            RedirectToRouteResult result = controller.Create(newArticleTime) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Article", result.RouteValues["controller"]);
            Assert.AreEqual(true, ((MockGrupaEkaDB)controller.db).saved);
            Assert.AreEqual(new DateTime(2011,10,10,5,23,0), controller.db.Articles.First().Date);
            Assert.AreEqual(newArticleTime.Article.Author, controller.db.Articles.First().Author);
            Assert.AreEqual(newArticleTime.Article.Title, controller.db.Articles.First().Title);
            Assert.AreEqual(newArticleTime.Article.Text, controller.db.Articles.First().Text);
        }

        [Test]
        public void DetailsExistsTest()
        {
            // Act
            ArticleTimeViewModel newArticleTime = new ArticleTimeViewModel()
            {
                Hour = "5",
                Minutes = "23",
                Article = new Article()
                {
                    ID = 1,
                    Author = "author",
                    Date = new DateTime(2011, 10, 10),
                    Title = "New article",
                    Text = "New article content."
                }
            };
            RedirectToRouteResult result = controller.Create(newArticleTime) as RedirectToRouteResult;
            ViewResult result2 = controller.Details(1) as ViewResult;

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Article", result.RouteValues["controller"]);
            Assert.NotNull(result2);
        }

        [Test]
        public void DetailsNotExistsTest()
        {
            // Act
            RedirectToRouteResult result = controller.Details(1) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Article", result.RouteValues["controller"]);
        }
    }
}
