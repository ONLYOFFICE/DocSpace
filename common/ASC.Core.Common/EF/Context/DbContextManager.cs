using System;
using System.Collections.Generic;

using Microsoft.Extensions.Options;

namespace ASC.Core.Common.EF
{
    public class DbContextManager<T> : OptionsManager<T>, IDisposable where T : BaseDbContext, new()
    {
        private Dictionary<string, T> Pairs { get; set; }
        private List<T> AsyncList { get; set; }

        public IOptionsFactory<T> Factory { get; }

        public DbContextManager(IOptionsFactory<T> factory) : base(factory)
        {
            Pairs = new Dictionary<string, T>();
            AsyncList = new List<T>();
            Factory = factory;
        }

        public override T Get(string name)
        {
            var result = base.Get(name);

            if (!Pairs.ContainsKey(name))
            {
                Pairs.Add(name, result);
            }

            return result;
        }

        public T GetNew(string name = "default")
        {
            var result = Factory.Create(name);

            AsyncList.Add(result);

            return result;
        }

        public void Dispose()
        {
            foreach (var v in Pairs)
            {
                v.Value.Dispose();
            }

            foreach (var v in AsyncList)
            {
                v.Dispose();
            }
        }
    }
}
