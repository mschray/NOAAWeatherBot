using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherHelperLibrary
{
    public class WeatherDataHelper
    {
        // Hotel zip code is 30303 and Hatsfield airport is KATL
        NOAAForecastHelper noaaForecastHelper;
        NOAAWeatherHelper noaaWeatherHelper;

        public WeatherDataHelper(string zipCode, string weatherUrl)
        {
            noaaForecastHelper = new NOAAForecastHelper(zipCode);
            noaaWeatherHelper = new NOAAWeatherHelper(weatherUrl);

        }
        async public Task<WeatherInfo> GatherWeatherData()
        {

            WeatherInfo weatherInfo = await noaaWeatherHelper.GetLocalWeather();

            return weatherInfo;
        }


        async public Task<List<ForecastData>> GatherForecastData()
        {
            await noaaForecastHelper.CallNOAAForecastService();
            //string test = "<temperature type=\"maximum\" units=\"Fahrenheit\" time-layout=\"k-p24h-n7-1\"> <name>Daily Maximum Temperature</name>  <value>6</value>  <value>24</value>  <value>23</value>  <value>23</value>  <value>2</value>  <value>8</value>  <value>13</value></temperature>";

            List<ForecastData> forecastList = new List<ForecastData>();

            for (int i = 0; i < 5; i++)
            {
                ForecastData day = new ForecastData
                {
                    ForecastDate = noaaForecastHelper.DateTimeFromTimeSeries(noaaForecastHelper.TimeSeries24Hour[i]).ToString("ddd MMMM dd"),
                    HighTemp = noaaForecastHelper.HighTemperatures[i],
                    LowTemp = noaaForecastHelper.LowTemperatures[i],
                    ChanceOfPrecip = noaaForecastHelper.ProbabilityOfPrecipitation[i]

                };

                forecastList.Add(day);

            }

            return forecastList;
        }
    }
}
