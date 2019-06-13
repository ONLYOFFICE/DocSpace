using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Web;
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
        public Common.Logging.LogManager LogManager { get; }

        public PeopleController(ASC.Common.Logging.LogManager logManager)
        {
            LogManager = logManager;
        }

        [FormatRoute, FormatRoute(false)]
        public IEnumerable<EmployeeWraper> GetAll()
        {
            return GetByStatus(EmployeeStatus.Active);
        }

        [FormatRoute("status/{status}")]
        public IEnumerable<EmployeeWraper> GetByStatus(EmployeeStatus status)
        {
            if (CoreContext.Configuration.Personal) throw new Exception("Method not available");
            var query = CoreContext.UserManager.GetUsers(status).AsEnumerable();
            return query.Select(x => new EmployeeWraperFull(x, HttpContext));
        }

        [FormatRoute("@self", Order = 1), FormatRoute("@self", false, Order = 1)]
        public EmployeeWraper Self()
        {
            return new EmployeeWraperFull(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID), HttpContext);
        }

        [FormatRoute("{username}", Order = 2)]
        public EmployeeWraperFull GetById(string username)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            var user = CoreContext.UserManager.GetUserByUserName(username);
            if (user.ID == Constants.LostUser.ID)
            {
                Guid userId;
                if (Guid.TryParse(username, out userId))
                {
                    user = CoreContext.UserManager.GetUsers(userId);
                }
                else
                {
                    LogManager.Get("ASC.Api").Error(string.Format("Account {0} сould not get user by name {1}",  SecurityContext.CurrentAccount.ID, username));
                }
            }

            if (user.ID == Constants.LostUser.ID)
            {
                throw new ItemNotFoundException("User not found");
            }

            return new EmployeeWraperFull(user);
        }
    }
}
