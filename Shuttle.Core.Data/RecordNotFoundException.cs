using System;

namespace Shuttle.Core.Data;

public class RecordNotFoundException : Exception
{
    public RecordNotFoundException(string message) : base(message)
    {
    }

    public static RecordNotFoundException For(string name, object id)
    {
        return new(string.Format(Resources.RecordNotFoundException, name, id));
    }

    public static RecordNotFoundException For<T>(object id)
    {
        return For(typeof(T).Name, id);
    }
}