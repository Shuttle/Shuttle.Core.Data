namespace Shuttle.Core.Data
{
    public interface IQueryParemeter : IQuery
    {
        IQueryParemeter AddParameterValue(IMappedColumn column, object value);
    }
}