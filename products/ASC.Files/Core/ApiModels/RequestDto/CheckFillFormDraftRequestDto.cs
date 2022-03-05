namespace ASC.Files.Core.Model;

public class CheckFillFormDraftRequestDto
{
    public int Version { get; set; }
    public string Doc { get; set; }
    public string Action { get; set; }
    public bool RequestView => (Action ?? "").Equals("view", StringComparison.InvariantCultureIgnoreCase);
    public bool RequestEmbedded => (Action ?? "").Equals("embedded", StringComparison.InvariantCultureIgnoreCase)
                && !string.IsNullOrEmpty(Doc);
}
