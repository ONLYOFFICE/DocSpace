/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;

using ASC.Common;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Data.DAO;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Projects.Data
{
    [Scope]
    public class DaoFactory : IDaoFactory
    {
        private IServiceProvider ServiceProvider { get; }
        public DaoFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IProjectDao GetProjectDao()
        {
            return ServiceProvider.GetService<IProjectDao>();
        }

        public IParticipantDao GetParticipantDao()
        {
            return ServiceProvider.GetService<IParticipantDao>();
        }

        public IMilestoneDao GetMilestoneDao()
        {
            return ServiceProvider.GetService<IMilestoneDao>();
        }

        public ITaskDao GetTaskDao()
        {
             return ServiceProvider.GetService<ITaskDao>();
        }

        public ISubtaskDao GetSubtaskDao()
        {
            return ServiceProvider.GetService<ISubtaskDao>();
        }

        public IMessageDao GetMessageDao()
        {
            return ServiceProvider.GetService<IMessageDao>();
        }

        public ICommentDao GetCommentDao()
        {
            return ServiceProvider.GetService<ICommentDao>();
        }

        public ITemplateDao GetTemplateDao()
        {
            return ServiceProvider.GetService<ITemplateDao>();
        }

        public ITimeSpendDao GetTimeSpendDao()
        {
            return ServiceProvider.GetService<ITimeSpendDao>();
        }

        public IReportDao GetReportDao()
        {
            return ServiceProvider.GetService<IReportDao>();
        }

        public ISearchDao GetSearchDao()
        {
            return ServiceProvider.GetService<ISearchDao>();
        }

        public ITagDao GetTagDao()
        {
            return ServiceProvider.GetService<ITagDao>();
        }

        public IStatusDao GetStatusDao()
        {
            return ServiceProvider.GetService<IStatusDao>();
        }

    }

    public class DaoFactoryExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<IProjectDao>();
            services.TryAdd<IParticipantDao>();
            services.TryAdd<IMilestoneDao>();
            services.TryAdd<ITaskDao>();
            services.TryAdd<ISubtaskDao>();
            services.TryAdd<IMessageDao>();
            services.TryAdd<ICommentDao>();
            services.TryAdd<ITemplateDao>();
            services.TryAdd<ITimeSpendDao>();
            services.TryAdd<IReportDao>();
            services.TryAdd<ISearchDao>();
            services.TryAdd<ITagDao>();
            services.TryAdd<IStatusDao>();
        }
    }
}
