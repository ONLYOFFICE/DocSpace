namespace ASC.People.Api;

[Scope(Additional = typeof(BaseLoginProviderExtension))]
[DefaultRoute]
[ApiController]
[ControllerName("people")]
public abstract class ApiControllerBase : ControllerBase { }