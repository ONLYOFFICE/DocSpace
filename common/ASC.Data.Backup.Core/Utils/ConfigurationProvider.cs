using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace ASC.Data.Backup.Utils;

internal static class ConfigurationProvider
{
    public static Configuration Open(string fileName)
    {
        var fileMap = new ExeConfigurationFileMap
        {
            ExeConfigFilename = PathHelper.ToRootedConfigPath(fileName)
        };

        return ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
    }
}
