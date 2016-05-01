namespace Shuttle.Core.Data
{
    public interface IQueryParameter : IQuery
    {
        IQueryParameter AddParameterValue(IMappedColumn column, object value);
    }
}