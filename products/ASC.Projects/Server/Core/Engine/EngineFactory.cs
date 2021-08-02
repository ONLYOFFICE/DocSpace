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
    [Scope]
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
            return ServiceProvider.GetService<ProjectEngine>().Init(false);
        }

        public MilestoneEngine GetMilestoneEngine()
        {
            return ServiceProvider.GetService<MilestoneEngine>().Init(false);
        }

        public CommentEngine GetCommentEngine()
        {
            return ServiceProvider.GetService<CommentEngine>().Init(false);
        }

        public SearchEngine GetSearchEngine()
        {
            return ServiceProvider.GetService<SearchEngine>();
        }

        public TaskEngine GetTaskEngine()
        {
            return ServiceProvider.GetService<TaskEngine>().Init(false);
        }

        public SubtaskEngine GetSubtaskEngine()
        {
            return ServiceProvider.GetService<SubtaskEngine>().Init(false);
        }

        public MessageEngine GetMessageEngine()
        {
            return ServiceProvider.GetService<MessageEngine>().Init(false);
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
}