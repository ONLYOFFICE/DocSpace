using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    public class ModelBuilderWrapper
    {
        ModelBuilder ModelBuilder { get; set; }
        Provider Provider { get; set; }
        Dictionary<Provider, List<Action<ModelBuilder>>> Actions { get; set; }

        private ModelBuilderWrapper(ModelBuilder modelBuilder, Provider provider)
        {
            ModelBuilder = modelBuilder;
            Provider = provider;
            Actions = new Dictionary<Provider, List<Action<ModelBuilder>>>();
        }

        public static ModelBuilderWrapper From(ModelBuilder modelBuilder, Provider provider)
        {
            return new ModelBuilderWrapper(modelBuilder, provider);
        }

        public ModelBuilderWrapper Add(Action<ModelBuilder> action, Provider provider)
        {
            if (!Actions.ContainsKey(provider))
            {
                Actions.Add(provider, new List<Action<ModelBuilder>>());
            }

            Actions[provider].Add(action);

            return this;
        }

        public void Finish()
        {
            if (Actions.ContainsKey(Provider))
            {
                foreach (var action in Actions[Provider])
                {
                    action(ModelBuilder);
                }
            }
        }
    }
}
