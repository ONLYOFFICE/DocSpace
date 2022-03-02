using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.ApiHelpers;

public abstract class ApiControllerEngineBase
{
    protected readonly ApiContext _apiContext;
    protected readonly AuthContext _authContext;
    protected readonly MessageService _messageService;
    protected readonly MessageTarget _messageTarget;
    protected readonly PermissionContext _permissionContext;
    protected readonly SecurityContext _securityContext;
    protected readonly StudioNotifyService _studioNotifyService;
    protected readonly UserManager _userManager;

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
        _userManager = userManager;
        _authContext = authContext;
        _apiContext = apiContext;
        _permissionContext = permissionContext;
        _securityContext = securityContext;
        _messageService = messageService;
        _messageTarget = messageTarget;
        _studioNotifyService = studioNotifyService;
    }

    protected Tenant Tenant => _apiContext.Tenant;
}