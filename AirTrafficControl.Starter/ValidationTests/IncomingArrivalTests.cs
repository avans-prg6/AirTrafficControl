using AirTrafficControl.Web.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.ComponentModel.DataAnnotations;

namespace ValidationTests
{
    [TestClass]
    public class IncomingArrivalTests
    {
        [TestMethod]
        public void HandlerValidationShouldRaiseOnUnknownCallsign()
        {
            // 1. Arrange
            HandlerValidation toTest = new HandlerValidation();
            var target = "HatseflatsNotKnown";
            
            var serviceProvider = new Mock<IServiceProvider>();
            //serviceProvider
            //    .Setup(x => x.GetService(typeof(ATCContext)))
            //    .Returns(new ATCContext(Options, StoreOptions));

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            //var testContext = new ValidationContext(target,);
            //var sut = new ApiResourceRepository(serviceProvider.Object);


            //2. Act
            var result = toTest.IsValid("HatseflatsNotKnown");
        }
        [TestMethod]
        public void HandlerValidationShouldPassOnKnownCallsign()
        { }
        [TestMethod]
        public void FlightValidationShouldRaiseOnUnknownFlight()
        { }
        [TestMethod]
        public void FlightValidationShouldPassOnKnownFlight()
        { }
    }
}
