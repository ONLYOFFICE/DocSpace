namespace ASC.Core.Common.EF.Model;

public class ModelBuilderWrapper
{
    private ModelBuilder ModelBuilder { get; set; }
    private Provider Provider { get; set; }

    private ModelBuilderWrapper(ModelBuilder modelBuilder, Provider provider)
    {
        ModelBuilder = modelBuilder;
        Provider = provider;
    }

    public static ModelBuilderWrapper From(ModelBuilder modelBuilder, Provider provider)
    {
        return new ModelBuilderWrapper(modelBuilder, provider);
    }

    public ModelBuilderWrapper Add(Action<ModelBuilder> action, Provider provider)
    {
        if (provider == Provider)
        {
            action(ModelBuilder);
        }

        return this;
    }

    public ModelBuilderWrapper HasData<T>(params T[] data) where T : class
    {
        ModelBuilder.Entity<T>().HasData(data);

        return this;
    }

    public void AddDbFunction()
    {
        ModelBuilder
            .HasDbFunction(typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.JsonValue)))
            .HasTranslation(e =>
            {
                var res = new List<SqlExpression>();
                if (e is List<SqlExpression> list)
                {
                    if (list[0] is SqlConstantExpression key)
                    {
                        res.Add(new SqlFragmentExpression($"`{key.Value}`"));
                    }

                    if (list[1] is SqlConstantExpression val)
                    {
                        res.Add(new SqlConstantExpression(Expression.Constant($"$.{val.Value}"), val.TypeMapping));
                    }
                }

                return new SqlFunctionExpression("JSON_EXTRACT", res, true, res.Select((SqlExpression a) => false), typeof(string), null);
            });
    }
}
