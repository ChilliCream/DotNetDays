using WorkshopChatServer.Types.Channels;

namespace WorkshopChatServer.Types.Workspaces
{
    public class Workspace
    {
        public string Name { get; set; }

        public async Task<UserNamespace.Channel> Channel(
            [Service] IChannelRepository channelRepository
        ) {
            return await channelRepository.GetChannelByWorkspace();
        }
    }
}