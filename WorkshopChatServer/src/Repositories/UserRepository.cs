using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using WorkshopChatServer.Repositories.Interfaces;
using WorkshopChatServer.Types.User;

namespace WorkshopChatServer.Repositories
{
    public class UserRepository: AbstractRepository, IUserRepository
    {
        public async  Task<User> GetUserByMessageId(Guid messageId)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var result = await conn.QuerySingleAsync<User>(
                "select au.name as Name, au.lastSeen as LastSeen from Message m inner join AppUser AU on AU.name = m.createdByUser where m.id = @id;",
                new
                {
                    id = messageId,
                }
            );

            return result;
        }

        public async Task<User> CreateUser(String userName)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            await conn.ExecuteAsync("insert into appuser (name) values(@name)", new
            {
                name = userName
            });

            return new User()
            {
                Name = userName
            };
        }
    }
}