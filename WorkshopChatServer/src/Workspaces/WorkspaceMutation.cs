using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using WorkshopChatServer.Repositories.Interfaces;

namespace WorkshopChatServer.Types.Workspaces
{
    [ExtendObjectType("Mutation")]
    public class WorkspaceMutation
    {
        [Authorize]
        public Task<Workspace> CreateWorkspace(
            [Service] IWorkspaceRepository workspaceRepository,
            string name)
        {
            return workspaceRepository.CreateWorkspace(name);
        }
    }
}