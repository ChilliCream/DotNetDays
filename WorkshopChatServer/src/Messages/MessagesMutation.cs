using System;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Subscriptions;
using WorkshopChatServer.Repositories.Interfaces;

namespace WorkshopChatServer.Types.Message
{
    [ExtendObjectType("Mutation")]
    public class MessagesMutation
    {
        public async Task<SimpleMessage> PostSimpleMessage(
            [Service] IMessageRepository messageRepository,
            [Service] ITopicEventSender  eventSender,
            string channelName,
            string userName,
            string message)
        {
            var result = await messageRepository.PostSimpleMessage(channelName, userName, message);
            await eventSender.SendAsync(channelName, result);
            return result;
        }

        public Task<Thread> PostResponse(
            [Service] IMessageRepository messageRepository, Guid messageId, string message)
        {
            return messageRepository.ReplyToMessage(messageId, message);
        }
    }
}