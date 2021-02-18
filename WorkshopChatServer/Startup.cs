using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkshopChatServer.Repositories;
using WorkshopChatServer.Repositories.Interfaces;
using WorkshopChatServer.Types.Channels;
using WorkshopChatServer.Types.Message;
using WorkshopChatServer.Types.User;
using WorkshopChatServer.Types.Workspaces;

namespace WorkshopChatServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddAuthorization();

            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IChannelRepository, ChannelRepository>();
            services.AddSingleton<IWorkspaceRepository, WorkspaceRepository>();
            services.AddSingleton<IMessageRepository, MessageRepository>();

            services.AddScoped<AuthorByMessageIdDataLoader>();
            services.AddScoped<ChannelByWorkspaceDataLoader>();
            services.AddScoped<MessageByChannelDataloader>();

            services.AddHttpResultSerializer<CustomHttpResultSerializer>();

            services
                .AddGraphQLServer()
                .AddAuthorization()
                .AddQueryType(descriptor => descriptor.Name("Query"))
                    .AddTypeExtension<WorkspaceQuery>()
                    .AddTypeExtension<UserQuery>()
                .AddMutationType(descriptor => descriptor.Name("Mutation"))
                    .AddTypeExtension<WorkspaceMutation>()
                    .AddTypeExtension<MessagesMutation>()
                    .AddTypeExtension<ChannelMutation>()
                    .AddTypeExtension<UserMutation>()
                .AddSubscriptionType(d => d.Name("Subscription"))
                    .AddTypeExtension<MessageSubscriptions>()
                .AddTypeExtension<WorkspaceChannelExtension>()
                .AddInMemorySubscriptions();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSockets();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        }
    }
}
