using ASC.Common.Mapping.PrimitiveTypeConverters;

namespace ASC.Common.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        Array.ForEach(AppDomain.CurrentDomain.GetAssemblies(), a => ApplyMappingsFromAssembly(a));
        ApplyPrimitiveMappers();
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        if (!assembly.GetName().Name.StartsWith("ASC."))
        {
            return;
        }

        var types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)));

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethod("Mapping")
                ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");

            methodInfo?.Invoke(instance, new object[] { this });
        }
    }

    private void ApplyPrimitiveMappers()
    {
        CreateMap<long, DateTime>().ReverseMap()
            .ConvertUsing<TimeConverter>();
    }
}
