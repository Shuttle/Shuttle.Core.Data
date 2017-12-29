using System.Data.Common;

namespace Shuttle.Core.Data
{
    public interface IDbProviderFactories
    {
        DbProviderFactory GetFactory(string providerName);
    }
}