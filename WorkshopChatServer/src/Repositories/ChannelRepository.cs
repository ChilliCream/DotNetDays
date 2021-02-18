using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using WorkshopChatServer.Repositories.Interfaces;
using WorkshopChatServer.Types.Channels;

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

        class DataLoaderResult
        {
            public string Name { get; set; }
            public string WorkspaceName { get; set; }
        }
        public async Task<ILookup<String, Channel>> GetChannelsByWorkspaces(IReadOnlyList<String> keys)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var result = await conn.QueryAsync<DataLoaderResult>("select name as \"Name\", belongsToWorkspace as \"WorkspaceName\" from channel where belongsToWorkspace = any(@keys)",
                new
                {
                    keys = keys,
                }
            );

            return result.ToLookup(x => x.WorkspaceName, x => new Channel(){Name = x.Name});
        }
    }
}