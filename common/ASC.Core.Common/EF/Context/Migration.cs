using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Core.Common.EF.Context
{
    public class Migration
    {
        public Migration(string name, string query, int number)
        {
            Number = number;
            Name = name;
            Query = query;

        }

        public readonly int Number;

        public string Query { get; }

        public string Name { get; }
    }
}
