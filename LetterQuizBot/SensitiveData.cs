
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
namespace LetterQuizBot
{
    class SensitiveData
    {
        public static string Token { get; set; }
        public static string Id { get; set; }
        public static string CommandPrefix { get; set; }
        public static string workingDirectory { get; set; }
        static SensitiveData()
        {
            workingDirectory= Environment.GetEnvironmentVariable("WorkingDirectory");
            Directory.SetCurrentDirectory(workingDirectory);
            Loggers.log.Info($"Working Directory: {Directory.GetCurrentDirectory()}");
            Token = Environment.GetEnvironmentVariable("LetterQuizBotToken");
            Id = Environment.GetEnvironmentVariable("LetterQuizBotID");
            using (StreamReader sr =  File.OpenText("config.json"))
            {
                string result = sr.ReadToEnd();
                JToken json = JObject.Parse(result);
                //Token = (string)json.SelectToken("token");
                //Id = (string)json.SelectToken("ID");
                CommandPrefix = JsonConvert.DeserializeObject<Dictionary<string,string>>(result)["command_prefix"];
            } 
        }
    }
}
