using Microsoft.EntityFrameworkCore.Query;

namespace ASC.Core.Common.EF
{
    public static class JsonExtensions
    {
        public static string JsonValue(string column, [NotParameterized] string path) 
            => column + path; //not using
    }
}