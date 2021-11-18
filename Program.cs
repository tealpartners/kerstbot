using System;
using MargieBot;
using Kerstbot.Responsers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;

namespace Kerstbot;

class Program
{
    private static string _token;
    private static bool _running;

    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: kerstbot.exe <token>");
            return;
        }

        Console.WriteLine("Kerstbot v1.2");

        _token = args[0];

        var replyResponder = new WittyReplyResponder();
        var bot = CreateBot(replyResponder);
        _running = await bot.TryConnect(_token);

        if (!_running)
        {
            Console.WriteLine("Connection failed...");
            return;
        }

        var tokenSource = new CancellationTokenSource();
        var consoleTask = Task.Run(() => ConsoleTask(bot, tokenSource));
        var connectionTask = Task.Run(() => ConnectionWatcher(bot, tokenSource.Token));
        var disconnectTask = Task.Run(() => DisconnectTask(bot, tokenSource.Token));
        var messageTask = Task.Run(() => MessageTask(bot, tokenSource.Token));
        var replyTask = Task.Run(() => LoadReplies(replyResponder, tokenSource.Token));

        Console.WriteLine("Connected... type 'close' to disconnect.");

        try
        {
            await Task.WhenAll(connectionTask, consoleTask, messageTask, disconnectTask);
        }
        catch (TaskCanceledException) { }
    }

    private static async Task ConsoleTask(Bot bot, CancellationTokenSource tokenSource)
    {
        do
        {
            var message = Console.ReadLine();

            switch (message)
            {
                case "close":
                    bot.Disconnect();
                    _running = false;
                    tokenSource.Cancel();
                    break;
                case "channels":
                    Console.WriteLine($"Connected Channels:\r\n{string.Join("\r\n", bot.ConnectedChannels.Select(c => $"{c.Name}, ID: {c.ID}, Type: {c.Type}"))}");
                    break;
                case "ping":
                    Console.WriteLine("Sending ping");
                    await bot.Shout("Pong");
                    break;
                default:
                    break;
            }
        } while (_running);
    }

    private static async Task ConnectionWatcher(Bot bot, CancellationToken token)
    {
        do
        {
            if (_running && !bot.IsConnected)
            {
                Console.WriteLine("Connection lost, trying to reconnect...");
                await bot.TryConnect(_token);
            }

            await Task.Delay(5000, token);
        } while (_running);
    }

    private static async Task DisconnectTask(Bot bot, CancellationToken token)
    {
        do
        {
            await Task.Delay(TimeSpan.FromHours(1), token);

            if (_running)
            {
                bot.Disconnect();
            }
        } while (_running);
    }

    private static async Task LoadReplies(WittyReplyResponder replyResponder, CancellationToken token)
    {
        do
        {
            var replyText = File.ReadAllText("replies.txt");
            var replySplit = replyText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var replyPart in replySplit)
            {
                var replyParts = replyPart.Split(";");
                var reply = replyParts[replyParts.Length - 1];
                for (var i = 0; i < replyParts.Length - 1; i++)
                {
                    var key = replyParts[i];
                    replyResponder.AddOrReplaceReply(key, reply);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(5), token);
        } while (_running);
    }

    private static async Task MessageTask(Bot bot, CancellationToken token)
    {
        var rnd = new Random();

        do
        {
            if (IsWorkingHours())
            {
                var messages = File.ReadAllText("messages.txt");
                var split = messages.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                var idx = rnd.Next(split.Length);
                var message = split[idx];

                await bot.Shout(message);
            }
            else
            {
                Console.WriteLine("Outside of working hours, bot is asleep...");
            }

            var rndSeconds = rnd.Next(1000);
            var waitTime = TimeSpan.FromHours(3) + TimeSpan.FromSeconds(rndSeconds);

            await Task.Delay(waitTime, token);
        } while (_running);
    }

    private static bool IsWorkingHours() => IsWorkDay() && DateTime.Now.TimeOfDay.Between(TimeSpan.FromHours(8), TimeSpan.FromHours(18));

    private static bool IsWorkDay() => DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday;

    private static Bot CreateBot(WittyReplyResponder replyResponder)
    {
        var bot = new Bot();
        var responders = new List<IResponder> {
            replyResponder,
            new HelloResponder(),
            new GoogleMeetResponder(),
            new ZoomResponder()
        };

        bot.Responders.AddRange(responders);

        return bot;
    }
}
