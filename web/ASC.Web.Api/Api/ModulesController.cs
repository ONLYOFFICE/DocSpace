using Module = ASC.Api.Core.Module;

namespace ASC.Web.Api.Controllers;

[Scope]
[DefaultRoute]
[ApiController]
public class ModulesController : ControllerBase
{
    private readonly WebItemManagerSecurity _webItemManagerSecurity;

    public ModulesController(
        WebItemManagerSecurity webItemManagerSecurity)
    {
        _webItemManagerSecurity = webItemManagerSecurity;
    }

    [Read]
    public IEnumerable<string> GetAll()
    {
        var result = new List<string>();

        foreach (var a in _webItemManagerSecurity.GetItems(WebZoneType.StartProductList))
        {
            result.Add(a.ApiURL);
        }

        return result;
    }

    [Read("info")]
    public IEnumerable<Module> GetAllWithInfo()
    {
        foreach (var a in _webItemManagerSecurity.GetItems(WebZoneType.StartProductList))
        {
            if(a is Product product)
            {
                product.Init();
                yield return new Module(product);
            }
                
        }
    }
}
