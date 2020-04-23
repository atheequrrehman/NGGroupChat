using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NGGroupChat.Service.Api.DBModels;
using NGGroupChat.Service.Api.Models;
using NGGroupChat.Service.Api.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ChatDbContext chatDbContext;
        private readonly IMapper mapper;

        public UserService(ChatDbContext chatDbContext, IMapper mapper)
        {
            this.chatDbContext = chatDbContext;
            this.mapper = mapper;
        }

        public async Task<(bool IsSuccess, bool IsDuplicate, string GroupName, string ErrorMessage)> CreateUserGroups(Models.CreateGroup group)
        {
            try
            {
                if (group == null || group.GroupName == "")
                {
                    return (false, false, group.GroupName, "Enter Group Name");
                }
                if ((chatDbContext.Groups.Any(gp => gp.GroupName == group.GroupName)) == true)
                {
                    return (false, true, group.GroupName, "Group Name Already Exist");
                }
                DBModels.Group newGroup = new DBModels.Group { GroupName = group.GroupName };
                chatDbContext.Groups.Add(newGroup);
                chatDbContext.SaveChanges();

                foreach (string UserName in group.UserNames)
                {
                    chatDbContext.UserGroups.Add(
                        new DBModels.UserGroup { UserName = UserName, GroupId = newGroup.ID }
                    );
                    await chatDbContext.SaveChangesAsync();
                }
                return (true, false, group.GroupName, null);
            }
            catch (Exception ex)
            {
                return (false, false, group.GroupName, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, bool IsDuplicate, string GroupName, string ErrorMessage)> CreateGroups(DBModels.Group group)
        {
            try
            {
                if (group == null || group.GroupName == "")
                {
                    return (false, false, group.GroupName, "Enter Group Name");
                }
                if ((chatDbContext.Groups.Any(gp => gp.GroupName == group.GroupName)) == true)
                {
                    return (false, true, group.GroupName, "Group Name Already Exist");
                }
                DBModels.Group newGroup = new DBModels.Group { GroupName = group.GroupName };
                chatDbContext.Groups.Add(newGroup);
                await chatDbContext.SaveChangesAsync();
                return (true, false, group.GroupName, null);
            }
            catch (Exception ex)
            {
                return (false, false, group.GroupName, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.User> Users, string ErrorMessage)> GetAllUsers()
        {
            try
            {
                var users = await chatDbContext.Users.ToListAsync();
                if (users != null && users.Any())
                {
                    var result = mapper.Map<IEnumerable<DBModels.User>, IEnumerable<Models.User>>(users);
                    return (true, result, null);
                }
                return (false, null, "No Users Found");
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, Models.User userDetail, string ErrorMessage)> GetUserDetail(DBModels.User userObj)
        {
            try
            {
                var user = await chatDbContext.Users.FirstOrDefaultAsync(x => x.UserName == userObj.UserName && x.Password == userObj.Password);
                if (user != null)
                {
                    var result = new Models.User { FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName };
                    return (true, result, null);
                }
                return (false, null, "Login Failed, User not Registered");
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.UserGroup> UserGroups, string ErrorMessage)> GetUserGroups(string userName)
        {
            try
            {
                var userGroup = await chatDbContext.UserGroups.Where(x => x.UserName == userName)
                    .Join(chatDbContext.Groups, ug => ug.GroupId, g => g.ID, (ug, g) =>
                                     new Models.UserGroup
                                     {
                                         UserName = ug.UserName,
                                         GroupId = g.ID,
                                         GroupName = g.GroupName
                                     }).ToListAsync();
                if (userGroup != null && userGroup.Any())
                {
                    return (true, userGroup, null);
                }
                return (false, null, "No Groups Found");
            }
            catch (Exception ex)
            {

                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, bool IsDuplicate, string UserName, string ErrorMessage)> RegisterUser(DBModels.User userObj)
        {
            try
            {
                var user = await chatDbContext.Users.FirstOrDefaultAsync(x => x.UserName == userObj.UserName);
                if (user != null)
                {
                    return (false, true, userObj.UserName, "UserName already Exist");
                }
                chatDbContext.Users.Add(new DBModels.User { FirstName = userObj.FirstName, LastName = userObj.LastName, Password = userObj.Password, UserName = userObj.UserName });
                await chatDbContext.SaveChangesAsync();
                return (true, false, userObj.UserName, "Registered Successfully");
            }
            catch (Exception ex)
            {
                return (false, false, userObj.UserName, ex.Message);
            }
        }

        public async Task<(bool IsSucess, bool IsGroupFull, string Message)> JoinGroup(DBModels.UserGroup groupDetails)
        {
            try
            {
                var usersInGroups = await chatDbContext.UserGroups.Where(x => x.GroupId == groupDetails.GroupId).ToListAsync();
                string groupName = chatDbContext.Groups.SingleOrDefault(x => x.ID == groupDetails.GroupId)?.GroupName?.ToString();
                if (usersInGroups != null)
                {
                    if (usersInGroups.Count >= 20)
                    {
                        return (true, true, "Cannot Join the Group " + groupName + " as it is Full");
                    }
                    else
                    {
                        chatDbContext.UserGroups.Add(
                            new DBModels.UserGroup { UserName = groupDetails.UserName, GroupId = groupDetails.GroupId });
                        await chatDbContext.SaveChangesAsync();
                        return (true, false, "Joined " + groupName + " Successfully");
                    }
                }
                return (false, false, "Cannot Join the Group");
            }
            catch (Exception ex)
            {
                return (false, false, ex.Message);
            }
        }

        public async Task<(bool IsSucess, bool IsDeleted, string ErrorMessage)> LeaveGroup(DBModels.UserGroup group)
        {
            try
            {
                var userGroup = chatDbContext.UserGroups.Where(x => x.GroupId == group.GroupId && x.UserName == group.UserName).FirstOrDefault<DBModels.UserGroup>();
                if (userGroup != null)
                {
                    chatDbContext.UserGroups.Remove(userGroup);
                    await chatDbContext.SaveChangesAsync();
                    return (true, true, "Removed from Group SuccessFully!");
                }
                return (false, false, "Group Not Found");
            }
            catch (Exception ex)
            {
                return (false, false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, bool IsUserInGroup, string ErrorMessage)> CheckUserSubscribedToGroup(int GroupId, string UserName)
        {
            try
            {
                var userGroup = await chatDbContext.UserGroups.Where(x => x.GroupId == GroupId && x.UserName == UserName).FirstOrDefaultAsync<DBModels.UserGroup>();
                if (userGroup != null)
                {
                    return (true, true, null);
                }
                return (true, false, "User Does not belong to the Group");
            }
            catch (Exception ex)
            {
                return (true, false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Group> Groups, string ErrorMessage)> GetAllGroups()
        {
            try
            {
                var groups = await chatDbContext.Groups.ToListAsync();
                if (groups != null && groups.Any())
                {
                    var result = mapper.Map<IEnumerable<DBModels.Group>, IEnumerable<Models.Group>>(groups);
                    return (true, result, null);
                }
                return (false, null, "No Groups Found");
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
    }
}
