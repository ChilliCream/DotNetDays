using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using WorkshopChatServer.Repositories.Interfaces;
using WorkshopChatServer.Types.Channels;

namespace WorkshopChatServer.Types.Channels
{
    [ExtendObjectType("Mutation")]
    public class ChannelMutation
    {
        public async Task<CreateChannelPayload> CreateChannel(
            [Service] IChannelRepository channelRepository,
            CreateChannelInput input)
        {
            var channel = await channelRepository.CreateChannel(
                input.WorkspaceName, 
                input.ChannelName);

            return new(channel);
        }
    }

    public record CreateChannelInput(string WorkspaceName, string ChannelName);

    public record CreateChannelPayload(Channel Channel);
}