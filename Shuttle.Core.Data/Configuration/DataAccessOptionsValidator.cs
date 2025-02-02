using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class DataAccessOptionsValidator : IValidateOptions<DataAccessOptions>
{
    public ValidateOptionsResult Validate(string? name, DataAccessOptions options)
    {
        Guard.AgainstNull(options);

        if (options.CommandTimeout < 0)
        {
            return ValidateOptionsResult.Fail(Resources.TimeoutException);
        }

        return ValidateOptionsResult.Success;
    }
}