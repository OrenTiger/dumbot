using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DumBot.Models.Callback;
using DumBot.Services;
using Newtonsoft.Json;
using DumBot.Infrastructure;

namespace DumBot.Controllers
{
    [Route("[controller]")]
    public class CallbackController : Controller
    {
        private readonly IApplicationSettings _settings;
        private readonly IBotService _botService;
        private readonly ILogger<CallbackController> _logger;
        
        public CallbackController(IApplicationSettings settings, IBotService botService,
            ILogger<CallbackController> logger)
        {
            _settings = settings;
            _botService = botService;
            _logger = logger;
        }

        // GET /callback
        [HttpGet]
        public string Get()
        {
            return "ok";
        }

        /// <summary>
        /// POST: /callback
        /// Handle incoming event
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]CallbackEventModel callbackEvent)
        {
            if (string.IsNullOrEmpty(callbackEvent.Type))
            {
                return BadRequest();
            }

            switch (callbackEvent.Type)
            {
                case CallbackEventType.Confirmation:
                    if (callbackEvent.Group_id != _settings.ServerConfirmationGroupId)
                    {
                        _logger.LogWarning($"Callback server confirmation failed. Group ids are mismatch. GroupId: {callbackEvent.Group_id}");
                        return BadRequest();
                    }

                    return Ok(new { Value = _settings.ServerConfirmationReplyString });
                case CallbackEventType.NewMessage:

                    var message = JsonConvert.DeserializeObject<MessageModel>(callbackEvent.Object.ToString());

                    await _botService.HandleMessageAsync(message.Body, message.User_id);

                    return Ok("ok");
                default:
                    return Ok("ok");
            }
        }
    }
}
