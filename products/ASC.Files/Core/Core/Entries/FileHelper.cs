namespace ASC.Files.Core;

[Scope]
public class FileHelper
{
    private readonly FileTrackerHelper _fileTracker;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly FileUtility _fileUtility;
    private readonly FileConverter _fileConverter;

    public FileHelper(FileTrackerHelper fileTracker, FilesLinkUtility filesLinkUtility, FileUtility fileUtility, FileConverter fileConverter)
    {
        _fileTracker = fileTracker;
        _filesLinkUtility = filesLinkUtility;
        _fileUtility = fileUtility;
        _fileConverter = fileConverter;
    }

    internal string GetTitle<T>(File<T> file)
    {
        return string.IsNullOrEmpty(file.ConvertedType)
                    ? file.PureTitle
                    : FileUtility.ReplaceFileExtension(file.PureTitle, _fileUtility.GetInternalExtension(file.PureTitle));
    }

    internal FileStatus GetFileStatus<T>(File<T> file, ref FileStatus currentStatus)
    {
        if (_fileTracker.IsEditing(file.ID))
        {
            currentStatus |= FileStatus.IsEditing;
        }

        if (_fileTracker.IsEditingAlone(file.ID))
        {
            currentStatus |= FileStatus.IsEditingAlone;
        }

        if (_fileConverter.IsConverting(file))
        {
            currentStatus |= FileStatus.IsConverting;
        }

        return currentStatus;
    }

    public string GetDownloadUrl<T>(FileEntry<T> fileEntry)
    {
        return _filesLinkUtility.GetFileDownloadUrl(fileEntry.ID);
    }
}
