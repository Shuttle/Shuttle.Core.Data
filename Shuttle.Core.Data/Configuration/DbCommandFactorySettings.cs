namespace Shuttle.Core.Data
{
    public class DbCommandFactorySettings
    {
        public DbCommandFactorySettings()
        {
            CommandTimeout = 15;
        }

        public int CommandTimeout { get; set; }
    }
}