using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using raceday_management_system.Controllers;
using raceday_management_system.Database.Interfaces;
using raceday_management_system.Services;
using Xunit;

namespace raceday_management_system.Tests
{//start of namespace
    public class RoleAccessTests
    {//start of class

        // Helper method to instantiate the events controller
        private events create_events_controller(Mock<Ievents_connection> mock_db)
        {
            var mock_config = new Mock<IConfiguration>();
            mock_config.Setup(c => c["AzureStorage:ConnectionString"]).Returns("DefaultEndpointsProtocol=https;AccountName=dummy;AccountKey=dummy;EndpointSuffix=core.windows.net");
            mock_config.Setup(c => c["AzureStorage:ContainerName"]).Returns("dummy-container");

            var blob_service = new blob_storage_service(mock_config.Object);
            return new events(mock_db.Object, blob_service);
        }

        //Organiser cannot access participant dashboard (403)
        [Fact]
        public void organiser_cannot_access_participant_dashboard()
        {//start of test
            //arrange
            var mock_db = new Mock<Idashboard_connection>();
            var controller = new dashboard(mock_db.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Organiser", 1)
            };

            //act
            var result = controller.get_participant_dashboard();

            //assert
            var status_result = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, status_result.StatusCode);
        }//end of test

        //Participant cannot access organiser dashboard (403)
        [Fact]
        public void participant_cannot_access_organiser_dashboard()
        {//start of test
            //arrange
            var mock_db = new Mock<Idashboard_connection>();
            var controller = new dashboard(mock_db.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Participant", 5)
            };

            //act
            var result = controller.get_organiser_dashboard();

            //assert
            var status_result = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, status_result.StatusCode);
        }//end of test

        //Unauthenticated user gets 401 on organiser dashboard
        [Fact]
        public void unauthenticated_gets_401_on_organiser_dashboard()
        {//start of test
            //arrange
            var mock_db = new Mock<Idashboard_connection>();
            var controller = new dashboard(mock_db.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context()
            };

            //act
            var result = controller.get_organiser_dashboard();

            //assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }//end of test

        //Participant cannot create event (403)
        [Fact]
        public void participant_cannot_create_event()
        {//start of test
            //arrange
            var mock_db = new Mock<Ievents_connection>();
            var controller = create_events_controller(mock_db);

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

        //Participant cannot access organiser events (403)
        [Fact]
        public void participant_cannot_access_organiser_events()
        {//start of test
            //arrange
            var mock_db = new Mock<Ievents_connection>();
            var controller = create_events_controller(mock_db);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Participant", 5)
            };

            //act
            var result = controller.get_organiser_events();

            //assert
            var status_result = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, status_result.StatusCode);
        }//end of test

    }//end of class

}//end of namespace