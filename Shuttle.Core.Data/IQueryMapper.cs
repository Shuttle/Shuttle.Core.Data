using System.Collections.Generic;

namespace Shuttle.Core.Data
{
    public interface IQueryMapper
    {
        MappedRow<T> MapRow<T>(IQuery query) where T : new();
        IEnumerable<MappedRow<T>> MapRows<T>(IQuery query) where T : new();
        T MapObject<T>(IQuery query) where T : new();
        IEnumerable<T> MapObjects<T>(IQuery query) where T : new();
    }
}