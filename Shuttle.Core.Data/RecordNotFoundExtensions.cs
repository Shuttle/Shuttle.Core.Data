namespace Shuttle.Core.Data
{
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
    }
}