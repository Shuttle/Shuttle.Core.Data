using System.Data;

namespace Shuttle.Core.Data;

public static class RecordNotFoundExtensions
{
    public static T GuardAgainstRecordNotFound<T>(this T entity, object id) where T : class
    {
        if (entity == null)
        {
            throw RecordNotFoundException.For<T>(id);
        }

        return entity;
    }

    public static DataRow GuardAgainstRecordNotFound<T>(this DataRow row, object id) where T : class
    {
        if (row == null)
        {
            throw RecordNotFoundException.For<T>(id);
        }

        return row;
    }
}