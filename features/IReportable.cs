using PoProj.MediaClasses;

namespace PoProj.features;

public interface IReportable
{
    string GetReported(IMedia media);
}