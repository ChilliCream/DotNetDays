using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GreenDonut;
using WorkshopChatServer.Repositories.Interfaces;
using HotChocolate;
using HotChocolate.DataLoader;
using HotChocolate.Types;
using WorkshopChatServer.Types.Channels;

namespace WorkshopChatServer.Types.Workspaces
{
    [ExtendObjectType("Workspace")]
    public class WorkspaceChannelExtension
    {
        public Task<Channel[]> GetChannels(
            [Parent] Workspace workspace,
            [DataLoader] ChannelByWorkspaceDataLoader channelRepository)
        {
            return channelRepository.LoadAsync(workspace.Name, CancellationToken.None);
        }
    }

    public class ChannelByWorkspaceDataLoader: GroupedDataLoader<string, Channel>
    {
        private readonly IChannelRepository _channelRepository;

        public ChannelByWorkspaceDataLoader(
            [Service] IChannelRepository channelRepository,
            IBatchScheduler batchScheduler, 
            DataLoaderOptions<String>? options = null) : base(batchScheduler,
            options)
        {
            _channelRepository = channelRepository;
        }

        protected override Task<ILookup<String, Channel>> LoadGroupedBatchAsync(IReadOnlyList<String> keys, CancellationToken cancellationToken)
        {
            return _channelRepository.GetChannelsByWorkspaces(keys);
        }
    };
}