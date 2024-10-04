using PoProj.features;

namespace PoProj.MediaClasses;

public class NewsGenerator
{
    // Content
    private readonly List<IMedia> _mediaList;
    private readonly List<IReportable> _reportableList;
    
    // Iterating
    private int _reportablePosition = -1;
    private int _mediaPosition = 0;
    
    public NewsGenerator(List<IMedia> mediaList, List<IReportable> reportableList)
    {
        _mediaList = mediaList;
        _reportableList = reportableList;
    }

    public string? GenerateNextNews()
    {
        // All of the news have been already reported
        if (_mediaPosition >= _mediaList.Count || _reportableList.Count == 0)
            return null;
        
        // Updating position
        _reportablePosition++;
        if (_reportablePosition >= _reportableList.Count)
        {
            _reportablePosition = 0;
            _mediaPosition++;
        }

        if (_mediaPosition >= _mediaList.Count)
            return null;

        return _reportableList[_reportablePosition].GetReported(_mediaList[_mediaPosition]);
    }
}