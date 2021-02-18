using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using HotChocolate;
using HotChocolate.Types;
using WorkshopChatServer.Repositories.Interfaces;
using WorkshopChatServer.Types;

namespace WorkshopChatServer.Types.Workspaces
{
    [ExtendObjectType("Query")]
    public class UserQuery
    {
        public Task<WorkshopChatServer.Types.User.User> GetUserByMessageId(
            [Service] IUserRepository userRepository, Guid userId)
        {
            return userRepository.GetUserByMessageId(userId);
        }

        public Task<WorkshopChatServer.Types.User.User> GetMe()
        {
            throw new NotImplementedException();
        }
    }
}