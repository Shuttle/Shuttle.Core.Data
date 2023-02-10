namespace Shuttle.Core.Data
{
    public interface IQueryParameter : IQuery
    {
        IQueryParameter AddParameterValue(IColumn column, object value);
    }
}