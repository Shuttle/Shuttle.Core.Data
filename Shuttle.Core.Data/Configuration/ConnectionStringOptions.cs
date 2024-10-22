namespace Shuttle.Core.Data;

public class ConnectionStringOptions
{
    public string ConnectionString { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string ProviderName { get; set; } = default!;
}