using Module = ASC.Api.Core.Module;

namespace ASC.Web.Api.Controllers
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private WebItemManagerSecurity WebItemManagerSecurity { get; }

        public ModulesController(
            WebItemManagerSecurity webItemManagerSecurity)
        {
            WebItemManagerSecurity = webItemManagerSecurity;
        }

        [Read]
        public IEnumerable<string> GetAll()
        {
            var result = new List<string>();

            foreach (var a in WebItemManagerSecurity.GetItems(WebZoneType.StartProductList))
            {
                result.Add(a.ApiURL);
            }

            return result;
        }

        [Read("info")]
        public IEnumerable<Module> GetAllWithInfo()
        {
            foreach (var a in WebItemManagerSecurity.GetItems(WebZoneType.StartProductList))
            {
                if(a is Product product)
                {
                    product.Init();
                    yield return new Module(product);
                }
                
            }
        }
    }
}
