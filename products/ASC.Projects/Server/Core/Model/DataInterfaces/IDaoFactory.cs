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


using ASC.Common;
using ASC.Projects.Data;

namespace ASC.Projects.Core.DataInterfaces
{
    [Scope(typeof(DaoFactory), Additional = typeof(DaoFactoryExtension))]
    public interface IDaoFactory
    {
        IProjectDao GetProjectDao();

        IParticipantDao GetParticipantDao();

        IMilestoneDao GetMilestoneDao();

        ITaskDao GetTaskDao();

        ISubtaskDao GetSubtaskDao();

        IMessageDao GetMessageDao();

        ICommentDao GetCommentDao();

        ITemplateDao GetTemplateDao();

        ITimeSpendDao GetTimeSpendDao();

        IReportDao GetReportDao();

        ISearchDao GetSearchDao();

        ITagDao GetTagDao();

        IStatusDao GetStatusDao();
    }
}
