using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using WeatherHelperLibrary;
using System.Web;
using Microsoft.Cognitive.LUIS;
using System.Collections.Generic;
using WeatherHelper;

namespace NOAAWeatherBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        
  
        // call LUIS
        //
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            string city = "";

            if (activity.Type == ActivityTypes.Message)
            {

                string appId = Utils.ReadSetting("LUISAppId");
                string appKey = Utils.ReadSetting("LUISAppSecret");
                LUISHelper.Initialize(appId, appKey);
                LuisResult luisResult = await LUISHelper.Predict(activity.Text);

                if (luisResult.Intents.Length > 0)
                {

                    if (luisResult.Entities.Count > 0)
                    {
                        if (luisResult.Entities.ContainsKey("CityName"))
                        {
                            var result = luisResult.Entities["CityName"];
                            city = result[0].Value;
                        }
                    }
                    else
                    {
                        activity.Text = "Not sure what you asked me";
                        await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());

                    }

                    if (string.IsNullOrWhiteSpace(city))
                    {
                        activity.Text ="Could not understand what you asked try using forecast city name";
                        await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                    }
                    else
                    {
                        // default to Chicago
                        WeatherDataHelper weatherDataHelper = new WeatherDataHelper("60604", "http://w1.weather.gov/xml/current_obs/KORD.xml");

                        CityInfo cityInfo = CityHelper.FindCity(city);


                        if (cityInfo != null)
                        {
                            weatherDataHelper = new WeatherDataHelper(cityInfo.ZipCode, $"http://w1.weather.gov/xml/current_obs/{cityInfo.AirportCode}.xml");

                        }

                        Intent intent = luisResult.TopScoringIntent;

                        if (intent.Name.ToUpper() == "forecast".ToUpper())
                        {

                            List<ForecastData> forecastData = await weatherDataHelper.GatherForecastData();
                            string message = $"{city} forecast";

                            foreach (var item in forecastData)
                            {
                                message += $"{item.ForecastDate}-Chance of precipiation={item.ChanceOfPrecip}-High={item.HighTemp}-Low={item.LowTemp}\n";
                            }

                            activity.Text = message;
                            await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                        }
                        else if (intent.Name.ToUpper() == "temperature".ToUpper())
                        {
                            string message = $"Current conditions for {city} ";

                            WeatherInfo weatherInfo = await weatherDataHelper.GatherWeatherData();

                            message += $"Temperature={weatherInfo.Temperature}-the weather is {weatherInfo.WeatherDescription}";

                            //IMessageActivity responseMessage = new Activity(message);
                            activity.Text = message;
                            await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            activity.Text = "Could not understand please ask for temperature city name or forecast city name";
                            await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                        }
                    }

                }

          
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }   

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}