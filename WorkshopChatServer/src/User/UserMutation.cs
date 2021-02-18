using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using WorkshopChatServer.Repositories.Interfaces;
using System;

namespace WorkshopChatServer.Types.User
{
    [ExtendObjectType("Mutation")]
    public class UserMutation
    {
        public Task<User> CreateUser(
            [Service] IUserRepository userRepository,
            string name)
        {
            return userRepository.CreateUser(name);
        }

        public Task<User> UserLogin(
          [Service] IUserRepository userRepository,
          string userName)
        {
            return Task.FromResult(new User { Name = userName, LastSeen = DateTime.Now  });
        }
    }
}