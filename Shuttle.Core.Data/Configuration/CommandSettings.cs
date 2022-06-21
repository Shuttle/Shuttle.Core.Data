namespace Shuttle.Core.Data
{
    public class CommandSettings
    {
        public CommandSettings()
        {
            Timeout = 15;
        }

        public int Timeout { get; set; }
    }
}