using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;


namespace LetterQuizBot
{
    class Program : ModuleBase<SocketCommandContext>
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                await client.SetGameAsync($"명령어를 보려면\t{SensitiveData.CommandPrefix}도움말");
                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;
                

                await client.LoginAsync(TokenType.Bot, SensitiveData.Token);
                await client.StartAsync();
                await services.GetRequiredService<CommandHandler>().InitializeAsnyc();


                await Task.Delay(-1);
            }
            
        }

        private ServiceProvider ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<CommandHandler>();
            return services.BuildServiceProvider();
        }

        private Task LogAsync(LogMessage log)
        {

            Loggers.log.Info(log.ToString());
            return Task.CompletedTask;
        }


    }

}
