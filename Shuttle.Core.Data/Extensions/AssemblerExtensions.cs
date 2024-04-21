using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public static class AssemblerExtensions
    {
        public static T AssembleItem<T>(this IAssembler<T> assembler, MappedData data) where T : class
        {
            return assembler.Assemble(data).FirstOrDefault();
        }

        public static async Task<T> AssembleItemAsync<T>(this IAsyncAssembler<T> assembler, MappedData data, CancellationToken cancellationToken = default) where T : class
        {
            return (await assembler.AssembleAsync(data, cancellationToken).ConfigureAwait(false)).FirstOrDefault();
        }
    }
}