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

        [TestFixtureSetUp]
        public void LectureControllerTestSetUp()
        {
            controller = new LectureController(new MockGrupaEkaDB());

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


    }
}
