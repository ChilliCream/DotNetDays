using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using WorkshopChatServer.Repositories.Interfaces;

namespace WorkshopChatServer.Types.Workspaces
{
    [ExtendObjectType("Query")]
    public class WorkspaceQuery
    {
        public Task<IList<Workspace>> GetAllWorkspaces(
            [Service] IWorkspaceRepository workspaceRepository)
        {
            return workspaceRepository.GetAllWorkspaces();
        }
    }
}