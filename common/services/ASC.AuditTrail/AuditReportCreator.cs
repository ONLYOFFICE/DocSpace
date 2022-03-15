namespace ASC.AuditTrail;

[Scope]
public class AuditReportCreator
{
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FileUploader _fileUploader;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly ILog _logger;

    public AuditReportCreator(
        GlobalFolderHelper globalFolderHelper,
        IOptionsMonitor<ILog> options,
        FileUploader fileUploader,
        FilesLinkUtility filesLinkUtility,
        CommonLinkUtility commonLinkUtility)
    {
        _globalFolderHelper = globalFolderHelper;
        _logger = options.CurrentValue;
        _fileUploader = fileUploader;
        _filesLinkUtility = filesLinkUtility;
        _commonLinkUtility = commonLinkUtility;
    }

    public string CreateCsvReport<TEvent>(IEnumerable<TEvent> events, string reportName) where TEvent : BaseEvent
    {
        try
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);

            csv.Configuration.RegisterClassMap(new BaseEventMap<TEvent>());

            csv.WriteHeader<TEvent>();
            csv.NextRecord();
            csv.WriteRecords(events);
            writer.Flush();

                    var file = _fileUploader.ExecAsync(_globalFolderHelper.FolderMy, reportName, stream.Length, stream, true).Result;
                    var fileUrl = _commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.GetFileWebEditorUrl(file.ID));

            fileUrl += string.Format("&options={{\"codePage\":{0}}}", Encoding.UTF8.CodePage);

            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.Error("Error while generating login report: " + ex);
            throw;
        }
    }
}