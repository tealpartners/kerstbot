using System;
using MargieBot;
using Kerstbot.Responsers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kerstbot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: kerstbot.exe <token>");
                return;
            }

            Console.WriteLine("Kerstbot v1.0");

            var token = args[0];

            var bot = CreateBot();

            var succeeded = await bot.TryConnect(token);
            if (!succeeded)
            {
                Console.WriteLine("Connection failed...");
                return;
            }

            Console.WriteLine("Connected... type 'close' to disconnect.");
            bot.ConnectionStatusChanged += (bool isConnected) =>
            {
                if (!isConnected)
                {
                    Console.WriteLine("Connection lost, trying to reconnect...");
                    //bot.Connect(token);
                }
                else
                {
                    Console.WriteLine("Started...");
                }
            };

            while (Console.ReadLine() != "close")
            {
                bot?.Disconnect();
            }
        }

        private static Bot CreateBot()
        {
            var bot = new Bot();
            var responders = new List<IResponder> {
                new HelloResponder(),
                new GoogleMeetResponder()
            };

            bot.Responders.AddRange(responders);

            return bot;
        }
    }
}
