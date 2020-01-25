using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLang
{
    public static class Logger
    {
        public static bool LogEnabled = false;

        public static void Trace(string str)
        {
            if (LogEnabled)
            {
                Console.WriteLine(str);
            }
        }
    }
}
