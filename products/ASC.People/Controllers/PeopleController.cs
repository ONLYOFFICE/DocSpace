using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Api.Core;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Employee.Core.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        public Common.Logging.LogManager LogManager { get; }
        public ApiContext ApiContext { get; }

        public PeopleController(ASC.Common.Logging.LogManager logManager)
        {
            LogManager = logManager;
            ApiContext = HttpContext;
        }

        [Read, Read(false)]
        public IEnumerable<EmployeeWraper> GetAll()
        {
            return GetByStatus(EmployeeStatus.Active);
        }

        [Read("status/{status}")]
        public IEnumerable<EmployeeWraper> GetByStatus(EmployeeStatus status)
        {
            if (CoreContext.Configuration.Personal) throw new Exception("Method not available");
            var query = CoreContext.UserManager.GetUsers(status).AsEnumerable();
            return query.Select(x => new EmployeeWraperFull(x, ApiContext));
        }

        [Read("@self"), Read("@self", false)]
        public EmployeeWraper Self()
        {
            return new EmployeeWraperFull(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID), ApiContext);
        }

        [Read("email"), Read("email", false)]
        public EmployeeWraperFull GetByEmail([FromQuery]string email)
        {
            if (CoreContext.Configuration.Personal && !CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsOwner())
                throw new MethodAccessException("Method not available");
            var user = CoreContext.UserManager.GetUserByEmail(email);
            if (user.ID == Constants.LostUser.ID)
            {
                throw new ItemNotFoundException("User not found");
            }

            return new EmployeeWraperFull(user);
        }

        [Read("{username}", Order = int.MaxValue)]
        public EmployeeWraperFull GetById(string username)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            var user = CoreContext.UserManager.GetUserByUserName(username);
            if (user.ID == Constants.LostUser.ID)
            {
                if (Guid.TryParse(username, out var userId))
                {
                    user = CoreContext.UserManager.GetUsers(userId);
                }
                else
                {
                    LogManager.Get("ASC.Api").Error(string.Format("Account {0} сould not get user by name {1}", SecurityContext.CurrentAccount.ID, username));
                }
            }

            if (user.ID == Constants.LostUser.ID)
            {
                throw new ItemNotFoundException("User not found");
            }

            return new EmployeeWraperFull(user);
        }

        [Read("@search/{query}")]
        public IEnumerable<EmployeeWraperFull> GetSearch(string query)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            try
            {
                var groupId = Guid.Empty;
                if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
                {
                    groupId = new Guid(ApiContext.FilterValue);
                }

                return CoreContext.UserManager.Search(query, EmployeeStatus.Active, groupId).Select(x => new EmployeeWraperFull(x));
            }
            catch (Exception error)
            {
                LogManager.Get("ASC.Api").Error(error);
            }
            return null;
        }

        [Read("search"), Read("search", false)]
        public IEnumerable<EmployeeWraperFull> GetPeopleSearch([FromQuery]string query)
        {
            return GetSearch(query);
        }

        [Read("status/{status}/search"), Read("status/{status}/search", false)]
        public IEnumerable<EmployeeWraperFull> GetAdvanced(EmployeeStatus status, [FromQuery]string query)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            try
            {
                var list = CoreContext.UserManager.GetUsers(status).AsEnumerable();

                if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
                {
                    var groupId = new Guid(ApiContext.FilterValue);
                    //Filter by group
                    list = list.Where(x => CoreContext.UserManager.IsUserInGroup(x.ID, groupId));
                    ApiContext.SetDataFiltered();
                }

                list = list.Where(x => x.FirstName != null && x.FirstName.IndexOf(query, StringComparison.OrdinalIgnoreCase) > -1 || (x.LastName != null && x.LastName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) ||
                                       (x.UserName != null && x.UserName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Email != null && x.Email.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Contacts != null && x.Contacts.Any(y => y.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)));

                return list.Select(x => new EmployeeWraperFull(x));
            }
            catch (Exception error)
            {
                LogManager.Get("ASC.Api").Error(error);
            }
            return null;
        }

        ///// <summary>
        ///// Adds a new portal user from import with the first and last name, email address
        ///// </summary>
        ///// <short>
        ///// Add new import user
        ///// </short>
        ///// <param name="userList">The list of users to add</param>
        ///// <param name="importUsersAsCollaborators" optional="true">Add users as guests (bool type: false|true)</param>
        ///// <returns>Newly created users</returns>
        //[Create("import/save")]
        //public void SaveUsers(string userList, bool importUsersAsCollaborators)
        //{
        //    lock (progressQueue.SynchRoot)
        //    {
        //        var task = progressQueue.GetItems().OfType<ImportUsersTask>().FirstOrDefault(t => (int)t.Id == TenantProvider.CurrentTenantID);
        //        if (task != null && task.IsCompleted)
        //        {
        //            progressQueue.Remove(task);
        //            task = null;
        //        }
        //        if (task == null)
        //        {
        //            progressQueue.Add(new ImportUsersTask(userList, importUsersAsCollaborators, GetHttpHeaders(HttpContext.Current.Request))
        //            {
        //                Id = TenantProvider.CurrentTenantID,
        //                UserId = SecurityContext.CurrentAccount.ID,
        //                Percentage = 0
        //            });
        //        }
        //    }
        //}

        //[Read("import/status")]
        //public object GetStatus()
        //{
        //    lock (progressQueue.SynchRoot)
        //    {
        //        var task = progressQueue.GetItems().OfType<ImportUsersTask>().FirstOrDefault(t => (int)t.Id == TenantProvider.CurrentTenantID);
        //        if (task == null) return null;

        //        return new
        //        {
        //            Completed = task.IsCompleted,
        //            Percents = (int)task.Percentage,
        //            UserCounter = task.GetUserCounter,
        //            Status = (int)task.Status,
        //            Error = (string)task.Error,
        //            task.Data
        //        };
        //    }
        //}


        [Read("filter"), Read("filter", false)]
        public IEnumerable<EmployeeWraperFull> GetFullByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);

            return users.Select(u => new EmployeeWraperFull(u, ApiContext));
        }

        [Read("simple/filter"), Read("simple/filter", false)]
        public IEnumerable<EmployeeWraper> GetSimpleByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);

            return users.Select(u => new EmployeeWraper(u));
        }

        private IEnumerable<UserInfo> GetByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
        {
            if (CoreContext.Configuration.Personal) throw new MethodAccessException("Method not available");
            var isAdmin = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin() ||
                          WebItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, SecurityContext.CurrentAccount.ID);
            var status = isAdmin ? EmployeeStatus.All : EmployeeStatus.Default;

            if (employeeStatus != null)
            {
                switch (employeeStatus)
                {
                    case EmployeeStatus.Terminated:
                    case EmployeeStatus.All:
                        status = isAdmin ? (EmployeeStatus)employeeStatus : EmployeeStatus.Default;
                        break;
                    default:
                        status = (EmployeeStatus)employeeStatus;
                        break;
                }
            }

            var users = string.IsNullOrEmpty(ApiContext.FilterValue) ?
                            CoreContext.UserManager.GetUsers(status).AsEnumerable() :
                            CoreContext.UserManager.Search(ApiContext.FilterValue, status).AsEnumerable();

            if (groupId != null && !groupId.Equals(Guid.Empty))
            {
                users = users.Where(x => CoreContext.UserManager.IsUserInGroup(x.ID, (Guid)groupId));
            }
            if (activationStatus != null)
            {
                users = activationStatus == EmployeeActivationStatus.Activated ?
                            users.Where(x => x.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated)) :
                            users.Where(x => x.ActivationStatus == EmployeeActivationStatus.NotActivated ||
                                             x.ActivationStatus == EmployeeActivationStatus.Pending ||
                                             x.ActivationStatus == EmployeeActivationStatus.AutoGenerated);
            }
            if (employeeType != null)
            {
                switch (employeeType)
                {
                    case EmployeeType.User:
                        users = users.Where(x => !x.IsVisitor());
                        break;
                    case EmployeeType.Visitor:
                        users = users.Where(x => x.IsVisitor());
                        break;
                }
            }

            if (isAdministrator.HasValue && isAdministrator.Value)
            {
                users = users.Where(x => x.IsAdmin() || x.GetListAdminModules().Any());
            }

            ApiContext.TotalCount = users.Count();

            switch (ApiContext.SortBy)
            {
                case "firstname":
                    users = ApiContext.SortDescending ? users.OrderByDescending(r => r, UserInfoComparer.FirstName) : users.OrderBy(r => r, UserInfoComparer.FirstName);
                    break;
                case "lastname":
                    users = ApiContext.SortDescending ? users.OrderByDescending(r => r, UserInfoComparer.LastName) : users.OrderBy(r => r, UserInfoComparer.LastName);
                    break;
                default:
                    users = ApiContext.SortDescending ? users.OrderByDescending(r => r, UserInfoComparer.Default) : users.OrderBy(r => r, UserInfoComparer.Default);
                    break;
            }

            users = users.Skip((int)ApiContext.StartIndex).Take((int)ApiContext.Count - 1);

            ApiContext.SetDataSorted();
            ApiContext.SetDataPaginated();

            return users;
        }
    }
}
