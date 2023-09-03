using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IAssembler<T> where T : class 
    {
        IEnumerable<T> Assemble(MappedData data);
        Task<IEnumerable<T>> AssembleAsync(MappedData data);
    }
}