using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Text.RegularExpressions;

namespace LetterQuizBot.Modules
{
    class PublicModule : ModuleBase<SocketCommandContext>
    {
        //secrete command 
        [Command("sendMsg")]
        public async Task sendMsgAsync(params string[] args)
        {
            if (Context.User.ToString() == DataStorage.author)
            {
                ulong channelID = ulong.Parse(args[0]);
                DiscordSocketClient _client = new DiscordSocketClient();
                var _chnl = Context.Client.GetChannel(channelID) as IMessageChannel;
                int argLen = args.Length;
                string result_msg = "";
                foreach (var m in args.Skip(1).Take(argLen))
                {
                    result_msg += m + " ";
                }

                await _chnl.SendMessageAsync(result_msg);
            }
        }

        [Command("사랑해")]
        public async Task helloAsync()
        {
            if (Context.Message.Channel.ToString()[0] == '@')
            {
                await SpecialMsgEventWasInvokedTask("***그 말이 미치도록 듣고싶엇어...\n언젠가 널 만날수 있는날이 올까?***", 5000);
            }
        }
        [Command("ping",RunMode = RunMode.Async)]
        public async Task PingAsync(params string[] option)
        {
            await Context.Channel.SendMessageAsync($"pong!");
        }
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user = user ?? Context.User;
            await ReplyAsync(user.ToString());
        }

        [Command("channelinfo")]
        public async Task ChanelInfoAsync(IChannel channel = null)
        {
            channel = channel ?? Context.Channel;
            await ReplyAsync(channel.ToString());
        }

        [Command("prefix")]
        [Alias("봇부름말")]
        public async Task PrefixAsync(params string[] prefix)
        {
            if (prefix.Length==0)
            {
                await ReplyAsync($"{Context.User.ToString()}\n봇 부름말을 지정해주세요\n{DataStorage.helpText}");
                return;
            }
            else
            {
                if (prefix.Length == 1)
                {
                    if (prefix[0] == "현재")
                    {
                        await ReplyAsync(
                            $"{Context.User.Mention}\n최근 설정하신 봇부름말은 {DataStorage.GetUserOptionVal(Context.User.ToString(), Option.SET_COMMAND_PREFIX)} 입니다.");
                        return;
                    }
                }
                if (prefix.Length == 2)
                {
                    if (prefix[0] == "설정")
                    {
                        string before = DataStorage.GetUserOptionVal(Context.User.ToString(), Option.SET_COMMAND_PREFIX);
                        string after = prefix[1];
                        await ReplyAsync($"{Context.User.Mention}\n" +
                                         $"봇부름말이 {before} 에서 {after}로 바뀌었습니다. 이제 봇은 {after}를 붙이고 부르시면 됩니다.\n" +
                                         $"예: {after}ㅎ");
                        DataStorage.SetUserOptionVal(Context.User.ToString(), Option.SET_COMMAND_PREFIX, after);
                        DataStorage.SaveEntirePairsToJson();
                        return;
                    }
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention}\n잘못된 사용법입니다.\n {DataStorage.helpText}");
                }
  
            }
        }


        [Command("봇초대")]
        public async Task InviteBotLinkAsync()
        {
            await ReplyAsync(DataStorage.botInviteLink+"\n"+
                             "패치노트 그리고 각종 봇에대한 정보를 볼려면: "+DataStorage.patchNoteDiscordLink);
        }


        [Command("help")]
        [Alias("도움말","명령어","도움","명령")]
        public async Task HelpAsync()
        {
            if (new Random().Next(1, 1000) == 1)
            {
                string speicalMsg = $"{Context.User.Mention}\n" +
                                    "***기대고 싶은데가 있다면 내게 기대도록해, 나는 언제든지 여기 있을테니까...\n언제든지....***";
                await SpecialMsgEventWasInvokedTask(speicalMsg, 5000);
            }
            else
            {
                await ReplyAsync(DataStorage.helpText + "\n" + "봇에대한 각종 다양한 소식과 개발자에 삽질현황을 볼려면!" +
                                 DataStorage.patchNoteDiscordLink);
            }
        }

        [Command("leaderboard")]
        [Alias("순위")]
        public async Task LeaderBoardAsync()
        {
            string resultMsg = DataStorage.GetTopnScore(10);// get top 10
            await ReplyAsync(resultMsg);
        }

        [Command("score")]
        [Alias("s", "ㄴ", "내점수")]
        public async Task ScoreAsync()
        {
            int streak = (int)DataStorage.GetUserOptionVal(Context.User.ToString(), Option.CORRECT_STREAK);
            long currentScore = DataStorage.GetUserScore(Context.User.ToString());
            int win = (int)DataStorage.GetUserOptionVal(Context.User.ToString(), Option.WIN);
            int lose = (int)DataStorage.GetUserOptionVal(Context.User.ToString(), Option.LOSE);
            double winRate = Math.Round(win == 0 && lose == 0 ? 0 : (double)win / (win + lose) * 100,2);
            
            string resultMsg = ($"현재 점수 : {currentScore} \n" +
                          $"승: {win} 패: {lose} 승률:{ winRate }%\n");
            if (streak > 1)
                resultMsg += $"**현재 {streak}연승중!!**";
            await ReplyAsync(resultMsg);
        }


        [Command("theme")]
        [Alias("t", "ㅅ","주제")]  
        public async Task ThemeAsync(params string[] option)
        {
            string allTheTheme = "";


            if(option.Length == 1)
            {
                if(option[0] == "c" || option[0] == "현재")
                    await ReplyAsync(Context.User.Mention+"\n현재 주제는 "+DataStorage.GetUserOptionVal(Context.User.ToString(), Option.SET_THEME)+" 입니다.");

                else if(option[0] == "l" || option[0] == "목록")
                {
                    for (int i = 0; i < DataStorage.themeList.Count; i++)
                    {
                        allTheTheme += $"{i+1}. {DataStorage.themeList[i]} ";
                    }
                    await ReplyAsync(Context.User.Mention + "\n"+$"주제를 설정하세요 random 으로 하실꺼면 r 이나 random 을 입력하세요\n{allTheTheme}"); //now index each theme
                }
                else
                {
                    await ReplyAsync(Context.User.Mention + "\n" + "지정되지 않은 옵션입니다.");
                }
            }
            else if (option.Length == 2)
            {
                if (option[0] == "s" || option[0] == "설정")
                {
                    if(int.TryParse(option[1],out int i)) //if indexed
                    {
                        try
                        {
                            DataStorage.SetUserOptionVal(Context.User.ToString(), Option.SET_THEME,
                                DataStorage.themeList[i-1]);
                            await ReplyAsync(Context.User.Mention + "\n" + $"설정 완료, 현재 주제 : {DataStorage.GetUserOptionVal(Context.User.ToString(),Option.SET_THEME)}");
                        }
                        catch (IndexOutOfRangeException e) //user put wrong number
                        {
                            await ReplyAsync(Context.User.Mention + "\n" + "선택하신 주제는 존재하지 않습니다.");
                        }
                    }
                    else if (option[1] == "r" || option[1].ToLower() == "random" || option[1] == "랜덤")
                    {
                        DataStorage.SetUserOptionVal(Context.User.ToString(),Option.SET_THEME,"random");
                        await ReplyAsync(Context.User.Mention + "\n" + $"설정 완료, 현재 주제 : {DataStorage.GetUserOptionVal(Context.User.ToString(), Option.SET_THEME)}");
                    }
                    else
                    {
                        //if string
                        try
                        {
                            string slang = DataStorage.themeToSlang[option[1]];
                            string theme = DataStorage.slangToTheme["kkutu_ko"][slang];
                            DataStorage.SetUserOptionVal(Context.User.ToString(), Option.SET_THEME, theme);
                            await ReplyAsync(Context.User.Mention + "\n" + $"설정 완료, 현재 주제 : {DataStorage.GetUserOptionVal(Context.User.ToString(), Option.SET_THEME)}");
                        }
                        catch (KeyNotFoundException e)
                        {
                            await ReplyAsync(Context.User.Mention + "\n" + "선택하신 주제는 존재하지 않습니다");
                        }
                    }
                    
                }
                else
                {
                   

                    await ReplyAsync(DataStorage.helpText);
                }
            }
            else
            {
                await ReplyAsync(DataStorage.helpText);
            }
        }

        [Command("디스코드")]
        public async Task CreatorDiscordLinkRequestAsync()
        {
            await ReplyAsync(DataStorage.patchNoteDiscordLink);
        }
        [Command("generate")] // generate 자음퀴즈 
        [Alias("g", "ㅎ")]
        public async Task GenerateAsync([Remainder] string user_ans = "")
        {
            //todo: don't let english and ㅇㅅㄴㄴㅁㅇㅅㅇㄴ comes in or disaster happen

            long time_limit = 60; //time limit before you can enter your answer

            if (DataStorage.userData.ContainsKey(Context.User.ToString()) == false)
            {
                DataStorage.AddUserPair(Context.User.ToString());
            }

            string correctAnswer = DataStorage.GetUserOptionVal(Context.User.ToString(), Option.ANSWER);
            string question = DataStorage.GetUserOptionVal(Context.User.ToString(), Option.QUESTION);
            string questionTheme = DataStorage.GetUserOptionVal(Context.User.ToString(), Option.QUESTION_THEME);
            string current_theme = DataStorage.GetUserOptionVal(Context.User.ToString(), Option.SET_THEME);
            

            int reward = (int) DataStorage.GetUserOptionVal(Context.User.ToString(), Option.REWARD);
            int streak = (int) DataStorage.GetUserOptionVal(Context.User.ToString(), Option.CORRECT_STREAK);
            long currentScore = DataStorage.GetUserScore(Context.User.ToString());
            double magicNumber = 5000; //after this point they get no handicap

            bool generated = DataStorage.GetUserOptionVal(Context.User.ToString(), Option.GENERATED);


            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            long currentSeconds = (long) t.TotalSeconds;


            int win = (int) DataStorage.GetUserOptionVal(Context.User.ToString(), Option.WIN);
            int lose = (int) DataStorage.GetUserOptionVal(Context.User.ToString(), Option.LOSE);

            string resultMsg = ""; // discord can't afford to have multiple ReplyAsync Pepega 
            int decrement = currentScore >= 0
                ? (int) Math.Round((reward - streak) / (magicNumber/ currentScore) )
                : 0; 

            if (generated == false) //if g was NOT invoked by user
            {
                Dictionary<string, dynamic> query_data = DataStorage.FetchWord(current_theme);
                 correctAnswer = query_data["_id"].Trim();
                string[] slangs = query_data["theme"].Split(",");
                string problem = "";
                const int MaxhintPercentage = 25; // in percentage
                int hintPercentage = (int)(MaxhintPercentage - currentScore / magicNumber * MaxhintPercentage);

                foreach (char c in correctAnswer)
                { 
                    if (new Random().Next(0,100) <= 25 )
                    {
                        problem += c;
                    }
                    else
                    {
                        problem += KoreanLetterDivider.DevideJaso(c).chars[0];
                    }
                }
                

                generated = true;
                reward = currentScore >= 0 ? (int) Math.Round((correctAnswer.Length * 2) + 20.0 + streak ) : 15;
                question = problem;

                resultMsg += $"문제: {problem}\t힌트:";
                string themeAll = "";
                foreach (string theme_i in DataStorage.GetThemeFromSlangs(slangs))
                {
                    themeAll += theme_i + ",";
                    resultMsg += theme_i + ", ";
                }

                resultMsg = resultMsg.Trim(' ').Trim(',');
                resultMsg += "\n";
                themeAll = themeAll.TrimEnd(','); // exclude that last comma
                 
                
                DataStorage.SetUserOptionVal(Context.User.ToString(),Option.QUESTION_THEME,themeAll);
                DataStorage.SetUserOptionVal(Context.User.ToString(), Option.GENERATED_TIME, currentSeconds);

            }
            else if (generated)
            {
                long time_passed = currentSeconds -
                                   DataStorage.GetUserOptionVal(Context.User.ToString(), Option.GENERATED_TIME);

                if (user_ans == "" || Regex.IsMatch(user_ans, "^[a-zA-Z0-9]*$") || Regex.IsMatch(user_ans, "[!@#$%^&*(),.?\":{ }|<>]") || KoreanLetterDivider.HTable_JungSung.Intersect(user_ans).Count() > 0)
                {
                    if (new Random().Next(1, 10) == 1)
                        await ReplyAsync(Context.User.Mention + "답을 입력하시면되요 :)");
                    else
                        await ReplyAsync(Context.User.Mention + "답을 끼어넣으세요!");
                    return;
                }

                //check if answer is correct
                if (current_theme == "random") //only random go on record
                {
                    if (user_ans == correctAnswer && time_passed < time_limit)
                    {
                        streak += 1;
                        win += 1;
                        currentScore += reward < magicNumber ? (int)((magicNumber-currentScore)/magicNumber*reward): (int)(magicNumber-1);
                        DataStorage.UpdateDataInSQL(Context.User.ToString(), reward);
                        if (streak > 1)
                        {
                            resultMsg += ($"정답입니다!!! +{reward}\n현재까지 {streak} 연승중!!! 연전연승!!\n");
                        }
                        else
                        {
                            resultMsg += ($"정답입니다!!! +{reward}\n");
                        }
                    }

                    else if (time_passed > time_limit)
                    {
                        if (new Random().Next(1, 1000) == 1)
                        {
                            string speicalMsg = $"{Context.User.Mention}\n"+
                            "***가끔씩은 조금 늦어도 괜찮아, 내가 항상 여기서 기다리고 있을테니까...***";
                            await SpecialMsgEventWasInvokedTask(speicalMsg, 5000);
                        }
                        else
                        {
                            currentScore -= decrement;
                            lose += 1;
                            streak = 0;
                            resultMsg +=
                                $"시간초과 ㅠㅠ 이미 {time_passed}초가 경과했습니다. 정답은 {correctAnswer}, 점수 -{decrement}\n시간제한: ({time_limit}초)\n";
                        }
                    }
                    else
                    {
                        // 틀렸어도 유저의 답 자음과 theme 이 데이터베이스에 존재하면 불상하니가 정답처리해주쟝
                        if (new Random().Next(1, 10000) == 1)
                        {
                            string speicalMsg = $"{Context.User.Mention}\n" +
                                                "***틀리지 않았어... 그저 길이 다를뿐...***";
                            await SpecialMsgEventWasInvokedTask(speicalMsg, 3000);

                        }

                        if (DataStorage.ChceckIfSynonym(user_ans, correctAnswer,
                            DataStorage.GetUserOptionVal(Context.User.ToString(), Option.QUESTION_THEME)))
                        {
                            streak += 1;
                            win += 1;
                            currentScore += reward;
                            DataStorage.UpdateDataInSQL(Context.User.ToString(), reward);
                            if (streak > 1)
                            {
                                resultMsg += ($"정답입니다!!! +{reward}\n현재까지 {streak} 연승중!!! 연전연승!!\n");
                            }
                            else
                            {
                                resultMsg += ($"정답입니다!!! +{reward}\n");
                            }
                        }
                        else
                        {
                            currentScore -= decrement;
                            lose += 1;
                            streak = 0;
                            resultMsg += ($"오답입니다! 정답은 {correctAnswer}, 점수 -{decrement}\n");
                        }
                    }
                }

                else if (current_theme != "random") // if not random
                {
                    if (user_ans == correctAnswer && time_passed < time_limit)
                    {
                        resultMsg += ($"정답입니다!!! \n");
                    }
                    else if (time_passed > time_limit)
                    {
                        resultMsg += $"시간초과 ㅠㅠ 이미 {time_passed}초가 경과했습니다. 시간제한: ({time_limit}초)\n";
                    }
                    else
                    {
                        // 틀렸어도 유저의 답 자음과 theme 이 데이터베이스에 존재하면 불상하니가 정답처리해주쟝
                        if (DataStorage.ChceckIfSynonym(user_ans, correctAnswer,
                            DataStorage.GetUserOptionVal(Context.User.ToString(), Option.QUESTION_THEME)))
                        {
                            resultMsg += ($"정답입니다!!! +{reward}\n");
                        }
                        else
                        {
                            resultMsg += ($"오답입니다! 정답은 {correctAnswer}, 점수 -{decrement}\n");
                        }
                 
                    }
                }

                generated = false;
            }

            double winRate = Math.Round(win == 0 && lose == 0 ? 0 : (double) win / (win + lose) * 100, 1);
                    resultMsg += ($"현재 점수 : {currentScore} \n" +
                                  $"승: {win} 패: {lose} 승률:{winRate}%");

                    await ReplyAsync(Context.User.Mention + "\n" + resultMsg);

                    //DataStorage.SetUserOptionVal(Context.User.ToString(), Option.SCORE, currentScore);
                    DataStorage.UpdateDataInSQL(Context.User.ToString(), currentScore);
                    DataStorage.SetUserOptionVal(Context.User.ToString(), Option.QUESTION, question);
                    DataStorage.SetUserOptionVal(Context.User.ToString(), Option.GENERATED, generated);
                    DataStorage.SetUserOptionVal(Context.User.ToString(), Option.ANSWER, correctAnswer);
                    DataStorage.SetUserOptionVal(Context.User.ToString(), Option.CORRECT_STREAK, streak);
                    DataStorage.SetUserOptionVal(Context.User.ToString(), Option.REWARD, reward);
                    DataStorage.SetUserOptionVal(Context.User.ToString(), Option.WIN, win);
                    DataStorage.SetUserOptionVal(Context.User.ToString(), Option.LOSE, lose);


                    DataStorage.SaveEntirePairsToJson();

        }


        public async Task SpecialMsgEventWasInvokedTask(string specialMsg,int delteAfterMillisec)
        { 
            var m = await ReplyAsync(specialMsg);
            Console.WriteLine($"{specialMsg} was invoked to user name: {Context.User.ToString()}");
            await Task.Delay(delteAfterMillisec);
            await Context.Channel.DeleteMessageAsync(m.Id);
        }


        [Command("author")]
        [Alias("제작자")]
        public async Task OwnerInfo()
        {
            await ReplyAsync($"{DataStorage.author}  kimkanna18@gmail.com");
        }

    }
}
