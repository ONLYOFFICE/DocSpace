namespace ASC.Files.Core.Model;

public class ChangeHistoryRequestDto
{
    public int Version { get; set; }
    public bool ContinueVersion { get; set; }
}
