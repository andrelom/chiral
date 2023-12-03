namespace Chiral.Core.Attributes;

/// <summary>
/// It will assign the value of the properties via the path provided.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class OptionAttribute : Attribute
{
    public OptionAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// The property path in the settings.
    /// </summary>
    public string Name { get; }
}
