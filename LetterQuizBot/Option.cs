using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Xml.Schema;

namespace LetterQuizBot
{
    public class Option 
    {
        //SYSTEM VARIABLE
        public const int CORRECT_STREAK = 1;
        public const int GENERATED = 2;
        public const int ANSWER = 3;
        public const int REWARD = 4;
        public const int QUESTION = 5;
        public const int WIN = 6;
        public const int LOSE = 7;
        public const int GENERATED_TIME = 8; // time when user hit !generated
        public const int QUESTION_THEME = 9;

        //USER-SET VARIABLE
        public const int SET_THEME = 800;
        public const int SET_COMMAND_PREFIX = 801;

        public static readonly Dictionary<int,string> optionList = new Dictionary<int, string>()
            {{CORRECT_STREAK,"correct_streak"},{GENERATED,"generated"},{ANSWER,"answer"},
                { REWARD,"reward"},{QUESTION,"question"},{WIN,"win"},{LOSE,"lose"}, {SET_THEME,"set_theme"},
                {GENERATED_TIME,"generated_time"},{QUESTION_THEME,"question_theme"},{SET_COMMAND_PREFIX,"set_command_prefix"}
            };


        public static readonly Dictionary<string, dynamic> defaultOptionValue= new Dictionary<string, dynamic>()
        {
            {optionList[CORRECT_STREAK], 0 },
            {optionList[GENERATED], false },
            {optionList[ANSWER],""},
            {optionList[REWARD],0},
            {optionList[QUESTION],""},
            {optionList[WIN],0},
            {optionList[LOSE],0},
            {optionList[SET_THEME],"random"},
            {optionList[GENERATED_TIME],0},
            {optionList[QUESTION_THEME],""},
            {optionList[SET_COMMAND_PREFIX],SensitiveData.CommandPrefix}
        };
    }
}
