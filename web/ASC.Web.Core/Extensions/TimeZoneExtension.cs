namespace System
{
    public static class TimeZoneExtension
    {
        /// <summary>
        /// Get utc offset
        /// </summary>
        /// <param name="timeZone">timeZone</param>
        /// <param name="baseOffset">return BaseUtcOffset</param>
        /// <param name="dateTime">return GetUtcOffset(dateTime ?? DateTime.UtcNow)</param>
        /// <returns></returns>
        public static TimeSpan GetOffset(this TimeZoneInfo timeZone, bool baseOffset = false, DateTime dateTime = default)
        {
            if (baseOffset)
                return timeZone.BaseUtcOffset;

            return timeZone.GetUtcOffset(dateTime == default ? DateTime.UtcNow : dateTime);
        }
    }
}
