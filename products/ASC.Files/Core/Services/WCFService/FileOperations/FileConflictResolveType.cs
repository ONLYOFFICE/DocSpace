namespace ASC.Web.Files.Services.WCFService.FileOperations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FileConflictResolveType
{
    Skip = 0,
    Overwrite = 1,
    Duplicate = 2
}
