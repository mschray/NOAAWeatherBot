using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherHelper
{
    public class CityHelper
    {

        static Dictionary<string, CityInfo> cityList = new Dictionary<string,CityInfo>();



        static CityHelper()
        {


            cityList.Add("Chicago".ToUpper(), new CityInfo { CityName = "Chicago", AirportCode = "KORD", ZipCode = "60604" });
            cityList.Add("Milwaukee".ToUpper(), new CityInfo { CityName = "Milwaukee", AirportCode = "KMKE", ZipCode = "53207" });
            cityList.Add("St. Louis".ToUpper(), new CityInfo { CityName = "St. Louis", AirportCode = "KSTL", ZipCode = "63145" });
            cityList.Add("Atlanta".ToUpper(), new CityInfo { CityName = "Atlanta", AirportCode = "KATL", ZipCode = "30303" });

        }

        public static CityInfo FindCity(string city)
        {
            CityInfo cityInfo = null;

            if (cityList.ContainsKey(city.ToUpper()))
            {
                return cityList[city.ToUpper()];
            }
            

            return cityInfo;
        }

    }


    public class CityInfo {
        public string CityName;
        public string AirportCode;
        public string ZipCode;
    }
}
