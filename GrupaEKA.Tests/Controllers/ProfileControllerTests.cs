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
    public class ProfileControllerTests
    {
        private ProfileController controller;

        [SetUp]
        public void LectureControllerTestSetUp()
        {
            controller = new ProfileController(new MockGrupaEkaDB());
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
        public void EditExistsTest()
        {
            // Act
            controller.db.Profiles.Add(new Profile { ID = 1, UserName = "jj09" });
            ViewResult result = controller.Edit(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void EditNotExistsTest()
        {
            // Act
            RedirectToRouteResult result = controller.Edit(1) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void EditPostTest()
        {
            // Act
            ProfileViewModel profilevm = new ProfileViewModel { Profile = new Profile { ID = 1, UserName = "jj09", LastName = "Doe" } };
            controller.db.Profiles.Add(profilevm.Profile);
            profilevm.Profile.LastName = "Smith";
            RedirectToRouteResult result = controller.Edit(profilevm) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", result.RouteValues["action"]);
            Assert.AreEqual(profilevm.Profile.LastName, controller.db.Profiles.Find(1).LastName);
        }
    }
}
