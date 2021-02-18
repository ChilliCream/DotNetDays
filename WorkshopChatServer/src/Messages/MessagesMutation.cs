using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using WorkshopChatServer.Repositories.Interfaces;
using System;

namespace WorkshopChatServer.Types.Message
{
    [ExtendObjectType("Mutation")]
    public class MessagesMutation
    {
        public Task<SimpleMessage> PostSimpleMessage(
            [Service] IMessageRepository messageRepository,
            String channelName, String userName,
            String message)
        {
            return messageRepository.PostSimpleMessage(channelName,userName,message);
        }

        public Task<Thread> PostRespone(
            [Service] IMessageRepository messageRepository, Guid messageId, string message)
            {
                return messageRepository.ReplyToMessage(messageId,message);
            }
    }
}