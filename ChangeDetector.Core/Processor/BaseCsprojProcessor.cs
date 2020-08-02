using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;

namespace ChangeDetector.Core.Processor
{
    internal class BaseCsprojProcessor
    {
        protected XPathDocument LoadCsproj(string csprojFilePath) =>
            new XPathDocument(csprojFilePath);

        protected internal List<string> FromRelativeToFullPath(string csprojPath, List<string> projectReferences) =>
            projectReferences.ConvertAll(x => FromRelativeToFullPath(csprojPath, x)).ToList();

        protected internal string FromRelativeToFullPath(string csprojPath, string relativeFilePath)
        {
            var path = Path.GetDirectoryName(csprojPath);

            var currentFolder = new DirectoryInfo(path ?? throw new InvalidOperationException("Csproj Path is not found"));

            foreach (var pathSection in relativeFilePath.Split('\\'))
            {
                if (pathSection == "..")
                {
                    currentFolder = currentFolder.Parent;
                    continue;
                }

                currentFolder = new DirectoryInfo(currentFolder + "\\" + pathSection);


            }

            return currentFolder.FullName;
        }
    }
}