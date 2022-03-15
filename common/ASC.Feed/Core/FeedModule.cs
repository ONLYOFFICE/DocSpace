namespace ASC.Feed;

[Flags]
public enum FeedModule
{
    None = 0,
    WhatsNew = 1,
    Timeline = 2,
    Any = WhatsNew | Timeline
}
