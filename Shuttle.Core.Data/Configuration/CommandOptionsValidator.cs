using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class CommandOptionsValidator : IValidateOptions<CommandOptions>
    {
        public ValidateOptionsResult Validate(string name, CommandOptions options)
        {
            Guard.AgainstNull(options, nameof(options));

            if (options.CommandTimeout < 0)
            {
                return ValidateOptionsResult.Fail(Resources.TimeoutException);
            }

            return ValidateOptionsResult.Success;
        }
    }
}