using System.Dynamic;

namespace ChangeDetector.Core.Processor
{
    internal class CsprojProcessorFactory
    {
        public static ICsprojProcessor CreateInstance()
        {
            return new DotNetCoreCsprojProcessor();
        }
    }
}