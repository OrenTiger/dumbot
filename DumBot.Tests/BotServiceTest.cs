using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoFixture;
using DumBot.Infrastructure;
using DumBot.Models;
using DumBot.Resources;
using DumBot.Services;

namespace DumBot.Tests
{
    [TestClass]
    public class BotServiceTest
    {
        Mock<BotService> _botService;
        Fixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _botService = new Mock<BotService>(new Mock<IApplicationSettings>().Object,
                new Mock<ILogger<BotService>>().Object);

            _botService.CallBase = true;
            _botService.Setup(x => x.SendMessageAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        }

        [TestMethod]
        public async Task HandleMessage_CorrectCommand_ShouldAccept()
        {
            // Arrange
            _botService.Setup(x => x.GetRandomDocAsync(It.IsAny<string>())).Returns(Task.FromResult(_fixture.Create<string>()));

            // Act
            await _botService.Object.HandleMessageAsync($"/{BotCommands.CatGif}", It.IsAny<int>());

            // Assert
            _botService.Verify(x => x.SendMessageAsync(It.IsAny<int>(), string.Empty, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task HandleMessage_IncorrectCommand_ShouldReturnHelpMessage()
        {
            // Arrange
            string useHelpMessage = $"{BotMessages.DumbBot}. {string.Format(BotMessages.UseHelp, BotCommands.Help)}";

            // Act
            await _botService.Object.HandleMessageAsync(_fixture.Create<string>(), It.IsAny<int>());

            // Assert
            _botService.Verify(x => x.SendMessageAsync(It.IsAny<int>(), useHelpMessage, string.Empty), Times.Once);
        }
    }
}
