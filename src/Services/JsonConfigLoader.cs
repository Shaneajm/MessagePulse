namespace MsgPulse.Services;

using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SwiftlyS2.Shared;

/// <summary>Loads per-feature JSON config files from the plugin's config directory.</summary>
public sealed class JsonConfigLoader(ISwiftlyCore core)
{
    private static readonly JsonSerializerOptions writeOptions = new() { WriteIndented = true };

    /// <summary>
    /// Loads <paramref name="fileName"/> into <typeparamref name="TConfig"/>. If the file does not exist it is
    /// created from <c>resources/configs/{exampleFileName}</c>, or from the model's defaults if no example exists.
    /// </summary>
    /// <remarks>
    /// Each file is read into its own configuration root, so keys with the same name in different files
    /// (e.g. "Enabled") cannot leak into each other.
    /// </remarks>
    public TConfig Load<TConfig>(string fileName, string? exampleFileName = null) where TConfig : class, new()
    {
        var path = core.Configuration.GetConfigPath(fileName);
        EnsureExists<TConfig>(path, exampleFileName);

        return new ConfigurationBuilder()
            .AddJsonFile(path, optional: false, reloadOnChange: false)
            .Build()
            .Get<TConfig>() ?? new TConfig();
    }

    private void EnsureExists<TConfig>(string path, string? exampleFileName) where TConfig : class, new()
    {
        if (File.Exists(path))
            return;

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        if (exampleFileName is not null)
        {
            var example = Path.Combine(core.PluginPath, "resources", "configs", exampleFileName);
            if (File.Exists(example))
            {
                File.Copy(example, path, overwrite: false);
                return;
            }
        }

        File.WriteAllText(path, JsonSerializer.Serialize(new TConfig(), writeOptions));
    }
}
