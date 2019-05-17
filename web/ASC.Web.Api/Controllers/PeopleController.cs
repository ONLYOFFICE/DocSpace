using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Employee.Core.Controllers
{
    [Route("api/2.0/[controller]")]
    [ApiController]
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
