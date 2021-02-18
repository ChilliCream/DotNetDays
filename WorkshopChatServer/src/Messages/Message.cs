using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using UserNamespace = WorkshopChatServer.Types.User;
using WorkshopChatServer.Repositories.Interfaces;

namespace WorkshopChatServer.Types.Message
{
    [UnionType("Message")]
    public interface IMessage
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public string BelongsToChannel { get; set; }
    }

    public class SimpleMessage : IMessage {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }


        public string BelongsToChannel { get; set; }
        
        public string Content {get; set;}

        public async Task<UserNamespace.User> Author(
            [Service] UserNamespace.AuthorByMessageIdDataLoader userRepository
        ) {
            return await userRepository.LoadAsync(Id, CancellationToken.None);
        }


    }

    public class Thread : IMessage {
        public Guid Id { get; set; }
        
        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string BelongsToChannel { get; set; }      
        
        public SimpleMessage StarterMessage { get; set; }
        
        public Task<IList<SimpleMessage>> Responses(
            [Service] IMessageRepository messageRepository
        ) {
            return messageRepository.GetThreadResponsesByThreadId(Id);
        }

        public async Task<UserNamespace.User> Author(
            [Service] UserNamespace.AuthorByMessageIdDataLoader userRepository
        ) {
            return await userRepository.LoadAsync(Id, CancellationToken.None);
        }
    }
}