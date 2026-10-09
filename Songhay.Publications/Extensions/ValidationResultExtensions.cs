using FluentValidation.Results;

namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="ValidationResult"/>
/// </summary>
public static class ValidationResultExtensions
{
    /// <summary>
    /// Logs the <see cref="ValidationResult.Errors"/>
    /// with the specified <see cref="ILogger"/>
    /// </summary>
    /// <param name="validationResult">the <see cref="ValidationResult"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static void LogValidationErrors(this ValidationResult validationResult, ILogger logger)
    {
        int numberOfErrors = validationResult.Errors.Count;

        logger.LogWarning("Validation is NOT valid! There are {Count} validation errors...", numberOfErrors);

        StringBuilder sb = new($"Validation Errors:{Environment.NewLine}");
        foreach (ValidationFailure validationFailure in validationResult.Errors)
        {
            sb.AppendLine($"Property {validationFailure.PropertyName}: {validationFailure.ErrorMessage} [Error Code: {validationFailure.ErrorCode}]");
        }

        logger.LogDebug("{ValidationMessage}", sb.ToString());
    }
}