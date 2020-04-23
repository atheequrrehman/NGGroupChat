using Microsoft.AspNetCore.Mvc;
using NGGroupChat.Service.Api.DBModels;
using NGGroupChat.Service.Api.Models;
using NGGroupChat.Service.Api.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> GetUserDetail(DBModels.User user)
        {
            var result = await userService.GetUserDetail(user);
            if (result.IsSuccess)
                return new OkObjectResult(result.userDetail);
            else
                return Unauthorized(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(DBModels.User user)
        {
            var result = await userService.RegisterUser(user);
            if (result.IsSuccess)
                return new OkObjectResult(result.UserName);
            else if (!result.IsSuccess && result.IsDuplicate)
                return BadRequest(result.ErrorMessage);
            else
                return NotFound(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await userService.GetAllUsers();
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Users);
            }
            return NotFound();
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetUserGroups(string userName)
        {
            var result = await userService.GetUserGroups(userName);
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.UserGroups);
            }
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var result = await userService.GetAllGroups();
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Groups);
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewGroup(CreateGroup createGroup)
        {
            var result = await userService.CreateUserGroups(createGroup);
            if (result.IsSuccess)
                return new OkObjectResult(result.GroupName);
            else if (!result.IsSuccess && result.IsDuplicate)
                return BadRequest(result.ErrorMessage);
            else
                return NotFound(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(DBModels.Group createGroup)
        {
            var result = await userService.CreateGroups(createGroup);
            if (result.IsSuccess)
                return new OkObjectResult(result.GroupName);
            else if (!result.IsSuccess && result.IsDuplicate)
                return BadRequest(result.ErrorMessage);
            else
                return NotFound(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> JoinGroup(DBModels.UserGroup userGroup)
        {
            var result = await userService.JoinGroup(userGroup);
            if (result.IsSucess)
            {
                var returnObj = new { IsGroupFull = result.IsGroupFull, Message = result.Message };
                return new OkObjectResult(returnObj);
            }
            else
                return NotFound(result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> LeaveGroup(DBModels.UserGroup userGroup)
        {
            var result = await userService.LeaveGroup(userGroup);
            if (result.IsSucess)
            {
                var returnObj = new { IsDeleted = result.IsDeleted, ErrorMessage = result.ErrorMessage };
                return new OkObjectResult(returnObj);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet("{GroupId}/{userName}")]

        public async Task<ActionResult> CheckUserSubscribedToGroup(int GroupId, string UserName)
        {
            var result = await userService.CheckUserSubscribedToGroup(GroupId, UserName);
            if (result.IsSuccess)
            {
                var returnObj = new { IsUserInGroup = result.IsUserInGroup, ErrorMessage = result.ErrorMessage };
                return new OkObjectResult(returnObj);
            }
            return NotFound(result.ErrorMessage);
        }
    }
}
