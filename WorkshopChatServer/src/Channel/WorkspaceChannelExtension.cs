using System.Threading.Tasks;
using System.Collections.Generic;
using WorkshopChatServer.Repositories.Interfaces;
using HotChocolate;
using HotChocolate.Types;

namespace WorkshopChatServer.Types.Workspaces
{
    [ExtendObjectType("Workspace")]
    public class WorkspaceChannelExtension
    {
        public Task<List<Channels.Channel>> GetChannels(
            [Parent] Workspace workspace,
            [Service] IChannelRepository channelRepository)
        {
            return channelRepository.GetChannelByWorkspace(workspace.Name);
        }
    }
}