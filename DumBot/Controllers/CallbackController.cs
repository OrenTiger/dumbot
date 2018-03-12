using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DumBot.Models;
using Microsoft.Extensions.Configuration;

namespace DumBot.Controllers
{
    [Route("[controller]")]
    public class CallbackController : Controller
    {
        public IConfiguration Configuration { get; }

        private readonly int _serverConfirmationGroupId;
        private readonly string _serverConfirmationReplyString;
        
        public CallbackController(IConfiguration configuration)
        {
            Configuration = configuration;
            _serverConfirmationGroupId = int.Parse(Configuration["ServerConfirmationGroupId"]);
            _serverConfirmationReplyString = Configuration["ServerConfirmationReplyString"];
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public string Post([FromBody]CallbackEventModel callbackEvent)
        {
            if (string.IsNullOrEmpty(callbackEvent.Type))
            {
                HttpContext.Response.StatusCode = 400;
                return string.Empty;
            }

            switch (callbackEvent.Type)
            {
                case CallbackEventType.Confirmation:
                    return callbackEvent.Group_id == _serverConfirmationGroupId
                        ? _serverConfirmationReplyString
                        : string.Empty;
                default:
                    return "ok";
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
