using System;
using System.Threading.Tasks;

namespace LoggingDemo
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log.Error("Hello logging world!");
            log.Debug("Hell logging world!");
            Console.WriteLine("Hit enter");
        }
    }
}
