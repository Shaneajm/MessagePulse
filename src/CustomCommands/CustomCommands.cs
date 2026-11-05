namespace MsgPulse.Services;

using Config;
using Models;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;

/// <summary>Handles creation of custom commands.</summary>
public sealed class CustomCommandsHandler(
    ISwiftlyCore core,
    ILogger<CustomCommandsHandler> logger,
    MessageProcessor processor,
    JsonConfigLoader configLoader)
{
    private CommandsConfig Config { get; set; } = new();
    private readonly HashSet<string> registered = new(StringComparer.OrdinalIgnoreCase);

    public void Initialize(bool hotReload)
    {
        this.Config = configLoader.Load<CommandsConfig>("custom_commands.json", "example_custom_commands.json");

        if (hotReload)
            UnregisterAllCommands();

        foreach (var command in Config.Commands)
        {
            Register(command);
        }

        logger.LogInformation("Registered {Count} custom commands: {Commands}",
            registered.Count, string.Join(", ", registered));
    }

    // Command registration ¯\_(ツ)_/¯
    public void Register(CustomCommand command)
    {
        var triggers = command.Triggers;

        if (triggers.Any(registered.Contains))
        {
            logger.LogError("Custom command with triggers {Triggers} is already registered.",
                string.Join(", ", triggers));
            return;
        }

        var target = command.Target.Trim();
        var replyToCaller =
            target.Equals("caller", StringComparison.OrdinalIgnoreCase)
            || target.Equals("player", StringComparison.OrdinalIgnoreCase);

        foreach (var trigger in triggers)
        {
            core.Command.RegisterCommand(trigger, ctx =>
                {
                    if (replyToCaller)
                    {
                        ctx.Reply(processor.ProcessMessage(ctx.Sender, command.Message));
                    }
                    else
                    {
                        processor.SendToAll(command.Message);
                    }
                }, registerRaw: true
            );

            registered.Add(trigger);
        }
    }

    public void UnregisterAllCommands()
    {
        foreach (var command in registered)
        {
            core.Command.UnregisterCommand(command);
        }

        registered.Clear();
    }

    public void Release()
    {
        UnregisterAllCommands();
    }
}
