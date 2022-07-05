using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class ConnectionStringSettingsValidator : IValidateOptions<ConnectionStringSettings>
    {
        public ValidateOptionsResult Validate(string name, ConnectionStringSettings options)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));
            Guard.AgainstNull(options, nameof(options));

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
}