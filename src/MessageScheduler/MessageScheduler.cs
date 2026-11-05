namespace MsgPulse.Services;

using Config;
using Models;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;

/// <summary>Schedules messages to be sent to players at specified intervals.</summary>
public sealed class MessageScheduler(
    ISwiftlyCore core,
    MessageProcessor processor,
    ILogger<MessageScheduler> logger,
    JsonConfigLoader configLoader)
{
    private readonly List<CancellationTokenSource> timers = [];
    private ChatBroadcast Config { get; set; } = new();

    public void Initialize(bool hotReload)
    {
        logger.LogInformation("Initializing...");

        if (hotReload)
        {
            this.Release();
        }

        this.Config = configLoader.Load<ChatBroadcast>("chat_broadcast.json", "example_chat_broadcast.json");

        var defaultInterval = Config.DefaultIntervalSeconds > 0 ? Config.DefaultIntervalSeconds : 60f;
        var buckets = new Dictionary<float, List<MsgModel>>();

        // Group messages by interval
        foreach (var msg in Config.Messages)
        {
            if (msg.Broadcast != true || string.IsNullOrWhiteSpace(msg.Message))
                continue;

            var interval = msg.Interval > 0 ? msg.Interval.Value : defaultInterval;

            if (!buckets.TryGetValue(interval, out var list))
            {
                list = new List<MsgModel>();
                buckets[interval] = list;
            }

            list.Add(msg);
        }

        // Schedule timers for each interval
        foreach (var kvp in buckets)
        {
            var interval = kvp.Key;
            var messages = kvp.Value;

            if (messages.Count == 0) continue;

            var index = 0;
            var token = core.Scheduler.DelayAndRepeatBySeconds(
                interval,
                interval,
                () =>
                {
                    var msg = messages[index];
                    processor.SendToAll(msg.Message);
                    index = (index + 1) % messages.Count;
                }
            );

            timers.Add(token);
        }

        logger.LogInformation("Initialized with {Count} timers.", timers.Count);
    }

    public void Release()
    {
        foreach (var token in timers)
        {
            try
            {
                token.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            finally
            {
                token.Dispose();
            }
        }

        timers.Clear();
    }
}
