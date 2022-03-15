namespace ASC.Notify.Patterns;

public interface IPatternFormatter
{
    string[] GetTags(IPattern pattern);
    void FormatMessage(INoticeMessage message, ITagValue[] tagsValues);
}
