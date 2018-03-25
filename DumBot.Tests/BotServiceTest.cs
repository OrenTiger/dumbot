using DumBot.Services;
using DumBot.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using DumBot.Models;
using System.Threading.Tasks;
using AutoFixture;

namespace DumBot.Tests
{
    [TestClass]
    public class BotServiceTest
    {
        [TestMethod]
        public async Task Handle_Message_Should_Accept_Correct_Command()
        {
            // Arrange
            var applicationSettings = new Mock<IApplicationSettings>().Object;
            var logger = new Mock<ILogger<BotService>>().Object;
            var botService = new Mock<BotService>(applicationSettings, logger);
            var fixture = new Fixture();

            botService.CallBase = true;
            botService.Setup(x => x.SendMessageAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            botService.Setup(x => x.GetRandomDocAsync(It.IsAny<string>())).Returns(Task.FromResult(fixture.Create<string>()));

            // Act
            await botService.Object.HandleMessageAsync($"/{BotCommands.CatGif}", It.IsAny<int>());

            // Assert
            botService.Verify(x => x.SendMessageAsync(It.IsAny<int>(), string.Empty, It.IsAny<string>()), Times.Once);
        }
    }
}
