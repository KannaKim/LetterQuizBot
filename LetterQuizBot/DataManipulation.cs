using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace LetterQuizBot
{
    public class DataManipulation
    {
        public static void UpdateCurrentGuild(SocketGuild sg)
        {

            foreach (var user in sg.Users)
            {
                string username = user.Username + "#" + user.Discriminator;
                if (DataStorage.userData.ContainsKey(username))
                {
                    DataStorage.UpdateGuildID(username, sg.Id);
                }
            }
            
        }
    }
}
