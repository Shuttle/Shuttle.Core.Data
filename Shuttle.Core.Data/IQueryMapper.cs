﻿using System.Collections.Generic;

namespace Shuttle.Core.Data
{
    public interface IQueryMapper
    {
        MappedRow<T> MapRow<T>(IQuery query) where T : new();
        IEnumerable<MappedRow<T>> MapRows<T>(IQuery query) where T : new();
        T MapObject<T>(IQuery query) where T : new();
        IEnumerable<T> MapObjects<T>(IQuery query) where T : new();
        T MapValue<T>(IQuery query);
        IEnumerable<T> MapValues<T>(IQuery query);
        dynamic MapItem(IQuery query);
        IEnumerable<dynamic> MapItems(IQuery query);
    }
}