using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using WorkshopChatServer.Repositories.Interfaces;
using WorkshopChatServer.Types.Workspaces;

namespace WorkshopChatServer.Repositories
{
    public class WorkspaceRepository: AbstractRepository, IWorkspaceRepository
    {
        public async Task<IList<Workspace>> GetAllWorkspaces()
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var result = await conn.QueryAsync<Workspace>("select Name from Workspace;");

            return result.ToList();
        }

        public async Task<Workspace> CreateWorkspace(String name)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var result = await conn.ExecuteAsync(@"
insert into Workspace(name)
VALUES (@name);",
                new
                {
                    name = name,
                }
            );

            return new Workspace()
            {
                Name =  name
            };
        }
    }
}