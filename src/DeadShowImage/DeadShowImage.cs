namespace MsgPulse.Services;

using Config;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;

/// <summary>Displays advertisment image to dead players</summary>
public sealed partial class DeadShowImage(
    ISwiftlyCore core,
    ILogger<DeadShowImage> logger,
    JsonConfigLoader configLoader)
{
    private readonly List<CancellationTokenSource> timers = [];
    private DeadShowImageConfig Config { get; set; } = new();

    public void Initialize(bool hotReload)
    {
        core.Registrator.Register(this);

        logger.LogInformation("Initializing...");

        if (hotReload)
        {
            this.Release();
        }

        this.Config = configLoader.Load<DeadShowImageConfig>("dead_show_image.jsonc", "example_dead_show_image.jsonc");

        Start();
    }

    public void Start()
    {
        if (!Config.Enabled || Config.Images.Count == 0)
            return;

        var duration = Config.Interval > 0 ? Config.Interval : 15f;
        var gap = Config.Delay > 0 ? Config.Delay.Value : 2f;
        var period = duration + gap;

        var currentIndex = 0;

        var token = core.Scheduler.DelayAndRepeatBySeconds(gap, period, () =>
        {
            if (Config.Images.Count == 0) return;

            var image = Config.Images[currentIndex % Config.Images.Count];
            currentIndex++;

            deadPlayers.ToList().ForEach(x =>
                x.SendCenterHTML($"<img src='{image}' />", (int)duration * 1000)
            );
        });
        timers.Add(token);

        logger.LogInformation("Started with {Count} image(s).", Config.Images.Count);
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
        deadPlayers.Clear();
    }
}
