using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.ApiHelpers;

public abstract class ApiControllerEngineBase
{
    protected readonly ApiContext ApiContext;
    protected readonly AuthContext AuthContext;
    protected readonly MessageService MessageService;
    protected readonly MessageTarget MessageTarget;
    protected readonly PermissionContext PermissionContext;
    protected readonly SecurityContext SecurityContext;
    protected readonly StudioNotifyService StudioNotifyService;
    protected readonly UserManager UserManager;

    protected ApiControllerEngineBase(
        UserManager userManager,
        AuthContext authContext,
        ApiContext apiContext,
        PermissionContext permissionContext,
        SecurityContext securityContext,
        MessageService messageService,
        MessageTarget messageTarget,
        StudioNotifyService studioNotifyService)
    {
        UserManager = userManager;
        AuthContext = authContext;
        ApiContext = apiContext;
        PermissionContext = permissionContext;
        SecurityContext = securityContext;
        MessageService = messageService;
        MessageTarget = messageTarget;
        StudioNotifyService = studioNotifyService;
    }

    protected Tenant Tenant => ApiContext.Tenant;
}