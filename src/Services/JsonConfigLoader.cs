namespace MsgPulse.Services;

using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;

/// <summary>Loads per-feature JSON config files from the plugin's config directory.</summary>
public sealed class JsonConfigLoader(ISwiftlyCore core, ILogger<JsonConfigLoader> logger)
{
    private static readonly JsonSerializerOptions writeOptions = new() { WriteIndented = true };

    /// <summary>
    /// Loads <paramref name="fileName"/> from <c>{swiftlys2}/configs/plugins/MessagePulse/</c> into
    /// <typeparamref name="TConfig"/>. If the file does not exist it is created from
    /// <c>resources/configs/{exampleFileName}</c>, or from the model's defaults if no example exists.
    /// </summary>
    /// <remarks>
    /// Each file is read into its own configuration root, so keys with the same name in different files
    /// (e.g. "Enabled") cannot leak into each other.
    /// </remarks>
    public TConfig Load<TConfig>(string fileName, string? exampleFileName = null) where TConfig : class, new()
    {
        // Resolve once to an absolute path so the file we create and the file we read are always the same,
        // regardless of the process working directory.
        var path = Path.GetFullPath(core.Configuration.GetConfigPath(fileName));
        var directory = Path.GetDirectoryName(path)!;

        EnsureExists<TConfig>(path, directory, exampleFileName);

        var config = new ConfigurationBuilder()
            .SetBasePath(directory)
            .AddJsonFile(Path.GetFileName(path), optional: false, reloadOnChange: false)
            .Build()
            .Get<TConfig>() ?? new TConfig();

        logger.LogInformation("Loaded {File} from {Path}", fileName, path);
        return config;
    }

    private void EnsureExists<TConfig>(string path, string directory, string? exampleFileName)
        where TConfig : class, new()
    {
        if (File.Exists(path))
            return;

        Directory.CreateDirectory(directory);

        if (exampleFileName is not null)
        {
            var example = Path.Combine(core.PluginPath, "resources", "configs", exampleFileName);
            if (File.Exists(example))
            {
                File.Copy(example, path, overwrite: false);
                logger.LogInformation("Created {Path} from {Example}", path, exampleFileName);
                return;
            }
        }

        File.WriteAllText(path, JsonSerializer.Serialize(new TConfig(), writeOptions));
        logger.LogInformation("Created {Path} with default values", path);
    }
}
