using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace WeatherHelperLibrary
{
    public static class Extensions
    {
        async public static Task<string> DownloadStringTask(this HttpClient webClient, Uri uri, string userAgent = null)
        {

            // NOAA now requies (as of October 2015) a user-agent string.  I added an option parameter to handle this

            if (userAgent != null)
            {
                //Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko
                //webClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko");
                //webClient.DefaultRequestHeaders.Add("User-Agent", "WeatherManChicago/1.1 (http://www.microsoft.com/en-US/store/Apps/Weather-Man-Chicago/9WZDNCRDF8V6/; martin.schray@hotmail.com)");
                webClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
            }

            string str = await webClient.GetStringAsync(uri);

            return str;
        }
    }
}
