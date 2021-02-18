using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshopChatServer.Types.User;

namespace WorkshopChatServer.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<WorkshopChatServer.Types.User.User> GetUserByMessageId(Guid messageIds);
        public Task<WorkshopChatServer.Types.User.User> CreateUser(string userName);
    }
}