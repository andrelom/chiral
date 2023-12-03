using System.ComponentModel.DataAnnotations;

namespace Chiral.Core.Extensions;

public static class ObjectExtensions
{
    public static void EnsureRequiredProperties<T>(this T value)
    {
        if (value == null)
        {
            return;
        }

        var source = typeof(T);
        var validations = new List<ValidationResult>();
        var context = new ValidationContext(value, null, null);

        Validator.TryValidateObject(value, context, validations, true);

        if (!validations.Any())
        {
            return;
        }

        var errors = validations.Select(entry => entry.ErrorMessage);
        var message = string.Join('\n', errors);

        throw new InvalidOperationException($"One or more properties from '{source.FullName}' are invalid:\n{message}");
    }
}
