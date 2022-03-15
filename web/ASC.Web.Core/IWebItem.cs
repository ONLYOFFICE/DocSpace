namespace ASC.Web.Core
{
    public interface IWebItem
    {
        Guid ID { get; }

        string Name { get; }

        string Description { get; }

        string StartURL { get; }
        string ApiURL { get; }

        string HelpURL { get; }

        string ProductClassName { get; }

        bool Visible { get; }

        WebItemContext Context { get; }
    }
}
