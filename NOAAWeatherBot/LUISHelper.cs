using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Cognitive.LUIS;
using System.Threading.Tasks;

namespace NOAAWeatherBot
{
    public class LUISHelper
    {
        static LuisClient luisClient;

        public static void Initialize(string appId, string appKey)
        {
            try
            {
                luisClient = new LuisClient(appId, appKey);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public async static Task<LuisResult> Predict(string input)
        {
            try
            {
                return await luisClient.Predict(input);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }

        }
    }
}