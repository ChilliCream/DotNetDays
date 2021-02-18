using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using WorkshopChatServer.Repositories.Interfaces;
using WorkshopChatServer.Types.Message;

namespace WorkshopChatServer.Repositories
{
    public class MessageRepository : AbstractRepository, IMessageRepository
    {
        class GetMessageSelectResult
        {
            public Guid Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Discriminator { get; set; }
            public string BelongsToChannel { get; set; }
            public string Content { get; set; }
            public string UserId { get; set; }
        }

        public async Task<IList<IMessage>> GetMessagesByChannelName(string channelName)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var result = await conn.QueryAsync<GetMessageSelectResult>(
                @"
            select 
                id ""Id""
                , createdat ""CreatedAt""
                , discriminator ""Discriminator""
                , content ""Content""
                , createdbyuser ""UserId""
            from message
            where belongstochannel = @Channel 
            and id not in (select distinct startermessageid from message where startermessageid is not null)
            and id not in (select responseid from threadresponses)
            order by createdat desc;",
                new
                {
                    Channel = channelName,
                });

            return result.Select<GetMessageSelectResult, IMessage>(
                rawResult =>
                {
                    return rawResult.Discriminator switch
                    {
                        nameof(SimpleMessage) => new SimpleMessage()
                        {
                            Id = rawResult.Id,
                            CreatedAt = rawResult.CreatedAt,
                            UserId = rawResult.UserId,
                            BelongsToChannel = rawResult.BelongsToChannel,
                            Content = rawResult.Content
                        },
                        nameof(Thread) => new Thread()
                        {
                            Id = rawResult.Id,
                            CreatedAt = rawResult.CreatedAt,
                            BelongsToChannel = rawResult.BelongsToChannel,
                            UserId = rawResult.UserId,
                        },
                        _ => throw new ArgumentException()
                    };
                }).ToList();
        }

        public async Task<SimpleMessage> GetThreadStarterMessageByThreadId(Guid threadId)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var starter = await conn.QuerySingleAsync<SimpleMessage>(
                @"select 
                    starter.id ""Id""
                    , starter.createdat ""CreatedAt""
                    , starter.createdbyuser ""UserId""
                    , starter.content ""Content""
                  from message thread 
                    inner join message starter on thread.startermessageid = starter.id
                  where thread.id = @Id",
                new
                {
                    Id = threadId,
                });

            return starter;
        }

        public async Task<IList<SimpleMessage>> GetThreadResponsesByThreadId(Guid threadId)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var result = await conn.QueryAsync<SimpleMessage>(
                @"select 
                    message.id ""Id""
                    , message.createdat ""CreatedAt""
                    , message.createdbyuser ""UserId""
                    , message.content ""Content""
                  from threadresponses thread 
                    inner join message on thread.responseid = message.id
                  where thread.threadid = @Id",
                new
                {
                    Id = threadId,
                });

            return result.ToList();
        }

        public async Task<SimpleMessage> PostSimpleMessage(String channelName, String userName,
            String message)
        {
            var ret = new SimpleMessage()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                UserId = userName,
                Content = message
            };

            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();
            var sql =
                @"insert into Message(
                    id, 
                    createdat, 
                    belongstochannel, 
                    createdbyuser, 
                    discriminator, 
                    content) values 
                    (@id, @createdAt, @belongstochannel, @username, @discriminator, @content);";

            await conn.ExecuteAsync(
                sql,
                new
                {
                    id = ret.Id,
                    createdat = ret.CreatedAt,
                    belongstochannel = channelName,
                    username = userName,
                    discriminator = nameof(SimpleMessage),
                    content = message,
                });

            return ret;
        }

        public async Task<Thread> ReplyToMessage(Guid messageId, string message)
        {
            var originalMessage = await GetMessageById(messageId);

            
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            Thread thread;
            switch (originalMessage)
            {
                case Thread thread2:
                    thread = thread2;
                    break;
                case SimpleMessage simpleMessage:
                {
                    thread = new Thread()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        UserId = simpleMessage.UserId
                    };
                
                    var sql = @"
                    insert into message(id, createdat, belongstochannel, createdbyuser, discriminator, startermessageid)
                    values (@id, @createdat, @belongstochannel, @createdbyuser, @discriminator, @startermessageid);";
                    await conn.ExecuteAsync(
                        sql,
                        new
                        {
                            id = thread.Id,
                            createdat = DateTime.Now,
                            belongstochannel = simpleMessage.BelongsToChannel,
                            createdbyuser = thread.UserId,
                            discriminator = nameof(Thread),
                            startermessageid = simpleMessage.Id
                        });
                    break;
                }
                default:
                    throw new InvalidCastException();
            }

            var postedMessage = await PostSimpleMessage(
                originalMessage.BelongsToChannel,
                originalMessage.UserId,
                message);

            await conn.ExecuteAsync(
                "insert into threadresponses(threadid, responseid) VALUES (@threadid, @responseid);",
                new
                {
                    threadid = thread.Id,
                    responseid = postedMessage.Id
                });

            return thread;
        }


        private async Task<IMessage> GetMessageById(Guid messageId)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var result = await conn.QuerySingleAsync<GetMessageSelectResult>(
                @"
            select 
                id ""Id""
                , createdat ""CreatedAt""
                , discriminator ""Discriminator""
                , content ""Content""
                , belongstochannel ""BelongsToChannel""
                , createdbyuser ""UserId""
            from message
            where id = @id;",
                new
                {
                    id = messageId,
                });

            return result.Discriminator switch
            {
                nameof(SimpleMessage) => new SimpleMessage()
                {
                    Id = result.Id,
                    CreatedAt = result.CreatedAt,
                    BelongsToChannel = result.BelongsToChannel,
                    UserId = result.UserId,
                    Content = result.Content
                },
                nameof(Thread) => new Thread()
                {
                    Id = result.Id,
                    CreatedAt = result.CreatedAt,
                    BelongsToChannel = result.BelongsToChannel,
                    UserId = result.UserId,
                },
                _ => throw new ArgumentException()
            };
        }

        public async Task<ILookup<String, IMessage>> GetMessagesByChannelNames(IReadOnlyList<String> keys)
        {
            await using var conn = new NpgsqlConnection(ConnString);
            await conn.OpenAsync();

            var result = await conn.QueryAsync<GetMessageSelectResult>(
                @"
            select 
                id ""Id""
                , createdat ""CreatedAt""
                , discriminator ""Discriminator""
                , content ""Content""
                , createdbyuser ""UserId""
                , belongstochannel ""BelongsToChannel""
            from message
            where belongstochannel = any(@keys) 
            and id not in (select distinct startermessageid from message where startermessageid is not null)
            and id not in (select responseid from threadresponses)
            order by createdat desc;",
                new
                {
                    keys = keys,
                });

            return result.Select<GetMessageSelectResult, IMessage>(
                rawResult =>
                {
                    return rawResult.Discriminator switch
                    {
                        nameof(SimpleMessage) => new SimpleMessage()
                        {
                            Id = rawResult.Id,
                            CreatedAt = rawResult.CreatedAt,
                            UserId = rawResult.UserId,
                            BelongsToChannel = rawResult.BelongsToChannel,
                            Content = rawResult.Content
                        },
                        nameof(Thread) => new Thread()
                        {
                            Id = rawResult.Id,
                            CreatedAt = rawResult.CreatedAt,
                            BelongsToChannel = rawResult.BelongsToChannel,
                            UserId = rawResult.UserId,
                        },
                        _ => throw new ArgumentException()
                    };
                }).ToList().ToLookup(x => x.BelongsToChannel, x => x);
        }
    }
}