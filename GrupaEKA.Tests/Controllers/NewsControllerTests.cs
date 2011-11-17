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
    public class NewsControllerTests
    {
        private NewsController controller;

        [SetUp]
        public void LectureControllerTestSetUp()
        {
            controller = new NewsController(new MockGrupaEkaDB());
            var routeData = new RouteData();
            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            FormCollection formParameters = new FormCollection();
            ControllerContext controllerContext =
            MockRepository.GenerateStub<ControllerContext>(httpContext, routeData, controller);
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
            for(int i=0; i<7; ++i)
                controller.db.NewsCategories.Add(new NewsCategory{ID=1, Name="cat"+(i+1)});
            ViewResult result = controller.Create() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(controller.ViewBag.Now);
            Assert.NotNull(controller.ViewBag.NewsCategories);
            Assert.AreEqual(7,controller.ViewBag.NewsCategories.Count());
        }

        [Test]
        public void DetailsExistsTest()
        {
            // Act
            controller.db.News.Add(new News { ID = 1, Date = DateTime.Now, Author = "jj09", Text = "txt", Title = "title" });
            ViewResult result = controller.Details(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void DetailsNotExistsTest()
        {
            // Act
            RedirectToRouteResult result = controller.Details(1) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void CategoryIndexTest()
        {
            // Act
            ViewResult result = controller.CategoryIndex() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void CategoryCreateTest()
        {
            // Act
            ViewResult result = controller.CategoryCreate() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void CategoryCreatePostTest()
        {
            // Act
            NewsCategory cat = new NewsCategory { ID = 1, Name = "cat" };
            RedirectToRouteResult result = controller.CategoryCreate(cat) as RedirectToRouteResult;
            RedirectToRouteResult result2 = controller.CategoryCreate(cat) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("CategoryIndex", result.RouteValues["action"]);
            Assert.AreEqual("CategoryIndex", result2.RouteValues["action"]);
        }

        [Test]
        public void CategoryDetailsExistsTest()
        {
            // Act
            controller.db.NewsCategories.Add(new NewsCategory { ID = 1, Name = "cat" });
            ViewResult result = controller.CategoryDetails(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void CategoryDetailsNotExistsTest()
        {
            // Act
            RedirectToRouteResult result = controller.CategoryDetails(1) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("CategoryIndex", result.RouteValues["action"]);
        }

        [Test]
        public void CategoryEditExistsTest()
        {
            // Act
            controller.db.NewsCategories.Add(new NewsCategory { ID = 1, Name = "cat" });
            ViewResult result = controller.CategoryEdit(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void CategoryEditNotExistsTest()
        {
            // Act
            RedirectToRouteResult result = controller.CategoryEdit(1) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("CategoryIndex", result.RouteValues["action"]);
        }

        [Test]
        public void CategoryEditPostTest()
        {
            // Act
            string catName1 = "cat";
            NewsCategory cat = new NewsCategory { ID = 1, Name = catName1 };
            RedirectToRouteResult result = controller.CategoryCreate(cat) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("CategoryIndex", result.RouteValues["action"]);
            Assert.AreEqual(cat.Name, controller.db.NewsCategories.Find(1).Name);

            // Act
            cat.Name = "cat2";
            RedirectToRouteResult result2 = controller.CategoryEdit(cat) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("CategoryIndex", result2.RouteValues["action"]);
            Assert.AreEqual(cat.Name, controller.db.NewsCategories.Find(1).Name);
        }

        [Test]
        public void CategoryDeleteExistsTest()
        {
            // Act
            controller.db.NewsCategories.Add(new NewsCategory { ID = 1, Name = "cat" });
            ViewResult result = controller.CategoryDelete(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void CategoryDeleteNotExistsTest()
        {
            // Act
            RedirectToRouteResult result = controller.CategoryDelete(1) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("CategoryIndex", result.RouteValues["action"]);
        }

        [Test]
        public void CategoryDeletePostTest()
        {
            // Act
            controller.db.NewsCategories.Add(new NewsCategory { ID = 1, Name = "cat" });
            RedirectToRouteResult result = controller.CategoryDelete(1,new FormCollection()) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("CategoryIndex", result.RouteValues["action"]);
            Assert.AreEqual(true, ((MockGrupaEkaDB)controller.db).saved);
        }
    }
}
