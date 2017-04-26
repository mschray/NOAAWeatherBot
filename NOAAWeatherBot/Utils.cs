using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NOAAWeatherBot
{
    /// <summary>
    /// Various utilities used in this sample app
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Read App Setting from file
        /// </summary>
        /// <param name="key">key for the parameter</param>
        /// <returns></returns>
        public static string ReadSetting(string key)
        {
            string result = "";

            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "Not Found";
                Console.WriteLine(result);

            }
            catch (ConfigurationErrorsException ex)
            {
                Console.WriteLine($"{Utils.FormatExceptionMessage(ex)}");
            }

            return result;
        }

        /// <summary>
        /// Given an exception gather information on class and function and prepend to the exception message and stacktrakce.  This
        /// funtion doesn't address nested exceptions.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string FormatExceptionMessage(Exception ex)
        {


            StackTrace st = new StackTrace();

            // get the frame of the caller
            MethodBase methodBase = st.GetFrame(1).GetMethod();

            string methodName = methodBase.Name;
            string className = methodBase.DeclaringType.Name;

            return $"Class:{className} Function:{methodBase} has exception:{ex.Message} and stacktrace {ex.StackTrace}";
        }


        public static string[] GetCallerInfo()
        {
            StackTrace st = new StackTrace();

            // get the frame of the caller
            MethodBase methodBase = st.GetFrame(1).GetMethod();

            string methodName = methodBase.Name;
            string className = methodBase.DeclaringType.Name;

            // return the methodName and the className
            string[] result = { methodName, className };

            return result;
        }
    }
}
