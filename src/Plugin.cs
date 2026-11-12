namespace MsgPulse;

using Services;
using PlaceholderAPI.Contract;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[PluginMetadata(Id = "MessagePulse", Version = "1.0.0", Name = "MessagePulse", Author = "itsAudio",
    Description = "Advertisments, Custom commands, Event based messages and more...")]
public sealed class MessagePulse : BasePlugin
{
    private ILogger<MessagePulse> Logger { get; }
    private IServiceProvider Provider { get; }

    private MessageProcessor Processor { get; }
    private DynamicEvents Events { get; }
    private MessageScheduler Scheduler { get; }
    private CustomCommandsHandler CustomCommands { get; }
    private DeadShowImage DeadShowImage { get; }

    public MessagePulse(ISwiftlyCore core) : base(core)
    {
        var services = new ServiceCollection();

        services.AddSwiftly(core);

        services.AddJsonConfigLoader()
            .AddMessageProcessor()
            .AddMessageScheduler()
            .AddDynamicEvents()
            .AddCustomCommands()
            .AddDeadShowImage();

        this.Provider = services.BuildServiceProvider();

        this.Logger = Provider.GetRequiredService<ILogger<MessagePulse>>();
        this.Processor = Provider.GetRequiredService<MessageProcessor>();
        this.Events = Provider.GetRequiredService<DynamicEvents>();
        this.Scheduler = Provider.GetRequiredService<MessageScheduler>();
        this.CustomCommands = Provider.GetRequiredService<CustomCommandsHandler>();
        this.DeadShowImage = Provider.GetRequiredService<DeadShowImage>();
    }

    // Register PlaceholderAPI.v1 interface
    public override void UseSharedInterface(IInterfaceManager interfaceManager)
    {
        if (!interfaceManager.HasSharedInterface("PlaceholderAPI.v1"))
            return;

        var api = interfaceManager.GetSharedInterface<IPlaceholderAPIv1>("PlaceholderAPI.v1");

        Processor.AttachPlaceholderApi(api);
    }

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("Plugin loading...");

        this.Events.Initialize(hotReload);
        this.Scheduler.Initialize(hotReload);
        this.CustomCommands.Initialize(hotReload);
        this.DeadShowImage.Initialize(hotReload);

        Logger.LogInformation("Successfully loaded.");
    }

    public override void Unload()
    {
        // Release all services
        this.Events.Release();
        this.Scheduler.Release();
        this.Processor.Release();
        this.CustomCommands.Release();
        this.DeadShowImage.Release();

        Logger.LogInformation("Successfully unloaded.");
    }
}
