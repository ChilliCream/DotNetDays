using System;
using System.Collections.Generic;
using System.Linq;
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

        class DataLoaderResult
        {
            public string Name { get; set; }
            public DateTime LastSeen { get; set; }
            public Guid MessageId { get; set; }
        }
        public async Task<IReadOnlyDictionary<Guid, User>> GetUsersByMessageIds(IReadOnlyList<Guid> keys)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var result = await conn.QueryAsync<DataLoaderResult>(
                "select " +
                "au.name as Name, " +
                "au.lastSeen as LastSeen, " +
                "m.id as MessageId " +
                "from Message m " +
                "inner join " +
                "AppUser AU on AU.name = m.createdByUser " +
                "where m.id = any(@keys);",
                new
                {
                    keys = keys,
                }
            );

            return result.ToDictionary(x => x.MessageId, x => new User() {Name = x.Name, LastSeen = x.LastSeen});
        }
    }
}