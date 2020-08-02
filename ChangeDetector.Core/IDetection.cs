namespace ChangeDetector.Core
{
    public interface IDetection
    {
        IDetection LoadInChangeLog(ChangeLog changeLog);
        IDetection Run();

    }
}