using System.Linq;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public static class AssemblerExtensions
    {
        public static T AssembleItem<T>(this IAssembler<T> assembler, MappedData data) where T : class
        {
            return assembler.Assemble(data).FirstOrDefault();
        }

        public static async Task<T> AssembleItemAsync<T>(this IAssembler<T> assembler, MappedData data) where T : class
        {
            return (await assembler.AssembleAsync(data).ConfigureAwait(false)).FirstOrDefault();
        }
    }
}