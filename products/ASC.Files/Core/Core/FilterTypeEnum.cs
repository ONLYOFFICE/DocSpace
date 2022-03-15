namespace ASC.Files.Core;

public enum FilterType
{
    [EnumMember] None = 0,
    [EnumMember] FilesOnly = 1,
    [EnumMember] FoldersOnly = 2,
    [EnumMember] DocumentsOnly = 3,
    [EnumMember] PresentationsOnly = 4,
    [EnumMember] SpreadsheetsOnly = 5,
    [EnumMember] ImagesOnly = 7,
    [EnumMember] ByUser = 8,
    [EnumMember] ByDepartment = 9,
    [EnumMember] ArchiveOnly = 10,
    [EnumMember] ByExtension = 11,
    [EnumMember] MediaOnly = 12,
}