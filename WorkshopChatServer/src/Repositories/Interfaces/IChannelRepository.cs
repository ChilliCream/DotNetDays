using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkshopChatServer.Types.Channels;

namespace WorkshopChatServer.Repositories.Interfaces
{
    public interface IChannelRepository
    {
        Task<List<Channel>> GetChannelByWorkspace(string workspaceName);
        Task<Channel> CreateChannel(string workspaceName, string channelName);
        Task<ILookup<String, Channel>> GetChannelsByWorkspaces(IReadOnlyList<String> keys);
    }
}