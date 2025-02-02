using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class ConnectionStringOptionsValidator : IValidateOptions<ConnectionStringOptions>
{
    public ValidateOptionsResult Validate(string? name, ConnectionStringOptions options)
    {
        Guard.AgainstNullOrEmptyString(name);
        Guard.AgainstNull(options);

        if (string.IsNullOrEmpty(options.ConnectionString))
        {
            return ValidateOptionsResult.Fail(string.Format(Resources.ConnectionStringMissingException, name));
        }

        if (string.IsNullOrEmpty(options.ProviderName))
        {
            return ValidateOptionsResult.Fail(string.Format(Resources.ProviderNameMissingException, name));
        }

        return ValidateOptionsResult.Success;
    }
}