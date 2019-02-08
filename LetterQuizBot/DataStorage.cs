using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CommandLine.Text;
using Discord.Commands;
using Newtonsoft.Json;
using Npgsql;

namespace LetterQuizBot
{
    public class DataStorage
    {
        public static readonly string userDataPath = Path.Join("UserData", "UserInfo.json");
        public static readonly string jsonLangPath = Path.Join("lang", "slangToTheme.json");
        public static readonly string commandHelpTextPath = Path.Join("doc", "CommandHelpText.txt");
        public static readonly string patchNoteDiscordLink = "https://discord.gg/vGfXwJM";

        public static readonly string botInviteLink =
            "https://discordapp.com/api/oauth2/authorize?client_id=535330750019665921&permissions=0&scope=bot";

        public static readonly string connString = "Host=localhost;Username=postgres;Password=" + Environment.GetEnvironmentVariable("PostresPassword") + ";Database=main";
        public static Dictionary<string, Dictionary<string, dynamic>> userData { get; private set; }
        public static Dictionary<string, Dictionary<string, string>> slangToTheme; // kkutu language data

        public static Dictionary<string, string> themeToSlang;
        public static List<string> themeList = new List<string>();

        public static string helpText;

        public static readonly string author = "Kanna Kim#4156";

        static DataStorage()
        {
         
            userData = GetUserDataFromJason();
            slangToTheme = GetLangDataFromJason();

            themeToSlang = GetThemeToSlang();
            themeList = GetThemeList();
            helpText = GetHelpText();

        }
        private static string GetHelpText()
        {
            string helpText = "";

            using (StreamReader sr = File.OpenText(DataStorage.commandHelpTextPath))
            {
                helpText = sr.ReadToEnd();
            }

            return helpText;
        }


        private static List<string> GetThemeList()
        {
            List<string> a = new List<string>();
            foreach (KeyValuePair<string, string> entry in DataStorage.slangToTheme["kkutu"])
            {
                string slang = entry.Key;
                string theme = entry.Value;
                a.Add(theme);
            }
            return a;
        }

        public static Dictionary<string, string> GetCommandHelpText()
        {
            string content = "";
            using (StreamReader sr = File.OpenText(commandHelpTextPath))
            {
               content = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        }

        private static Dictionary<string, string> GetThemeToSlang()
        {
            Dictionary<string, string> tts = new Dictionary<string, string>();
            foreach (KeyValuePair<string,string> entry in slangToTheme["kkutu"])
            {
                string slang = entry.Key.Replace("theme_","");
                tts.Add(entry.Value,slang);
            }
            return tts;
        }
        private static Dictionary<string, Dictionary<string, dynamic>> GetUserDataFromJason()
        {
            string userData = "";
            if (File.Exists(userDataPath) == false)
            {
                File.WriteAllText(userDataPath, "");
            }
            using (StreamReader sr = File.OpenText(userDataPath))
            {
                userData = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(userData);
        }
        public static Dictionary<string, Dictionary<string, string>> GetLangDataFromJason()
        {
            string data = "";
            using (StreamReader sr = File.OpenText(jsonLangPath))
            {
                data = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(data);
        }
        

        public static List<string> GetThemeFromSlangs(string[] theme)
        {
            List<string> slangs = new List<string>();
            foreach(string theme_str in theme)
            {
                if(theme_str != "0")
                {
                    slangs.Add(slangToTheme["kkutu"][$"theme_{theme_str}"]);
                }
            }

            return slangs;
        }

        public static bool ChceckIfSynonym(string userAns, string actualAns, string actualTheme)
        {
            string actualAnsJaum = "";
            for (int i = 0; i < actualAns.Length; i++)
            {
                actualAnsJaum += KoreanLetterDivider.DevideJaso(actualAns[i]).chars[0];
            }

            string userAnsJaum = "";
            string[] userAnsSlang = null;

            for (int i = 0; i < userAns.Length; i++)
            {
                userAnsJaum += KoreanLetterDivider.DevideJaso(userAns[i]).chars[0];
            }

            string slangsAll = "";
            foreach (string theme in actualTheme.Split(','))
            {
                slangsAll += themeToSlang[theme] + ",";
            }
            slangsAll = slangsAll.TrimEnd(',');
            string[] actualAnsSlang = slangsAll.Split(',');

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sql_statement = $"select _id,theme from public.kkutu_ko where _id='{userAns}'";

                using (var cmd = new NpgsqlCommand(sql_statement))
                {
                    cmd.Connection = conn;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    Loggers.log.Info(cmd.CommandText);

                    using (var reader = cmd.ExecuteReader()) 
                    {
                        while(reader.Read())
                        {
                            // reader.GetString(1) = score
                            userAnsSlang = reader.GetString(1).Trim().Split(',');
                        }

                        if (userAnsSlang == null) // meaning no db exist 
                            return false;
                    }
                }
                ///after checking user theme that may exist or not
            }

            if (userAnsJaum == actualAnsJaum)    //check if user has that 자음  too and if so
            {
                if (userAnsSlang.Intersect(actualAnsSlang).Count() > 0) // check if themes intersects
                {
                    return true;
                }
                else
                {
                    return false;
                }
             }
            else
            {
                return false;
            }
        }
        public static Dictionary<string, dynamic> GetUserData(string userNameTag)
        {
            if (userData.ContainsKey(userNameTag))
            {
                return userData[userNameTag];
            }
            else
            {
                AddUserPair(userNameTag);
                return userData[userNameTag];
            }
        }

        public static void SetUserOptionVal(string userNameTag, int option, dynamic value)
        {
            userData[userNameTag][Option.optionList[option]] = value;
        }
        public static dynamic GetUserOptionVal(string userNameTag, int option)  //use Option class 
        {
            string op = Option.optionList[option];  // this is not the best way to get an option, revise require in the future 
            if (userData.ContainsKey(userNameTag)) // want to make sure option is from user-specified feild      
            {
                try
                {
                    return userData[userNameTag][op]; 
                }
                catch (KeyNotFoundException e)  //when user not exist setup user account
                {
                    userData[userNameTag].Add(op, Option.defaultOptionValue[op]);
                    return userData[userNameTag][op];
                }
            }
            else
            {
                throw new KeyNotFoundException(string.Format($"unable to find userNameTag : {userNameTag}"));
            }
            
        }

        public static void AddUserPair(string userNameTag)
        {
            userData.Add(userNameTag,Option.defaultOptionValue);
            Loggers.log.Info("Adding User Pair");
        }


        public static void SaveEntirePairsToJson()
        {
            string output = JsonConvert.SerializeObject(userData,Formatting.Indented);

            File.WriteAllText(userDataPath,output);
            Loggers.log.Info("Saving Entire Pair");
        }

        public static void RegisterUserInSQL(SocketCommandContext context)
        {
            string userName = context.User.ToString();
            if (context.IsPrivate)
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    // Insert some data
                    string sql_statement =
                        $"insert into public.leaderboard(username) values ('{userName}')";
                    using (var cmd = new NpgsqlCommand(sql_statement))
                    {
                        cmd.Connection = conn;
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        Loggers.log.Info(sql_statement);
                    }
                }
            }
            else
            {
                ulong GuildID = context.Guild.Id;
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    // Insert some data
                    string sql_statement =
                        $"insert into public.leaderboard(username, Guildid) values ('{userName}','{{{GuildID}}}')";
                    using (var cmd = new NpgsqlCommand(sql_statement))
                    {
                        cmd.Connection = conn;
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        Loggers.log.Info(sql_statement);
                    }
                }
            }
        }

        public static void UpdateGuildID(string userName,ulong GuildID)  
        {

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string check_statement = $"select Guildid from public.leaderboard where username='{userName}' and {GuildID} = any(Guildid)";
                bool GuildIDExist;
                using (var cmd = new NpgsqlCommand(check_statement))
                {
                    cmd.Connection = conn;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    using (var reader = cmd.ExecuteReader())
                    {
                        GuildIDExist = reader.HasRows;
                    }
                    Loggers.log.Info($"GuildIDExist: {GuildIDExist}");
                }

                if (GuildIDExist==false)
                {
                    // Insert some data
                    string sql_statement =
                        $"update public.leaderboard set GuildID = array_append(GuildID, {GuildID}) where username = '{userName}'";
                    using (var cmd = new NpgsqlCommand(sql_statement))
                    {
                        cmd.Connection = conn;
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void RegisterUserData(SocketCommandContext context)
        {
            AddUserPair(context.User.ToString());
            SaveEntirePairsToJson();
            RegisterUserInSQL(context);
        }

        public static bool CheckIfGuildIDExist(SocketCommandContext context)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                // Insert some data
                string check_statement =
                    $"select guildID from public.leaderboard where username='{context.User.ToString()}' and {context.Guild.Id} = any(guildID)";
                bool guildIDExist;
                using (var cmd = new NpgsqlCommand(check_statement))
                {
                    cmd.Connection = conn;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    using (var reader = cmd.ExecuteReader())
                    {
                        guildIDExist = reader.HasRows;
                    }

                    Loggers.log.Info($"guildIDExist: {guildIDExist}");
                }

                return guildIDExist;
            }
        }
        public static void UpdateScoreInSQL(SocketCommandContext context,long currentScore)        // theme to slang and put it in command
        {
            string userName = context.User.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sql_statement;
                if (context.IsPrivate) //if DM 
                {
                    sql_statement = $"update public.leaderboard set score = {currentScore} where username='{userName}'";
                }
                else
                {
                    bool guildIDExist = CheckIfGuildIDExist(context);
                    if(guildIDExist) // that Guild already exist
                    {
                        sql_statement =
                            $"update public.leaderboard set score = {currentScore} where username='{userName}'";

                    }

                    else // if Guild ID not exist in array
                    {
                        sql_statement =
                            $"update public.leaderboard set score = {currentScore},guildID = array_append(guildID, {context.Guild.Id}::bigint) where username='{userName}'";
                    }
                }

                using (var cmd = new NpgsqlCommand(sql_statement))
                {
                    cmd.Connection = conn;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    Loggers.log.Info(sql_statement);
                }

            }
        }


        public static long GetUserScore(SocketCommandContext context)   // assuming msg is from channel 
        {
            long currentScore= 0;
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                // Insert some data
                string sql_statement = $"select * from public.leaderboard where username='{context.User.ToString()}'";

                    using (var cmd = new NpgsqlCommand(sql_statement))
                    {
                        cmd.Connection = conn;
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        Loggers.log.Info(cmd.CommandText);

                        var reader = cmd.ExecuteReader();
                        
                        if (reader.HasRows ==false)
                        {
                            reader.Close();
                            RegisterUserInSQL(context);
                          
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                            reader = cmd.ExecuteReader();
                            
                        }

                        while (reader.Read())
                        {
                            // reader.GetString(1) = score
                            currentScore = reader.GetInt64(1);
                        }   
                    }                
            }
            return currentScore;
        }


        [Obsolete] //too slow, better approach needed
        public static int[] GetUniqueRandomArray(int min, int max, int size)
        {
            int[] ret = new int[size];
            for (int i = 0; i < size; i++)
            {
                bool unique = false;
                while (unique == false)
                {
                    int rIndex = new Random().Next(min, max);
                    if (ret.Contains(rIndex) == false)
                    {
                        ret[i] = rIndex;
                        unique = true;
                    }
                }
            }

            return ret;
        }

        public static string GetMyScoreInGuild(int n, ulong guildID, string userName,string guildName) // 내 랭킹을 중심으로 사람들을 보여줌 
        { 
            string resultMsg = null;
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                // Insert some data
                string sql_statement = $"with user_rank"+
                $" as (select username, score, guildid, row_number() over(order by score desc) from public.leaderboard where {guildID}=any(guildid)),"+
                $" user_rank_var(a)"+
                $" as(select row_number from user_rank where username='{userName}')"+
                $" select* from user_rank where user_rank.row_number between(select a-{n} from user_rank_var) and(select a+{n} from user_rank_var)";
                using (var cmd = new NpgsqlCommand(sql_statement))
                {
                    cmd.Connection = conn;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    Loggers.log.Info(cmd.CommandText);
                    resultMsg = $"```md\n#{guildName} 서버 내 랭킹\n";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // reader.GetString(1) = score
                            int win = (int)DataStorage.GetUserOptionVal(reader.GetString(0), Option.WIN);
                            int lose = (int)DataStorage.GetUserOptionVal(reader.GetString(0), Option.LOSE);
                            double winrate = Math.Round(win == 0 && lose == 0 ? 0 : (double)win / (win + lose) * 100, 2);
                            resultMsg += $"{reader.GetInt64(3)}. {reader.GetString(0)} {reader.GetInt64(1)}점 ({win}승 {lose}패 승률:{winrate}%)\n";
                        }
                    }

                    resultMsg += "```";
                }
            }
            return resultMsg;
        }
        public static string GetTopnScoreInGuild(int n, ulong guildID, string GuildName) 
        {
            string resultMsg = null;
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                // Insert some data
                string sql_statement = $"select * from public.leaderboard where {guildID} = any(Guildid) order by score desc limit {n}";
                using (var cmd = new NpgsqlCommand(sql_statement))
                {
                    cmd.Connection = conn;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    Loggers.log.Info(cmd.CommandText);
                    resultMsg = $"```md\n#{GuildName} 서버 내 랭킹\n";
                    using (var reader = cmd.ExecuteReader())
                    {
                        int i = 1;
                        while (reader.Read())
                        {
                            // reader.GetString(1) = score
                            int win = (int)DataStorage.GetUserOptionVal(reader.GetString(0), Option.WIN);
                            int lose = (int)DataStorage.GetUserOptionVal(reader.GetString(0), Option.LOSE);
                            double winrate = Math.Round(win == 0 && lose == 0 ? 0 : (double)win / (win + lose) * 100, 2);
                            resultMsg += $"{i}. {reader.GetString(0)} {reader.GetInt64(1)}점 ({win}승 {lose}패 승률:{winrate}%)\n";
                            i += 1;
                        }
                    }

                    resultMsg += "```";
                }
            }
            return resultMsg;
        }
        public static string GetTopnScore(int n)
        {
            string resultMsg = null;
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                // Insert some data
                string sql_statement= $"select * from public.leaderboard order by score desc limit {n}";
                using (var cmd = new NpgsqlCommand(sql_statement))
                {
                    cmd.Connection = conn;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    Loggers.log.Info(cmd.CommandText);
                    resultMsg = "```md\n";
                    using (var reader = cmd.ExecuteReader())
                    {
                        int i = 1;
                        while (reader.Read())
                        {
                            // reader.GetString(1) = score
                            int win = (int)DataStorage.GetUserOptionVal(reader.GetString(0), Option.WIN);
                            int lose = (int)DataStorage.GetUserOptionVal(reader.GetString(0), Option.LOSE);
                            double winrate = Math.Round(win == 0 && lose == 0 ? 0 : (double)win / (win + lose) * 100, 2);
                            resultMsg += $"{i}. {reader.GetString(0)} {reader.GetInt64(1)}점 ({win}승 {lose}패 승률:{winrate}%)\n";
                            i += 1;
                        }
                    }

                    resultMsg += "```";
                }
            }
            return resultMsg;
        }


        //sql 
        public static Dictionary<string,dynamic> FetchWord(string theme)        // theme to slang and put it in command
        {    
            Dictionary<string,dynamic> re =  new Dictionary<string, dynamic>();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                // Insert some data
                string sql_statement;
                if (theme.ToLower() == "random")
                {
                    sql_statement =//"select * from public.kkutu_ko where _id like '평촌역'" //debug
                    "select * from public.kkutu_ko where theme not like '%0%' order by RANDOM() limit 1";
                }
                else
                {
                    string slang = themeToSlang[theme];
                    sql_statement = /*"select * from public.kkutu_ko where _id like '광명역'"*/
                        $"select * from public.kkutu_ko where theme not like '%0%'and theme like '%{slang}%' order by random() limit 1"; 
                }
                using (var cmd = new NpgsqlCommand( sql_statement ))
                {
                    cmd.Connection = conn;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    Loggers.log.Info(cmd.CommandText);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            re.Add("_id", reader.GetString(0));
                            re.Add("type", reader.GetString(1));
                            re.Add("mean", reader.GetString(2));
                            re.Add("hit", reader.GetInt32(3));
                            re.Add("flag", reader.GetInt32(4));
                            re.Add("theme", reader.GetString(5));

                            string _id, _type, _mean, _hit,_flag,_theme;
                            _id = reader.GetString(0);
                            _type = reader.GetString(1);
                            _mean = reader.GetString(2);
                            _hit = reader.GetInt32(3).ToString();
                            _flag = reader.GetInt32(4).ToString();
                            _theme = reader.GetString(5);



                            Loggers.log.Info($"_id: {_id} type: {_type} mean: {_mean} hit: {_hit} flag: {_flag} theme: {_theme}");
                        }
                    }
                }

                return re;
            }
        }



    }
}
