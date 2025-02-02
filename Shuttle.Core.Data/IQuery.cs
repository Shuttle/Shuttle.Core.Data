using System.Data;

namespace Shuttle.Core.Data;

public interface IQuery
{
    IQuery AddParameter(IColumn column, object? value);
    void Prepare(IDbCommand command);
}