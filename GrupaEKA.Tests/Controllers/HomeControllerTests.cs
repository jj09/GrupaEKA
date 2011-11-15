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
    public class HomeControllerTests
    {
        private HomeController controller;

        [TestFixtureSetUp]
        public void HomeControllerTestSetUp()
        {
            controller = new HomeController(new MockGrupaEkaDB());

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
            ViewResult result = controller.Index("SomeCat",-99) as ViewResult;

            // Assert
            Assert.AreEqual(1, result.ViewBag.CurrentPage);
            Assert.AreEqual("SomeCat", result.ViewBag.Category);
        }

        [Test]
        public void AboutTest()
        {
            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void ProjectsTest()
        {
            // Act
            ViewResult result = controller.Projects() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void ProjectsEditTest()
        {
            // Act
            ViewResult result = controller.ProjectsEdit() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void ProjectsEditPostTest()
        {
            // Act
            string newContent = "Nowy opis naszych projektów";
            Projects projects = new Projects { ID = 1, Content = newContent };
            RedirectToRouteResult result = controller.ProjectsEdit(projects) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Projects", result.RouteValues["action"]);
            Assert.AreEqual(newContent, controller.db.Projects.First().Content);
        }

        [Test]
        public void OtherTest()
        {
            // Act
            ViewResult result = controller.Other() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }


    }
}
