namespace Shuttle.Core.Data
{
    public class DataAccessOptions
    {
        public int CommandTimeout { get; set; } = 15;
        public DatabaseContextFactoryOptions DatabaseContextFactory { get; set; } = new DatabaseContextFactoryOptions();
    }
}