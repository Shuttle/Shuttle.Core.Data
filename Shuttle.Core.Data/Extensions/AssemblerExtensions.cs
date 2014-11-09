using System.Linq;

namespace Shuttle.Core.Data
{
    public static class AssemblerExtensions
    {
        public static T AssembleItem<T>(this IAssembler<T> assembler, MappedData data) where T : class
        {
            return assembler.Assemble(data).FirstOrDefault();
        }
    }
}