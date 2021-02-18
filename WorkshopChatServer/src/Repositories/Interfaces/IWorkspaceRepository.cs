using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshopChatServer.Types.Workspaces;

namespace WorkshopChatServer.Repositories.Interfaces
{
    public interface IWorkspaceRepository
    {
        public Task<IList<Workspace>> GetAllWorkspaces();
        public Task<Workspace> CreateWorkspace(string name);
    }
}