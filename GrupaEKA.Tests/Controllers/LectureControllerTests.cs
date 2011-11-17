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
    public class LectureControllerTests
    {
        private LectureController controller;

        [SetUp]
        public void LectureControllerTestSetUp()
        {
            controller = new LectureController(new MockGrupaEkaDB());
            var routeData = new RouteData();
            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            FormCollection formParameters = new FormCollection();
            ControllerContext controllerContext =
            MockRepository.GenerateStub<ControllerContext>(httpContext,routeData,controller);
            controller.ControllerContext = controllerContext;
            controller.ValueProvider = formParameters.ToValueProvider();
        }

        [Test]
        public void IndexTest()
        {
            // Act
            ViewResult result = controller.Index(-99) as ViewResult;

            // Assert
            Assert.AreEqual(1, result.ViewBag.CurrentPage);

            // Act
            ViewResult result2 = controller.Index(99) as ViewResult;

            // Assert
            Assert.AreEqual(1, result2.ViewBag.CurrentPage);
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
            LectureTimeViewModel newLectureTime = new LectureTimeViewModel()
            {
                Hour = "5",
                Minutes = "23",
                Lecture = new Lecture()
                {
                    ID = 1,
                    Author = "author",
                    Date = new DateTime(2011, 10, 10),
                    Title = "New lecture",
                    Text = "New lecture content."
                }
            };
            
            RedirectToRouteResult result = controller.Create(newLectureTime) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Lecture", result.RouteValues["controller"]);
            Assert.AreEqual(true, ((MockGrupaEkaDB)controller.db).saved);
            Assert.AreEqual(new DateTime(2011, 10, 10, 5, 23, 0), controller.db.Lectures.First().Date);
            Assert.AreEqual(newLectureTime.Lecture.Author, controller.db.Lectures.First().Author);
            Assert.AreEqual(newLectureTime.Lecture.Title, controller.db.Lectures.First().Title);
            Assert.AreEqual(newLectureTime.Lecture.Text, controller.db.Lectures.First().Text);
        }

        [Test]
        public void DetailsExistsTest()
        {
            // Act
            controller.db.Lectures.Add(new Lecture { ID = 1, Date = DateTime.Now, Author = "jj09", Text = "txt", Title = "title" });
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
            Assert.AreEqual("Lecture", result.RouteValues["controller"]);
        }
    }
}
