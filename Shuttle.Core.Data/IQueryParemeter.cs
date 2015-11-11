namespace Shuttle.Core.Data
{
    public interface IQueryParemeter : IQuery
    {
        IQuery AddParameterValue(IMappedColumn column, object value);
    }
}