using System;
using System.Collections.Generic;
using System.Linq;
using MargieBot;

namespace Kerstbot.Responsers;

public class WittyReplyResponder : IResponder
{
    private readonly IDictionary<string, string> _replies;
    private static readonly object _lock = new object();

    public WittyReplyResponder()
    {
        _replies = new Dictionary<string, string>(StringComparer.InvariantCulture);
    }

    public void AddOrReplaceReply(string key, string reply)
    {
        if (!_replies.ContainsKey(key))
        {
            lock (_lock)
                _replies.Add(key, reply);

            return;
        }

        if (_replies[key] == reply)
            return;

        lock (_lock)
            _replies[key] = reply;
    }

    public bool CanRespond(ResponseContext context)
    {
        return !context.BotHasResponded
               && !context.Message.User.IsSlackbot
               && _replies.Keys.ToList().Any(x => context.Message.Text.ToLower().Contains(x));
    }

    public BotMessage GetResponse(ResponseContext context)
    {
        var key = _replies.Keys.ToList().FirstOrDefault(x => context.Message.Text.ToLower().Contains(x));
        var reply = _replies[key];

        return new BotMessage { Text = reply };
    }
}
