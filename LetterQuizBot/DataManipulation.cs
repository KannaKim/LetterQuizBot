using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace LetterQuizBot
{
    public class DataManipulation
    {
        public static void UpdateCurrentGuild(DiscordSocketClient ds)
        {
            foreach (var guild in ds.Guilds)
            {

                foreach (var user in guild.Users)
                {
                    string username = user.Username + "#" + user.Discriminator;
                    if (DataStorage.userData.ContainsKey(username))
                    {
                        DataStorage.UpdateGuildID(username, guild.Id);
                    }
                }
            }
        }
    }
}
