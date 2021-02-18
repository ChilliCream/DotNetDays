using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GreenDonut;
using HotChocolate;
using HotChocolate.DataLoader;
using WorkshopChatServer.Repositories.Interfaces;
using WorkshopChatServer.Types.Message;

namespace WorkshopChatServer.Types.Channels
{
    public class Channel
    {
        public string Name { get; set; }
        
        public Task<IMessage[]> GetMessages(
            [DataLoader] MessageByChannelDataloader messageRepository
            )
        {
            return messageRepository.LoadAsync(Name, CancellationToken.None);
        } 
    }
    
    public class MessageByChannelDataloader: GroupedDataLoader<string, IMessage>
    {
        private readonly IMessageRepository _messageRepository;

        public MessageByChannelDataloader(
            [Service] IMessageRepository messageRepository,
            IBatchScheduler batchScheduler, 
            DataLoaderOptions<String>? options = null) : base(batchScheduler,
            options)
        {
            _messageRepository = messageRepository;
        }

        protected override Task<ILookup<String, IMessage>> LoadGroupedBatchAsync(IReadOnlyList<String> keys, CancellationToken cancellationToken)
        {
            return _messageRepository.GetMessagesByChannelNames(keys);
        }
    };
}