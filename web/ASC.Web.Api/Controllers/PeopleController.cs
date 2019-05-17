using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ASC.Employee.Core.Controllers
{
    [FormatFilter]
    [Route("api/2.0/[controller]")]
    [ApiController]
    [Authorize]
    public class PeopleController : ControllerBase
    {
        // GET api/values
        [HttpGet("{action}.{format?}")]
        public ActionResult<IEnumerable<string>> Self()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
