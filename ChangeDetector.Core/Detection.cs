using ChangeDetector.Core.Processor;

namespace ChangeDetector.Core
{
    public class Detection : IDetection
    {
        private ChangeLog _changeLog;

        public static IDetection Build() =>
            new Detection();

        public static IDetection Build(ChangeLog changeLog) =>
            new Detection().LoadInChangeLog(changeLog);

        public IDetection LoadInChangeLog(ChangeLog changeLog)
        {
            _changeLog = changeLog;
            return this;
        }

        public IDetection Run()
        {
            var csprojProcessor = CsprojProcessorFactory.CreateInstance();

            var projectReferences = csprojProcessor.GetProjectReference(_changeLog.CsprojPath);

            projectReferences = csprojProcessor.FromRelativeToFullPath(_changeLog.CsprojPath, projectReferences);

            return this;
        }
    }
}
