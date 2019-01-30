using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using LetterQuizBot.Modules;
namespace LetterQuizBot
{
    internal class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        public CommandHandler(IServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _services = services;

            _client.MessageReceived += MessageReceivedAsync;
            _commands.CommandExecuted += CommandExecutedAsync;

        }
        public async Task InitializeAsnyc()
        {
            await _commands.AddModuleAsync<PublicModule>(_services);
        }

        public async Task MessageReceivedAsync(SocketMessage msg)
        {
            if (!(msg is SocketUserMessage messege)) return;
            if (messege.Source != MessageSource.User) return;

            var argPos = 0;

            var context = new SocketCommandContext(_client, messege);
            if (! (messege.HasMentionPrefix(_client.CurrentUser, ref argPos)|| 
                   messege.HasStringPrefix((string)DataStorage.GetUserOptionVal(context.User.ToString(), Option.SET_COMMAND_PREFIX), ref argPos)  || messege.HasStringPrefix(SensitiveData.CommandPrefix,ref argPos ))) return;

            await _commands.ExecuteAsync(context, argPos, _services); // we will handle the result in CommandExecutedAsync
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> cmdInfo, ICommandContext context, IResult result)
        { 
            if (cmdInfo.IsSpecified == false)
                return;
            

            if (result.IsSuccess)
                return;

            await context.Channel.SendMessageAsync($"error: {result.ToString()}");
        }
    }
}
