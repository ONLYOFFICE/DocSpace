namespace ASC.Core.Common.Notify;

/// <summary>
/// Class that generates 'mail to' addresses to create new TeamLab entities from post client 
/// </summary>
public class ReplyToTagProvider
{
    private static readonly Regex _entityType = new Regex(@"blog|forum.topic|event|photo|file|wiki|bookmark|project\.milestone|project\.task|project\.message");

    private const string TagName = "replyto";

    public ReplyToTagProvider(TenantManager tenantManager, CoreBaseSettings coreBaseSettings, CoreSettings coreSettings)
    {
        _tenantManager = tenantManager;
        _coreBaseSettings = coreBaseSettings;
        _coreSettings = coreSettings;
    }

    /// <summary>
    /// Creates 'replyto' tag that can be used to comment some TeamLab entity
    /// </summary>
    /// <param name="entity">Name of entity e.g. 'blog', 'project.task', etc.</param>
    /// <param name="entityId">Uniq id of the entity</param>
    /// <returns>New TeamLab tag</returns>
    public TagValue Comment(string entity, string entityId)
    {
        return Comment(entity, entityId, null);
    }

    /// <summary>
    /// Creates 'replyto' tag that can be used to comment some TeamLab entity
    /// </summary>
    /// <param name="entity">Name of entity e.g. 'blog', 'project.task', etc.</param>
    /// <param name="entityId">Uniq id of the entity</param>
    /// <param name="parentId">Comment's parent comment id</param>
    /// <returns>New TeamLab tag</returns>
    public TagValue Comment(string entity, string entityId, string parentId)
    {
        if (string.IsNullOrEmpty(entity) || !_entityType.Match(entity).Success)
        {
            throw new ArgumentException(@"Not supported entity type", entity);
        }

        if (string.IsNullOrEmpty(entityId))
        {
            throw new ArgumentException(@"Entity Id is null or empty", entityId);
        }

        var pId = parentId != Guid.Empty.ToString() && parentId != null ? parentId : string.Empty;

        return new TagValue(TagName, $"reply_{entity}_{entityId}_{pId}@{AutoreplyDomain}");
    }

    /// <summary>
    /// Creates 'replyto' tag that can be used to create TeamLab project message
    /// </summary>
    /// <param name="projectId">Id of the project to create message</param>
    /// <returns>New TeamLab tag</returns>
    public TagValue Message(int projectId)
    {
        return new TagValue(TagName, string.Format("message_{0}@{1}", projectId, AutoreplyDomain));
    }

    private string AutoreplyDomain
    {
        get
        {
            // we use mapped domains for standalone portals because it is the only way to reach autoreply service
            // mapped domains are no allowed for SAAS because of http(s) problem
            var tenant = _tenantManager.GetCurrentTenant();

            return tenant.GetTenantDomain(_coreSettings, _coreBaseSettings.Standalone);
        }
    }

    private readonly TenantManager _tenantManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CoreSettings _coreSettings;
}
