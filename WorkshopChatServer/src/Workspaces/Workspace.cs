using WorkshopChatServer.Repositories.Interfaces;
using HotChocolate;
using WorkshopChatServer.Types.Channels;

namespace WorkshopChatServer.Types.Workspaces
{
    public class Workspace
    {
        public string Name { get; set; }

        public async Task<Channel> Channel(
            [Service] IChannelRepository channelRepository, string workspaceName
        ) {
            return await channelRepository.GetChannelByWorkspace(workspaceName);
        }
    }
}