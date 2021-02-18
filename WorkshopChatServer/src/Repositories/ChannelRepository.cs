using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using WorkshopChatServer.Repositories.Interfaces;
using WorkshopChatServer.Types.Channel;

namespace WorkshopChatServer.Repositories
{
    public class ChannelRepository : AbstractRepository, IChannelRepository
    {
        public async Task<List<Channel>> GetChannelByWorkspace(String workspaceName)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var result = await conn.QueryAsync<Channel>("select Name from channel where belongsToWorkspace = @Workspace",
                new
                {
                    Workspace = workspaceName,
                }
            );

            return result.ToList();
        }

        public async Task<Channel> CreateChannel(String workspaceName, String channelName)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var result = await conn.ExecuteAsync(@"
insert into Channel(name, belongsToWorkspace) 
VALUES (@name, @belongsToWorkspace);",
                new
                {
                    name = channelName,
                    belongsToWorkspace = workspaceName,
                }
            );

            return new Channel()
            {
                Name =  channelName
            };
        }
    }
}