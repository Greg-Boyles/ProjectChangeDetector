using System.Collections.Generic;

namespace ChangeDetector.Core.Processor
{
    internal class DotNetCoreCsprojProcessor : BaseCsprojProcessor, ICsprojProcessor
    {
        public List<string> GetProjectReference(string csprojFilePath)
        {
            var xPathDocument = LoadCsproj(csprojFilePath);

            var nav = xPathDocument.CreateNavigator();
            var lines = nav.Select("//ProjectReference");

            var projectReferences = new List<string>();

            while (lines.MoveNext())
            {
                projectReferences.Add(lines.Current.GetAttribute("Include", ""));
            }

            return projectReferences;
        }
    }
}