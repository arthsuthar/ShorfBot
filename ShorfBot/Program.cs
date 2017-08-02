using System;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Discord;
using Discord.Commands;

namespace ShorfBot
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Start();
        }

        private DiscordClient _client;

        public void Start()
        {
            _client = new DiscordClient(dc =>
            {
                dc.AppName = "Shorf Bot";
                dc.AppUrl = ConfigurationSettings.AppSettings["AppUrl"];
                dc.LogLevel = LogSeverity.Info;
                dc.LogHandler = Log;
            });

            _client.UsingCommands(uc =>
           {
               uc.PrefixChar = '!';
               uc.AllowMentionPrefix = true;
           });

            var token = ConfigurationSettings.AppSettings["DiscordKey"];

            CreateCommands();          

            var shorfisms = CreateShorfisms();

            _client.MessageReceived += RandomShorfisms;

            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(token);
            });
        }

        public void RandomShorfisms(object sender, MessageEventArgs e)
        {
            int randomNumber = GetRandomNumber();
            var shorfisms = CreateShorfisms();
            if (randomNumber < 8)
            {
                e.Channel.SendMessage(shorfisms[randomNumber]);
            }
        }

        public void CreateCommands()
        {
            var commandService = _client.GetService<CommandService>();
            commandService.CreateCommand("ping")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Seventy-Two-Five!");
                });

            commandService.CreateCommand("cat")
              .Do(async (e) =>
              {
                  await e.Channel.SendMessage(GetCatImage());
              });

            commandService.CreateCommand("puppers")
             .Do(async (e) =>
             {
                 await e.Channel.SendMessage(GetDogImage());
             });

            commandService.CreateCommand("gif").Parameter("message", ParameterType.Multiple)
             .Do(async (e) =>
             {
                 await GetGifWithUsersSearchTerm(e);
             });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Severity}] [{e.Source}]");
        }

        public string[] CreateShorfisms()
        {
            var shorfisms = new string[8];
            shorfisms[0] = "";
            shorfisms[1] = "";
            shorfisms[2] = "";
            shorfisms[3] = "";
            shorfisms[4] = "";
            shorfisms[5] = "";
            shorfisms[6] = "";
            shorfisms[7] = "";
            return shorfisms;
        }


        public string GetCatImage()
        {
            return GetGiphyUrlByTag("cat");
        }

        public string GetDogImage()
        {
            return GetGiphyUrlByTag("dog");           
        }

        private Task GetGifWithUsersSearchTerm(CommandEventArgs e)
        {
            var searchTerm = e.Message.Text.Replace("!gif", string.Empty);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var giphyUrl = GetGiphyUrlByTag(searchTerm.Trim());
                if (!string.IsNullOrEmpty(giphyUrl))
                {
                    return e.Channel.SendMessage(giphyUrl);
                }
            }

            return e.Channel.SendMessage($"Could not find gif for {searchTerm.Trim()}");            
        }

        public string GetGiphyUrlByTag(string tag)
        {
            var giphyImageProvider = new GiphyImageProvider();
            var giphyData = giphyImageProvider.GetRandomGiphyImageData(tag);
            if (!string.IsNullOrEmpty(giphyData.Url))
            {
                return giphyData.Url;
            }
            return string.Empty;
        }
        
        public int GetRandomNumber()
        {
            var serializedQuantumRandomNumber = DownloadSerializedObject("https://qrng.anu.edu.au/API/jsonI.php?length=1&type=uint8");
            if (!string.IsNullOrEmpty(serializedQuantumRandomNumber))
            {
                var quantumRandomNumber = DeserializeQuantumRng(serializedQuantumRandomNumber);
                Console.WriteLine(quantumRandomNumber.data[0]);
                return quantumRandomNumber.data[0];
            }
            return 99;
        }

        public string DownloadSerializedObject(string url)
        {
            var serializedData = string.Empty;

            using (var w = new WebClient())
            {
                try
                {
                    serializedData = w.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return serializedData;
        }


        public QuantumRandomNumber DeserializeQuantumRng(string serializedData)
        {
            return JsonConvert.DeserializeObject<QuantumRandomNumber>(serializedData);
        }

    }
}
