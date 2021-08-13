/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
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