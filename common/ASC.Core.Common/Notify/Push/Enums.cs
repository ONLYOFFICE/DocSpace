namespace ASC.Core.Common.Notify.Push;

public enum PushAction
{
    Unknown,
    Created,
    Assigned,
    InvitedTo,
    Closed,
    Resumed,
    Deleted
}

public enum PushItemType
{
    Unknown,
    Task,
    Subtask,
    Milestone,
    Project,
    Message
}

public enum PushModule
{
    Unknown,
    Projects
}

public enum MobileAppType
{
    IosProjects = 0,
    AndroidProjects = 1,
    IosDocuments = 2,
    AndroidDocuments = 3,
    DesktopEditor = 4
}
