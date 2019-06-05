using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Employee.Core.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        [FormatIndexRoute()]
        [FormatIndexRoute(false)]
        public IEnumerable<EmployeeWraper> GetAll()
        {
            return GetByStatus(EmployeeStatus.Active);
        }

        [FormatRoute("status/{status}")]
        public IEnumerable<EmployeeWraper> GetByStatus(EmployeeStatus status)
        {
            if (CoreContext.Configuration.Personal) throw new Exception("Method not available");
            var query = CoreContext.UserManager.GetUsers(status).AsEnumerable();
            return query.Select(x => new EmployeeWraper(x));
        }

        [FormatRoute("{action}")]
        public EmployeeWraper Self()
        {
            return new EmployeeWraper(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID));
        }
    }
}
