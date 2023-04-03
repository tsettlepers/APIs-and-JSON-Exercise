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
    public class RonVSKanyeAPI
    {
        public string URL { get; set; }

        internal HttpClient client;

        public RonVSKanyeAPI(string url) 
        {
            URL = url;
            client = new HttpClient();
        }

        public string Listen()
        {
            var resp = client.GetStringAsync(URL).Result;

            try
            {
                var jo = JObject.Parse(resp);
                return "KANYE: " + jo.SelectToken("quote").ToString();
            }
            catch (Exception ex)
            {
                return "RON: " + resp.Replace("[", "").Replace("]", "").Replace("{", "").Replace("]", "").Replace("\"","");
            }
        }
    }
}
