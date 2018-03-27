using System.Threading.Tasks;
using Newtonsoft.Json;
using AutoFixture;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DumBot.Controllers;
using DumBot.Infrastructure;
using DumBot.Models.Callback;
using DumBot.Services;

namespace DumBot.Tests
{
    [TestClass]
    public class CallbackControllerTest
    {
        [TestMethod]
        public async Task Post_ConfirmationEventWithCorrectGroupId_ShouldConfirm()
        {
            // Arrange
            var fixture = new Fixture();
            int groupId = fixture.Create<int>();
            string serverConfirmationReplyString = fixture.Create<string>();

            var stubApplicationSettings = new Mock<IApplicationSettings>();
            stubApplicationSettings.SetupGet(x => x.ServerConfirmationGroupId).Returns(groupId);
            stubApplicationSettings.SetupGet(x => x.ServerConfirmationReplyString).Returns(serverConfirmationReplyString);

            var botService = new Mock<BotService>(stubApplicationSettings.Object, new Mock<ILogger<BotService>>().Object);

            botService.CallBase = true;
            botService.Setup(x => x.HandleMessageAsync(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            CallbackController callbackController = new CallbackController(stubApplicationSettings.Object,
                botService.Object, new Mock<ILogger<CallbackController>>().Object);

            CallbackEventModel callbackEventModel = new CallbackEventModel()
            {
                Type = CallbackEventType.Confirmation,
                Group_id = groupId
            };

            OkObjectResult okResult = new OkObjectResult(new { Value = serverConfirmationReplyString });

            // Act
            var result = await callbackController.Post(callbackEventModel);

            // Assert
            Assert.AreEqual(okResult.Value.ToString(), (result as OkObjectResult)?.Value.ToString());
        }

        [TestMethod]
        public async Task Post_NewMessage_ShouldHandle()
        {
            // Arrange
            var fixture = new Fixture();
            var stubApplicationSettings = new Mock<IApplicationSettings>();
            var botService = new Mock<BotService>(stubApplicationSettings.Object, new Mock<ILogger<BotService>>().Object);

            botService.CallBase = true;
            botService.Setup(x => x.HandleMessageAsync(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            CallbackController callbackController = new CallbackController(stubApplicationSettings.Object,
                botService.Object, new Mock<ILogger<CallbackController>>().Object);

            CallbackEventModel callbackEventModel = new CallbackEventModel()
            {
                Type = CallbackEventType.NewMessage,
                Group_id = fixture.Create<int>(),
                Object = JsonConvert.SerializeObject(new { Body = fixture.Create<string>(), User_id = fixture.Create<int>() })
            };

            // Act
            var result = await callbackController.Post(callbackEventModel);

            // Assert
            botService.Verify(x => x.HandleMessageAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }
}
