namespace ASC.Common.Mapping.PrimitiveTypeConverters;

public class TimeConverter : ITypeConverter<long, DateTime>, ITypeConverter<DateTime, long>
{
    public DateTime Convert(long source, DateTime destination, ResolutionContext context)
    {
        return new DateTime(source);
    }

    public long Convert(DateTime source, long destination, ResolutionContext context)
    {
        return source.Ticks;
    }
}