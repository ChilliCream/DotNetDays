using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshopChatServer.Types.Channels;

namespace WorkshopChatServer.Repositories.Interfaces
{
    public interface IChannelRepository
    {
        Task<List<Channel>> GetChannelByWorkspace(string workspaceName);
        Task<Channel> CreateChannel(string workspaceName, string channelName);
    }
}