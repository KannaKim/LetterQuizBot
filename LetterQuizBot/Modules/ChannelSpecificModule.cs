﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetterQuizBot.Modules
{
    class ChannelSpecificModule: ModuleBase<SocketCommandContext> // since it's channel specific always add if(Context.IsPrivate==false) unless you have a better way
    {

        [Command("서버내순위")] // 순위 로 바뀔예정
        public async Task ServerRankIndividual()
        {
            if(Context.IsPrivate==false)
                await ReplyAsync(DataStorage.GetTopnScoreInGuild(10, Context.Guild.Id,Context.Guild.Name));
        }
        [Command("서버랭킹")]
        public async Task ServerRanking()
        {
            if (Context.IsPrivate == false)
                await ReplyAsync(DataStorage.GetTopnScoreInGuild(10, Context.Guild.Id, Context.Guild.Name));
        }
    }
}
