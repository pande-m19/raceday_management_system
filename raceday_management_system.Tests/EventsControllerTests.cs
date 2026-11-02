using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using raceday_management_system.Controllers;
using raceday_management_system.Database.Interfaces;
using raceday_management_system.Services;
using Xunit;

namespace raceday_management_system.Tests
{//start of namespace
    public class EventsControllerTests
    {//start of class

        // Helper method to instantiate the controller with the required dependencies
        private events create_controller(Mock<Ievents_connection> mock_db)
        {
            // Setup the mock configuration so the blob service doesn't crash
            var mock_config = new Mock<IConfiguration>();
            mock_config.Setup(c => c["AzureStorage:ConnectionString"]).Returns("DefaultEndpointsProtocol=https;AccountName=dummy;AccountKey=dummy;EndpointSuffix=core.windows.net");
            mock_config.Setup(c => c["AzureStorage:ContainerName"]).Returns("dummy-container");

            var blob_service = new blob_storage_service(mock_config.Object);
            return new events(mock_db.Object, blob_service);
        }

        //Participant cannot create event (403)
        [Fact]
        public void participant_cannot_create_event()
        {//start of test
            //arrange
            var mock_db = new Mock<Ievents_connection>();
            var controller = create_controller(mock_db);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Participant", 5)
            };

            //act
            var result = controller.create_event(
                "Test", "Desc", DateTime.UtcNow.AddDays(30),
                "Cape Town", 42.2m, "Run");

            //assert
            var status_result = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, status_result.StatusCode);
        }//end of test

        //Organiser can create event (201)
        [Fact]
        public void organiser_can_create_event()
        {//start of test
            //arrange
            var mock_db = new Mock<Ievents_connection>();

            //Return an int (dummy ID) instead of a string
            mock_db.Setup(m => m.create_event(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<decimal>(),
                It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<decimal?>()))
                .Returns(1);

            var controller = create_controller(mock_db);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Organiser", 1)
            };

            //act
            var result = controller.create_event(
                "Test Event", "Desc", DateTime.UtcNow.AddDays(30),
                "Cape Town", 42.2m, "Run");

            //assert
            var status_result = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, status_result.StatusCode);
        }//end of test

        //Unauthenticated user cannot view events (401)
        [Fact]
        public void unauthenticated_cannot_view_events()
        {//start of test
            //arrange
            var mock_db = new Mock<Ievents_connection>();
            var controller = create_controller(mock_db);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context()
            };

            //act
            var result = controller.get_all_events();

            //assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }//end of test

        //Invalid event type returns 400 Bad Request
        [Fact]
        public void invalid_event_type_returns_400()
        {//start of test
            //arrange
            var mock_db = new Mock<Ievents_connection>();
            var controller = create_controller(mock_db);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Organiser", 1)
            };

            //act - invalid type "Swim"
            var result = controller.create_event(
                "Test Event", "Desc", DateTime.UtcNow.AddDays(30),
                "Cape Town", 42.2m, "Swim");

            //assert
            Assert.IsType<BadRequestObjectResult>(result);
        }//end of test

    }//end of class

}//end of namespace