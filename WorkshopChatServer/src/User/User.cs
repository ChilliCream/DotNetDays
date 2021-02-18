using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using HotChocolate;
using HotChocolate.DataLoader;
using WorkshopChatServer.Repositories.Interfaces;

namespace WorkshopChatServer.Types.User
{
    public class User
    {
        public string Name { get; set; }

        public DateTime LastSeen { get; set; } 
    }
    
    public class AuthorByMessageIdDataLoader: BatchDataLoader<Guid, User>
    {
        private readonly IUserRepository _userRepository;

        public AuthorByMessageIdDataLoader(
            [Service] IUserRepository userRepository,
            IBatchScheduler batchScheduler, 
            DataLoaderOptions<Guid>? options = null) : base(batchScheduler,
            options)
        {
            _userRepository = userRepository;
        }

        protected override Task<IReadOnlyDictionary<Guid, User>> LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
        {
            return _userRepository.GetUsersByMessageIds(keys);
        }
    };
}