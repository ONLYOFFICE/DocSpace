namespace ASC.AuditTrail.Types;

/// <summary>
/// </summary>
[EnumExtensions]
public enum EntryType
{
    None,
    File,
    Folder,
    Project,
    Contact,
    Milestone,
    Task,
    Comment,
    SubTask,
    Message,
    TimeSpend,
    ReportTemplate,
    Template,
    Relationship,
    CRMTask,
    Opportunity,
    Invoice,
    Case,
    ListItem,
    InvoiceItem,
    InvoiceTax,
    FieldDescription,
    OpportunityMilestone,
    User,
    Group,
    Room,
    Tag
}
