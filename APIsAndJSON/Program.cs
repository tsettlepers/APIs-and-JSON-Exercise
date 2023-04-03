using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace APIsAndJSON
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var kanye = new RonVSKanyeAPI("https://api.kanye.rest/");
            var ron = new RonVSKanyeAPI("https://ron-swanson-quotes.herokuapp.com/v2/quotes");

            Console.WriteLine("For your entertainment, a conversation between Yeezy and Ron Swanson.");
            for (int i = 0; i<5; ++i)
            {
                Console.WriteLine();
                Console.WriteLine(kanye.Listen());
                Console.WriteLine(ron.Listen());
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Enter a zip code or city,state within the U.S.: ");
            string strLocation = Console.ReadLine();
            try
            {
                var owapi = new OpenWeatherMapAPI(strLocation);
                Console.WriteLine(owapi.CurrentWeather());              
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Weather call failed: " + ex.Message);
            }
        }
    }
}
