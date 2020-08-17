using plus.enhancer;
using System;
using System.Windows.Forms;

namespace utils.logging
{
    public static class Logger
    {
        public static void log(string logMessage)
        {
            Console.WriteLine("["+ DateTime.Now.ToLongTimeString() + " "+
                DateTime.Now.ToLongDateString() + "] "+ logMessage);
        }
    }
}