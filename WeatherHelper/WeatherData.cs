using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherHelperLibrary
{
    public class WeatherData
    {
        public string Temperature { get; set; }
        public string WeatherDescription { get; set; }
        public string[] ForecastDay1 { get; set; }
    }
}
