using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Linq;

namespace WeatherHelperLibrary
{
    public sealed class NOAAForecastHelper
    {
        public string[] HighTemperatures { get; private set; }
        public string[] LowTemperatures { get; private set; }
        public string[] ProbabilityOfPrecipitation { get; private set; }
        public string[] TimeSeries24Hour { get; private set; }
        public string[] TimeSeries12Hour { get; private set; }
        public string Zipcode { get; set; }

        public NOAAForecastHelper(string zipCode)
        {
            this.Zipcode = zipCode;
        }

        public DateTime DateTimeFromTimeSeries(string timeSeries)
        {

            // example format is 2013-10-01T
            string year = timeSeries.Substring(0, 4);  // get the year
            string month = timeSeries.Substring(5, 2); // get the month
            string day = timeSeries.Substring(8, 2);   // get the date

            return new DateTime(Convert.ToInt16(year), Convert.ToInt16(month), Convert.ToInt16(day));
        }

        public string BuildRequestURLZipCode()
        {
            string noaaRequestURIBeforeZip = "http://graphical.weather.gov/xml/SOAP_server/ndfdXMLclient.php?whichClient=NDFDgenMultiZipCode&lat=&lon=&listLatLon=&lat1=&lon1=&lat2=&lon2=&resolutionSub=&listLat1=&listLon1=&listLat2=&listLon2=&resolutionList=&endPoint1Lat=&endPoint1Lon=&endPoint2Lat=&endPoint2Lon=&listEndPoint1Lat=&listEndPoint1Lon=&listEndPoint2Lat=&listEndPoint2Lon=&zipCodeList=";
            string noaaRequestURIAfterZip = "&listZipCodeList=&centerPointLat=&centerPointLon=&distanceLat=&distanceLon=&resolutionSquare=&listCenterPointLat=&listCenterPointLon=&listDistanceLat=&listDistanceLon=&listResolutionSquare=&citiesLevel=&listCitiesLevel=&sector=&gmlListLatLon=&featureType=&requestedTime=&startTime=&endTime=&compType=&propertyName=&product=time-series&begin=2004-01-01T00%3A00%3A00&end=2020-02-08T00%3A00%3A00&Unit=e&maxt=maxt&mint=mint&pop12=pop12&temp=temp&wx=wx&icons=icons&Submit=Submit";
            return string.Format("{0}{1}{2}", noaaRequestURIBeforeZip, Zipcode, noaaRequestURIAfterZip);
        }

        public async Task<string> CallNOAAForecastService()
        {
            //    // get the URL building in the zip code
            string noaaRequestURI = BuildRequestURLZipCode();

            var xmlHolder = await Extensions.DownloadStringTask( new System.Net.Http.HttpClient(), new Uri(noaaRequestURI));
            XDocument xDoc = XDocument.Parse(xmlHolder);

            if (!string.IsNullOrEmpty(xmlHolder))
            {
                var dwml = xDoc.Element("dwml");
                var data = dwml.Element("data");
                var parameters = data.Element("parameters");
            
                // get the high temperatures
                var temperatures = parameters.Element("temperature");
                HighTemperatures = GetNodeListValues(temperatures.ToString(), "temperature", "value");
                                
                // do the work to get to the low temperatures
                var lowTemperatures = parameters.Elements("temperature").InDocumentOrder();
                var lowEnum = lowTemperatures.GetEnumerator();
                lowEnum.MoveNext(); // go past high temps
                lowEnum.MoveNext(); // now move to low temps

                var lowTempXML = lowEnum.Current;  // get that low temp

                LowTemperatures = GetNodeListValues(lowTempXML.ToString(), "temperature", "value");

                //get the probability of precipitation (which happens arrnaged in the 12 hour time format)
                var probabiltyOfPrec = parameters.Element("probability-of-precipitation");
                ProbabilityOfPrecipitation = GetNodeListValues(probabiltyOfPrec.ToString(), "probability-of-precipitation", "value");

                // get the 24 hour time time layout
                var timeLayout = data.Element("time-layout");
                TimeSeries24Hour = GetNodeListValues(timeLayout.ToString(), "time-layout","start-valid-time");

                var timeLayout12Hour = data.Elements("time-layout").InDocumentOrder();
                var time12Enum = timeLayout12Hour.GetEnumerator();
                time12Enum.MoveNext(); // goto 1st layout
                time12Enum.MoveNext(); // goto 2nd layout
                time12Enum.MoveNext(); // goto 3rd layout

                var time12XML = time12Enum.Current;  // Grab that 3rd layout

                TimeSeries12Hour = GetNodeListValues(time12XML.ToString(), "time-layout", "start-valid-time");

            }

            return xmlHolder;
        }

        public string[] GetNodeListValues(string xmlChunk, string topLevel ,string value)
        {
            List<string> values = new List<string>();

            XDocument xDoc = XDocument.Parse(xmlChunk);
            var outerBlock = xDoc.Element(topLevel);
            var innerBlock = outerBlock.Elements(value);
            var blockIterator = innerBlock.GetEnumerator();
            while (blockIterator.MoveNext())
            {
                values.Add(blockIterator.Current.Value);
            }

            return values.ToArray();
        }

    }
}