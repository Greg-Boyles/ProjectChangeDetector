using System.Collections.Generic;

namespace ChangeDetector.Core.Processor
{
    internal interface ICsprojProcessor
    {
        List<string> GetProjectReference(string csprojFilePath);
        List<string> FromRelativeToFullPath(string csprojPath, List<string> projectReferences);
    }
}