using System.Collections.Generic;

namespace Shuttle.Core.Data
{
    public interface IAssembler<out T> where T : class 
    {
        IEnumerable<T> Assemble(MappedData data);
    }
}