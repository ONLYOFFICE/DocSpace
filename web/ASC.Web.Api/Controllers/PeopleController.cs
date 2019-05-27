using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Core.Users;
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

        [HttpGet("")]
        public IEnumerable<EmployeeWraper> GetAll()
        {
            return GetByStatus(EmployeeStatus.Active);
        }

        [HttpGet("status/{status}.{format?}")]
        public IEnumerable<EmployeeWraper> GetByStatus(EmployeeStatus status)
        {
            if (CoreContext.Configuration.Personal) throw new Exception("Method not available");
            var query = CoreContext.UserManager.GetUsers(status).AsEnumerable();
            return query.Select(x => new EmployeeWraper(x));
        }
    }
}
