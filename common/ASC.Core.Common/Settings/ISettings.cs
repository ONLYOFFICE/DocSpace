namespace ASC.Core.Common.Settings;

public interface ISettings
{
    Guid ID { get; }
    ISettings GetDefault(IServiceProvider serviceProvider);
}
