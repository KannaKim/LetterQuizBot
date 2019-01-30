
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LetterQuizBot
{
    class SensitiveData
    {
        public static string Token { get; set; }
        public static string Id { get; set; }
        public static string CommandPrefix { get; set; }

        static SensitiveData()
        {
            Directory.SetCurrentDirectory(@"C:\Users\barkd\source\repos\LetterQuizBot\LetterQuizBot\");
            Console.WriteLine(Directory.GetCurrentDirectory());
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
