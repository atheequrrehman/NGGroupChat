using Microsoft.AspNetCore.Mvc;
using NGGroupChat.Service.Api.DBModels;
using NGGroupChat.Service.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Providers
{
    public interface IUserService
    {
        Task<(bool IsSuccess, Models.User userDetail, string ErrorMessage)> GetUserDetail(DBModels.User user);
        Task<(bool IsSuccess,bool IsDuplicate, string UserName, string ErrorMessage)> RegisterUser(DBModels.User user);
        Task<(bool IsSuccess, IEnumerable<Models.User> Users, string ErrorMessage)> GetAllUsers();
        Task<(bool IsSuccess, IEnumerable<Models.UserGroup> UserGroups, string ErrorMessage)> GetUserGroups(string userName);
        Task<(bool IsSuccess, bool IsDuplicate, string GroupName, string ErrorMessage)> CreateUserGroups(CreateGroup userGroup);
        Task<(bool IsSuccess, bool IsDuplicate, string GroupName, string ErrorMessage)> CreateGroups(DBModels.Group group);
        Task<(bool IsSucess, bool IsGroupFull, string Message)> JoinGroup(DBModels.UserGroup group);
        Task<(bool IsSucess, bool IsDeleted ,string ErrorMessage)> LeaveGroup(DBModels.UserGroup group);
        Task<(bool IsSuccess, bool IsUserInGroup, string ErrorMessage)> CheckUserSubscribedToGroup(int GroupId, string UserName);
        Task<(bool IsSuccess, IEnumerable<Models.Group> Groups, string ErrorMessage)> GetAllGroups();
    }
}
