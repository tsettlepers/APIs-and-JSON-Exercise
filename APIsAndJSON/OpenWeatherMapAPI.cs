using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIsAndJSON
{
    /*
        {
          "coord": {
            "lon": 10.99,
            "lat": 44.34
          },
          "weather": [
            {
              "id": 501,
              "main": "Rain",
              "description": "moderate rain",
              "icon": "10d"
            }
          ],
          "base": "stations",
          "main": {
            "temp": 298.48,
            "feels_like": 298.74,
            "temp_min": 297.56,
            "temp_max": 300.05,
            "pressure": 1015,
            "humidity": 64,
            "sea_level": 1015,
            "grnd_level": 933
          },
          "visibility": 10000,
          "wind": {
            "speed": 0.62,
            "deg": 349,
            "gust": 1.18
          },
          "rain": {
            "1h": 3.16
          },
          "clouds": {
            "all": 100
          },
          "dt": 1661870592,
          "sys": {
            "type": 2,
            "id": 2075663,
            "country": "IT",
            "sunrise": 1661834187,
            "sunset": 1661882248
          },
          "timezone": 7200,
          "id": 3163858,
          "name": "Zocca",
          "cod": 200
        }      
     */

    public class OpenWeatherMapAPI
    {
        public string TargetLocation { get; set; }
        private float lat=0;
        private float lon=0;
        private HttpClient client;
        private string urlWithParameters="";
        private const string apiKey = "94fb07f3812be9db7fa87f1cc1d370a4";           //Normally we'd keep this in a more secure place!!!!

        public OpenWeatherMapAPI(string locDescr) 
        { 
            TargetLocation = locDescr;
            client = new HttpClient();
            Geocode();
        }

        public string CurrentWeather()
        {
            if (lat==0 || lon==0) 
                throw new Exception("Failed to geocode location '" + TargetLocation + "'.");

            urlWithParameters = $"http://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=imperial&appid={apiKey}";
            var resp = client.GetStringAsync(urlWithParameters).Result;
            var jo = JObject.Parse(resp);

            var formResp = new StringBuilder("");
            formResp.AppendLine("Formatted Weather Output...");
            formResp.AppendLine($"Location: {jo["coord"]["lat"]},{jo["coord"]["lon"]}");
            formResp.AppendLine($"Forecast: {jo["weather"][0]["main"]} ({jo["weather"][0]["description"]})");
            formResp.AppendLine($"Temperature: {jo["main"]["temp"]} degrees, feels like {jo["main"]["feels_like"]} degrees");
            formResp.AppendLine($"Pressure: {jo["main"]["pressure"]} hPa");
            formResp.AppendLine($"Humidity: {jo["main"]["humidity"]}%");

            return formResp.ToString();
        }

        private void Geocode()
        {
            if (string.IsNullOrEmpty(TargetLocation))
            {
                throw new Exception("Empty location provided. Unable to geocode.");
            }
            else if (TargetLocation.Length == 5 && TargetLocation.All(char.IsNumber))         
            {
                // Looks like a zip code, which returns a single object
                try
                {
                    urlWithParameters = $"http://api.openweathermap.org/geo/1.0/zip?zip={TargetLocation},US&appid={apiKey}";
                    var resp = client.GetStringAsync(urlWithParameters).Result;
                    var jo = JObject.Parse(resp);
                    lat = (float)jo.SelectToken("lat");
                    lon = (float)jo.SelectToken("lon");
                }
                catch (Exception ez) 
                {
                    throw new Exception("Failed to geocode the zip: " + ez.Message);
                }
            }
            else if (TargetLocation.Contains(','))                                          
            {
                // Looks like a city, which returns an array of objects
                try
                {
                    string[] cityst = TargetLocation.Split(",");
                    urlWithParameters = $"http://api.openweathermap.org/geo/1.0/direct?q={cityst[0]},{cityst[1]},US&limit={5}&appid={apiKey}";
                    var resp = client.GetStringAsync(urlWithParameters).Result;
                    var ja = JArray.Parse(resp);
                    if (ja.Count != 1)
                        throw new Exception("Ambiguous city name cannot be resolved.");
                    else
                    {
                        lat = (float)ja[0]["lat"];
                        lon = (float)ja[0]["lon"];
                    }
                }
                catch (Exception ec)
                {
                    throw new Exception("Failed to geocode the city: " + ec.Message);
                }
            }
            else
            {
                throw new Exception("Location looks like neither a zip or city,state. Unable to geocode.");
            }
        }
    }
}
