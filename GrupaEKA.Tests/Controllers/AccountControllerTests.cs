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
    public class AccountControllerTests
    {
        private AccountController controller;

        [TestFixtureSetUp]
        public void AccountControllerTestSetUp()
        {
            controller = new AccountController(new MockGrupaEkaDB());

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
        public void RegisterTest()
        {
            // Act
            ViewResult result = controller.Register() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void LogOnTest()
        {
            // Act
            ViewResult result = controller.LogOn() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void LogOnFastTest()
        {
            // Act
            PartialViewResult result = controller.LogOnFast() as PartialViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void ChangePasswordTest()
        {
            // Act
            ViewResult result = controller.ChangePassword() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void ChangePasswordSuccessTest()
        {
            // Act
            ViewResult result = controller.ChangePasswordSuccess() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void ResetPasswordTest()
        {
            // Act
            ViewResult result = controller.ResetPassword() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void ResetPasswordFailTest()
        {
            // Act
            ViewResult result = controller.ResetPasswordFail() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void ResetPasswordSuccessTest()
        {
            // Act
            ViewResult result = controller.ResetPasswordSuccess() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void ResetPasswordSentTest()
        {
            // Act
            ViewResult result = controller.ResetPasswordSent() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void ChangeEmailTest()
        {
            // Act
            ViewResult result = controller.ChangeEmail() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void ChangeEmailSuccessTest()
        {
            // Act
            ViewResult result = controller.ChangeEmailSuccess() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }
    }
}
