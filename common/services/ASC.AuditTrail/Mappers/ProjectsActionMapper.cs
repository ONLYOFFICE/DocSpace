// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode


namespace ASC.AuditTrail.Mappers;

internal class ProjectsActionsMapper : IProductActionMapper
{
    public List<IModuleActionMapper> Mappers { get; }
    public ProductType Product { get; }
    public ProjectsActionsMapper()
    {
        Product = ProductType.Projects;

        Mappers = new List<IModuleActionMapper>()
        {
            new ProjectsActionMapper(),
            new MilestonesActionMapper(),
            new TasksActionMapper(),
            new DiscussionsActionMapper(),
            new TimeTrackingActionMapper(),
            new ReportsActionMapper(),
            new ProjectsSettingsActionMapper()
        };
    }
}

internal class ProjectsActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public ProjectsActionMapper()
    {
        Module = ModuleType.Projects;

        Actions = new MessageMapsDictionary(ProductType.Projects, Module)
        {
            {
                EntryType.Project, new Dictionary<ActionType, MessageAction[]>()
                {
                    { ActionType.Create, new[] { MessageAction.ProjectCreated, MessageAction.ProjectCreatedFromTemplate } },
                    { ActionType.Update, new[] { MessageAction.ProjectUpdated, MessageAction.ProjectUpdatedStatus, MessageAction.ProjectUpdatedTeam } },
                    { ActionType.Delete, new[] { MessageAction.ProjectDeleted, MessageAction.ProjectDeletedMember } },
                    { ActionType.Link, new[] { MessageAction.ProjectLinkedCompany, MessageAction.ProjectLinkedPerson, MessageAction.ProjectLinkedContacts } },
                    { ActionType.Unlink, new[] { MessageAction.ProjectUnlinkedCompany, MessageAction.ProjectUnlinkedPerson } }
                },
                new Dictionary<ActionType, MessageAction>()
                {

                    { ActionType.Follow, MessageAction.ProjectFollowed },
                    { ActionType.Unfollow, MessageAction.ProjectUnfollowed },
                    { ActionType.UpdateAccess, MessageAction.ProjectUpdatedMemberRights },
                    { ActionType.Import, MessageAction.ProjectsImportedFromBasecamp }
                }
            }
        };
    }
}

internal class MilestonesActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public MilestonesActionMapper()
    {
        Module = ModuleType.Milestones;

        Actions = new MessageMapsDictionary(ProductType.Projects, Module)
        {
            {
                EntryType.Milestone, new Dictionary<ActionType, MessageAction[]>()
                {
                    { ActionType.Update, new[] { MessageAction.MilestoneUpdated, MessageAction.MilestoneUpdatedStatus } }
                }, new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Create, MessageAction.MilestoneCreated },
                    { ActionType.Delete, MessageAction.MilestoneDeleted }
                }
            }
        };
    }
}

internal class TasksActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public TasksActionMapper()
    {
        Module = ModuleType.Tasks;

        Actions = new MessageMapsDictionary(ProductType.Projects, Module)
        {
            {
                EntryType.Task, new Dictionary<ActionType, MessageAction[]>()
                {
                    { ActionType.Create, new[] { MessageAction.TaskCreated, MessageAction.TaskCreatedFromDiscussion  } },
                    { ActionType.Update, new[] { MessageAction.TaskUpdated, MessageAction.TaskUpdatedStatus, MessageAction.TaskMovedToMilestone, MessageAction.TaskUnlinkedMilestone } }
                },
                new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Delete, MessageAction.TaskDeleted },
                    { ActionType.Follow, MessageAction.TaskUpdatedFollowing },
                    { ActionType.Attach, MessageAction.TaskAttachedFiles },
                    { ActionType.Detach, MessageAction.TaskDetachedFile },
                    { ActionType.Link, MessageAction.TasksLinked },
                    { ActionType.Unlink, MessageAction.TasksUnlinked },
                }
            },
            {
                EntryType.Comment, new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Create, MessageAction.TaskCommentCreated },
                    { ActionType.Update, MessageAction.TaskCommentUpdated },
                    { ActionType.Delete, MessageAction.TaskCommentDeleted }
                }
            },
            {
                EntryType.SubTask, new Dictionary<ActionType, MessageAction[]>()
                {
                    { ActionType.Update, new[] { MessageAction.SubtaskUpdated, MessageAction.SubtaskUpdatedStatus, MessageAction.SubtaskMoved }  }
                },
                new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Create, MessageAction.SubtaskCreated },
                    { ActionType.Delete,MessageAction.SubtaskDeleted  }
                }
            },
        };
    }
}

internal class DiscussionsActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }

    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public DiscussionsActionMapper()
    {
        Module = ModuleType.Discussions;

        Actions = new MessageMapsDictionary(ProductType.Projects, Module)
        {
            {
                EntryType.Message, new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Create, MessageAction.DiscussionCreated },
                    { ActionType.Update, MessageAction.DiscussionUpdated },
                    { ActionType.Delete, MessageAction.DiscussionDeleted },
                    { ActionType.Follow, MessageAction.DiscussionUpdatedFollowing },
                    { ActionType.Attach, MessageAction.DiscussionAttachedFiles },
                    { ActionType.Detach, MessageAction.DiscussionDetachedFile }
                }
            },
            {
                EntryType.Comment, new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Create, MessageAction.DiscussionCommentCreated },
                    { ActionType.Update, MessageAction.DiscussionCommentUpdated },
                    { ActionType.Delete, MessageAction.DiscussionCommentDeleted }
                }
            },
        };
    }
}

internal class TimeTrackingActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public TimeTrackingActionMapper()
    {
        Module = ModuleType.TimeTracking;

        Actions = new MessageMapsDictionary(ProductType.Projects, Module)
        {
            {
                EntryType.TimeSpend, new Dictionary<ActionType, MessageAction[]>()
                {
                    { ActionType.Update, new[] { MessageAction.TaskTimeUpdated, MessageAction.TaskTimesUpdatedStatus } }
                },
                new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Create, MessageAction.TaskTimeCreated },
                    { ActionType.Delete, MessageAction.TaskTimesDeleted },
                }
            }
        };
    }
}

internal class ReportsActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public ReportsActionMapper()
    {
        Module = ModuleType.Reports;

        Actions = new MessageMapsDictionary(ProductType.Projects, Module)
        {
            {
                EntryType.TimeSpend, new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Create, MessageAction.ReportTemplateCreated },
                    { ActionType.Update, MessageAction.ReportTemplateUpdated },
                    { ActionType.Delete, MessageAction.ReportTemplateDeleted },
                }
            }
        };
    }
}

internal class ProjectsSettingsActionMapper : IModuleActionMapper
{
    public ModuleType Module { get; }
    public IDictionary<MessageAction, MessageMaps> Actions { get; }

    public ProjectsSettingsActionMapper()
    {
        Module = ModuleType.ProjectsSettings;

        Actions = new MessageMapsDictionary(ProductType.Projects, Module)
        {
            {
                EntryType.Template, new Dictionary<ActionType, MessageAction>()
                {
                    { ActionType.Create, MessageAction.ProjectTemplateCreated },
                    { ActionType.Update, MessageAction.ProjectTemplateUpdated },
                    { ActionType.Delete, MessageAction.ProjectTemplateDeleted },
                }
            }
        };
    }
}