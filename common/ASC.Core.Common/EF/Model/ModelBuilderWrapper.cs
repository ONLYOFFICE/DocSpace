using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
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
    }
}
