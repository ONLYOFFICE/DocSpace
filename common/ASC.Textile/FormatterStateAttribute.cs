namespace Textile
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FormatterStateAttribute : Attribute
    {
        public string Pattern { get; }

        public FormatterStateAttribute(string pattern)
        {
            Pattern = pattern;
        }

        public static FormatterStateAttribute Get(Type type)
        {
            var atts = type.GetCustomAttributes(typeof(FormatterStateAttribute), false);
            if (atts.Length == 0)
                return null;
            return (FormatterStateAttribute)atts[0];
        }
    }
}
