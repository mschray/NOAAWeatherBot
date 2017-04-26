using System;
using System.Threading.Tasks;
using System.Xml.Linq;
//using Windows.Data.Xml.Dom;
//using Windows.Networking.BackgroundTransfer;
//using Windows.Storage;
//using Windows.UI.Xaml.Media.Imaging;

namespace WeatherHelperLibrary
{

    public struct WeatherInfo
    {
        public string Temperature { get; set; }
        public string WeatherDescription { get; set; }
        //public BitmapImage WeatherImagePath { get; set; }
    }

    public class NOAAWeatherHelper
    {
        public string weatherURL { get; set; }

        public NOAAWeatherHelper(string weatherURL)
        {
            this.weatherURL = weatherURL;
        }

        //            <?xml version="1.0" encoding="ISO-8859-1"?> 
        //<?xml-stylesheet href="latest_ob.xsl" type="text/xsl"?>
        //<current_observation version="1.0"
        //     xmlns:xsd="http://www.w3.org/2001/XMLSchema"
        //     xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
        //     xsi:noNamespaceSchemaLocation="http://www.weather.gov/view/current_observation.xsd">
        //    <credit>NOAA's National Weather Service</credit>
        //    <credit_URL>http://weather.gov/</credit_URL>
        //    <image>
        //        <url>http://weather.gov/images/xml_logo.gif</url>
        //        <title>NOAA's National Weather Service</title>
        //        <link>http://weather.gov</link>
        //    </image>
        //    <suggested_pickup>15 minutes after the hour</suggested_pickup>
        //    <suggested_pickup_period>60</suggested_pickup_period>
        //    <location>Chicago / West Chicago, Dupage Airport, IL</location>
        //    <station_id>KDPA</station_id>
        //    <latitude>41.93</latitude>
        //    <longitude>-88.25</longitude>
        //    <observation_time>Last Updated on Aug 29 2013, 11:52 am CDT</observation_time>
        //        <observation_time_rfc822>Thu, 29 Aug 2013 11:52:00 -0500</observation_time_rfc822>
        //    <weather>Mostly Cloudy</weather>
        //    <temperature_string>82.0 F (27.8 C)</temperature_string>
        //    <temp_f>82.0</temp_f>
        //    <temp_c>27.8</temp_c>
        //    <relative_humidity>63</relative_humidity>
        //    <wind_string>Variable at 3.5 MPH (3 KT)</wind_string>
        //    <wind_dir>Variable</wind_dir>
        //    <wind_degrees>999</wind_degrees>
        //    <wind_mph>3.5</wind_mph>
        //    <wind_kt>3</wind_kt>
        //    <pressure_string>1018.3 mb</pressure_string>
        //    <pressure_mb>1018.3</pressure_mb>
        //    <pressure_in>30.09</pressure_in>
        //    <dewpoint_string>68.0 F (20.0 C)</dewpoint_string>
        //    <dewpoint_f>68.0</dewpoint_f>
        //    <dewpoint_c>20.0</dewpoint_c>
        //    <heat_index_string>85 F (29 C)</heat_index_string>
        //        <heat_index_f>85</heat_index_f>
        //        <heat_index_c>29</heat_index_c>
        //    <visibility_mi>9.00</visibility_mi>
        //    <icon_url_base>http://forecast.weather.gov/images/wtf/small/</icon_url_base>
        //    <two_day_history_url>http://www.weather.gov/data/obhistory/KDPA.html</two_day_history_url>
        //    <icon_url_name>bkn.png</icon_url_name>
        //    <ob_url>http://www.weather.gov/data/METAR/KDPA.1.txt</ob_url>
        //    <disclaimer_url>http://weather.gov/disclaimer.html</disclaimer_url>
        //    <copyright_url>http://weather.gov/disclaimer.html</copyright_url>
        //    <privacy_policy_url>http://weather.gov/notice.html</privacy_policy_url>
        //</current_observation>

        public async Task<WeatherInfo> GetLocalWeather()
        {
            //string url = "http://w1.weather.gov/xml/current_obs/KDPA.xml";
            
            WeatherInfo weatherInfo = new WeatherInfo();
            String xmlHolder;
            try
            {
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "WeatherManChicago/1.1 (http://www.microsoft.com/en-US/store/Apps/Weather-Man-Chicago/9WZDNCRDF8V6/; martin.schray@hotmail.com)");

                    xmlHolder = await WeatherHelperLibrary.Extensions.DownloadStringTask(httpClient, new Uri(weatherURL),
                        "WeatherManChicago/1.1 (http://www.microsoft.com/en-US/store/Apps/Weather-Man-Chicago/9WZDNCRDF8V6/; martin.schray@hotmail.com)");

                }

                XDocument xDoc = XDocument.Parse(xmlHolder);

                if (!string.IsNullOrEmpty(xmlHolder))
                {
                    var currentObs = xDoc.Element("current_observation");
                    string location = currentObs.Element("location").Value;
                    string temp = currentObs.Element("temp_f").Value;
                    string weather = currentObs.Element("weather").Value;
                    string urlBase = currentObs.Element("icon_url_base").Value;
                    string imageName = currentObs.Element("icon_url_name").Value;
                    string fullPathAndFile = string.Format("{0}{1}", urlBase, imageName);

                    weatherInfo.Temperature = temp;
                    weatherInfo.WeatherDescription = weather;
                    //    weatherInfo.WeatherImagePath = await ReadWeatherImage(fullPathAndFile, imageFileName);


                }

            }
            catch (Exception ex)  // rethrow the exception
            {
                
                //System.Diagnostics.
                //Console.WriteLine("{0}Implicitly specified:{0}{1}",ex.Message, ex.StackTrace);

                throw;
            }

            return weatherInfo;
        }

        //public async Task<BitmapImage> ReadWeatherImage(string fullPathAndFile, string imageFileName)
        //{
        //    bool fileExists = false;
        //    StorageFile file = null;

        //    try
        //    {
        //        file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(imageFileName);

        //    }
        //    catch (Exception ex) // deosn't seem to be a good way to check for file exists so we'll use this
        //    {
        //        //Cannot create a file when that file already exists. (Exception from HRESULT: 0x800700B7) = -2147024713
        //        fileExists = true;
        //    }

        //    // the files doesn't already exist so we'll download it
        //    if (!fileExists)
        //    {

        //        try // download the file
        //        {
        //            var downloader = new BackgroundDownloader();
        //            DownloadOperation d = downloader.CreateDownload(new Uri(fullPathAndFile), file);
        //            await d.StartAsync();
        //        }
        //            /* yes for the team being I am ignoring the exception.  I dont' want to clutter the 
        //             code students will be looking at with a bunch of Try Catch right now */
        //        catch (Exception)
        //        {

        //        }
        //    }

        //    return new BitmapImage(new Uri("ms-appdata:///local/" + imageFileName));

        //}

    }
}
