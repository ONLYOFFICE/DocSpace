using ASC.Core;
using ASC.Web.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Employee.Core.Controllers
{
    [Route("api/2.0/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        [HttpGet("{action}.{format?}")]
        public EmployeeWraper Self()
        {
            return new EmployeeWraper(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID));
        }
    }
}
