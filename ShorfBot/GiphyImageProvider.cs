using System;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;

namespace ShorfBot
{
    public class GiphyImageProvider : IGiphyImageProvider
    {
        private string GiphyEndpoint { get; set; }

        private string ApiKey { get; set; }

        public GiphyData GetRandomGiphyImageData(string tags)
        {
            GiphyEndpoint = ConfigurationSettings.AppSettings["GiphyEndpoint"];

            ApiKey = ConfigurationSettings.AppSettings["GiphyKey"];

            string response = GetJsonResponse(GetFullGiphyEndpoint(tags));

            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    var giphy = JsonConvert.DeserializeObject<GiphyResponse>(response);
                    if (giphy != null && giphy.Meta != null && giphy.Meta.Status == 200 && giphy.Data != null)
                    {
                        return giphy.Data;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            return new GiphyData();           
        }

        protected virtual string GetJsonResponse(string requestUrl)
        {
            try
            {
                WebRequest request = HttpWebRequest.Create(requestUrl);
                request.Method = "GET";
                string json;
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return string.Empty;
        }

        protected virtual string GetFullGiphyEndpoint(string tags)
        {
            return string.Format(GiphyEndpoint, ApiKey, Uri.EscapeDataString(tags));
        }



    }
}
