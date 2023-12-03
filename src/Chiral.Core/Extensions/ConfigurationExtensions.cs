using Chiral.Core.Attributes;
using Microsoft.Extensions.Configuration;

namespace Chiral.Core.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets a configuration sub-section with the specified key, and attempts to bind
    /// the configuration instance to a new instance of type T.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="name">The section name.</param>
    /// <typeparam name="T">The options type.</typeparam>
    /// <returns>The options.</returns>
    /// <exception cref="Exception"></exception>
    public static T Load<T>(this IConfiguration configuration, string name = null!)
    {
        var attribute = GetOptionAttribute<T>();
        var key = string.IsNullOrWhiteSpace(name) ? attribute.Name : $"{attribute.Name}:{name}";
        var options = GetOptions<T>(configuration, key);

        options.EnsureRequiredProperties();

        return options;
    }

    #region Private Methods

    private static T GetOptions<T>(IConfiguration configuration, string path)
    {
        var source = typeof(T);
        var options = configuration.GetSection(path).Get<T>();

        if (options == null)
        {
            throw new ArgumentException($"Options from '{source.FullName}', not found for '{path}'");
        }

        return options;
    }

    private static OptionAttribute GetOptionAttribute<T>()
    {
        var source = typeof(T);
        var target = typeof(OptionAttribute);

        if (source.GetCustomAttributes(target, true).FirstOrDefault() is not OptionAttribute attribute)
        {
            throw new InvalidOperationException($"Missing option attribute for '{source.FullName}'");
        }

        return attribute;
    }

    #endregion
}
