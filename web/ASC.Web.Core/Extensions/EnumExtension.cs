namespace System
{
    public static class EnumExtension
    {
        public static T TryParseEnum<T>(string value, T defaultValue) where T : struct
        {
            return TryParseEnum(value, defaultValue, out _);
        }

        public static T TryParseEnum<T>(string value, T defaultValue, out bool isDefault) where T : struct
        {
            isDefault = false;
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                isDefault = true;
                return defaultValue;
            }
        }
    }
}
