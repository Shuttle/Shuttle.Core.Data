using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class CommandSettingsValidator : IValidateOptions<CommandSettings>
    {
        public ValidateOptionsResult Validate(string name, CommandSettings options)
        {
            Guard.AgainstNull(options, nameof(options));

            if (options.Timeout < 0)
            {
                return ValidateOptionsResult.Fail(Resources.TimeoutException);
            }

            return ValidateOptionsResult.Success;
        }
    }
}