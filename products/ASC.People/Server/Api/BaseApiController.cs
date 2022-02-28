using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.Api;

[Scope(Additional = typeof(BaseLoginProviderExtension))]
[DefaultRoute]
[ApiController]
[ControllerName("people")]
public class BaseApiController : ControllerBase
{
    protected Tenant Tenant => ApiContext.Tenant;

    protected readonly UserManager UserManager;
    protected readonly AuthContext AuthContext;
    protected readonly ApiContext ApiContext;
    protected readonly PermissionContext PermissionContext;
    protected readonly SecurityContext SecurityContext;
    protected readonly MessageService MessageService;
    protected readonly MessageTarget MessageTarget;
    protected readonly StudioNotifyService StudioNotifyService;

    protected BaseApiController(
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
}