using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkshopChatServer.Types.Message;

namespace WorkshopChatServer.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        public Task<IList<IMessage>> GetMessagesByChannelName(string channelName);
        public Task<SimpleMessage> GetThreadStarterMessageByThreadId(Guid threadId);
        public Task<IList<SimpleMessage>> GetThreadResponsesByThreadId(Guid threadId);

        public Task<SimpleMessage> PostSimpleMessage(string channelName, string userName, string message);
        public Task<Thread> ReplyToMessage(Guid messageId, string message);
        Task<ILookup<String, IMessage>> GetMessagesByChannelNames(IReadOnlyList<string> keys);
    }
}