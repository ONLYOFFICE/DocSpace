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

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Projects.Engine
{
    [Scope(Additional = typeof(EngineFactoryExtension))]
    public class EngineFactory
    {
        private IServiceProvider ServiceProvider;

        public EngineFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public FileEngine GetFileEngine()
        {
            return ServiceProvider.GetService<FileEngine>();
        }

        public ProjectEngine GetProjectEngine()
        {
            return ServiceProvider.GetService<ProjectEngine>();
        }

        public MilestoneEngine GetMilestoneEngine()
        {
            return ServiceProvider.GetService<MilestoneEngine>();
        }

        public CommentEngine GetCommentEngine()
        {
            return ServiceProvider.GetService<CommentEngine>();
        }

        public SearchEngine GetSearchEngine()
        {
            return ServiceProvider.GetService<SearchEngine>();
        }

        public TaskEngine GetTaskEngine()
        {
            return ServiceProvider.GetService<TaskEngine>();
        }

        public SubtaskEngine GetSubtaskEngine()
        {
            return ServiceProvider.GetService<SubtaskEngine>();
        }

        public MessageEngine GetMessageEngine()
        {
            return ServiceProvider.GetService<MessageEngine>();
        }

        public TimeTrackingEngine GetTimeTrackingEngine()
        {
            return ServiceProvider.GetService<TimeTrackingEngine>();
        }

        public ParticipantEngine GetParticipantEngine()
        {
            return ServiceProvider.GetService<ParticipantEngine>();
        }

        public TagEngine GetTagEngine()
        {
            return ServiceProvider.GetService<TagEngine>();
        }

        public ReportEngine GetReportEngine()
        {
            return ServiceProvider.GetService<ReportEngine>();
        }

        public TemplateEngine GetTemplateEngine()
        {
            return ServiceProvider.GetService<TemplateEngine>();
        }

        public StatusEngine GetStatusEngine()
        {
            return ServiceProvider.GetService<StatusEngine>();
        }
    }

    public class EngineFactoryExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<FileEngine>();
            services.TryAdd<ProjectEngine>();
            services.TryAdd<MilestoneEngine>();
            services.TryAdd<CommentEngine>();
            services.TryAdd<SearchEngine>();
            services.TryAdd<TaskEngine>();
            services.TryAdd<SubtaskEngine>();
            services.TryAdd<MessageEngine>();
            services.TryAdd<TimeTrackingEngine>();
            services.TryAdd<ParticipantEngine>();
            services.TryAdd<TagEngine>();
            services.TryAdd<ReportEngine>();
            services.TryAdd<TemplateEngine>();
            services.TryAdd<StatusEngine>();
        }
    }
}