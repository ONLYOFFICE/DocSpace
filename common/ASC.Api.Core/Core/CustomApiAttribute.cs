using System;

namespace ASC.Api.Core
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class CustomApiAttribute : Attribute
    {
    }
}
