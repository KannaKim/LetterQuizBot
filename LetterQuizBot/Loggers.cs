using System;
using System.Collections.Generic;
using System.Text;

namespace LetterQuizBot
{
    public class Loggers
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Loggers()
        { }
    }
}
