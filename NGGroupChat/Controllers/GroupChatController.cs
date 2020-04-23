using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using NGGroupChat.Service.Api.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GroupChatController : ControllerBase
    {
        private readonly IChatService chatService;

        public GroupChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(DBModels.Message newMessage)
        {
            var result = await chatService.CreateMessage(newMessage);
            if (result.IsSucess)
            {
                return new OkObjectResult(result.Message);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet("{groupID}")]

        public async Task<IActionResult> GetGroupMessages(int GroupId)
        {
            var result = await chatService.GetGroupMessages(GroupId);
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Message);
            }
            return NotFound(result.ErrorMessage);
        }
    }
}
