using ASC.Api.Core;
using ASC.Api.CRM.Wrappers;
using ASC.Common;
using ASC.Common.Web;
using ASC.Core;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.MessagingSystem;
using ASC.Web.Api.Routing;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Services.NotifyService;
using ASC.Web.Files.Services.WCFService;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ASC.Api.CRM
{
    [DefaultRoute]
    [ApiController]
    public partial class CRMController : ControllerBase
    {
        public CRMController(CRMSecurity cRMSecurity,
                         DaoFactory daoFactory,
                         ApiContext apiContext,
                         MessageTarget messageTarget,
                         MessageService messageService,
                         NotifyClient notifyClient,
                         TaskWrapperHelper taskWrapperHelper,
                         SecurityContext securityContext,
                         UserManager userManager,
                         CurrencyProvider currencyProvider)
        {
            DaoFactory = daoFactory;
            ApiContext = apiContext;
            CRMSecurity = cRMSecurity;
            MessageTarget = messageTarget;
            MessageService = messageService;
            NotifyClient = notifyClient;
            TaskWrapperHelper = taskWrapperHelper;
            SecurityContext = securityContext;
            UserManager = userManager;
            CurrencyProvider = currencyProvider;
        }

        public CurrencyProvider CurrencyProvider { get; }
        public UserManager UserManager { get; }
        public SecurityContext SecurityContext { get; }
        public TaskWrapperHelper TaskWrapperHelper { get; }
        public NotifyClient NotifyClient { get; }
        
        private readonly ApiContext ApiContext;
        
        public MessageService MessageService { get; }
        public MessageTarget MessageTarget { get; }
        public CRMSecurity CRMSecurity { get; }
        public DaoFactory DaoFactory { get; }


        private static EntityType ToEntityType(string entityTypeStr)
        {
            EntityType entityType;

            if (string.IsNullOrEmpty(entityTypeStr)) return EntityType.Any;

            switch (entityTypeStr.ToLower())
            {
                case "person":
                    entityType = EntityType.Person;
                    break;
                case "company":
                    entityType = EntityType.Company;
                    break;
                case "contact":
                    entityType = EntityType.Contact;
                    break;
                case "opportunity":
                    entityType = EntityType.Opportunity;
                    break;
                case "case":
                    entityType = EntityType.Case;
                    break;
                default:
                    entityType = EntityType.Any;
                    break;
            }

            return entityType;
        }

        private string GetEntityTitle(EntityType entityType, int entityId, bool checkAccess, out DomainObject entity)
        {
            switch (entityType)
            {
                case EntityType.Contact:
                case EntityType.Company:
                case EntityType.Person:
                    var conatct = (entity = DaoFactory.GetContactDao().GetByID(entityId)) as Contact;
                    if (conatct == null || (checkAccess && !CRMSecurity.CanAccessTo(conatct)))
                        throw new ItemNotFoundException();
                    return conatct.GetTitle();
                case EntityType.Opportunity:
                    var deal = (entity = DaoFactory.GetDealDao().GetByID(entityId)) as Deal;
                    if (deal == null || (checkAccess && !CRMSecurity.CanAccessTo(deal)))
                        throw new ItemNotFoundException();
                    return deal.Title;
                case EntityType.Case:
                    var cases = (entity = DaoFactory.GetCasesDao().GetByID(entityId)) as Cases;
                    if (cases == null || (checkAccess && !CRMSecurity.CanAccessTo(cases)))
                        throw new ItemNotFoundException();
                    return cases.Title;
                default:
                    throw new ArgumentException("Invalid entityType: " + entityType);
            }
        }
    }

    public static class CRMControllerExtention
    {
        public static DIHelper AddCRMControllerService(this DIHelper services)
        {
            return services.AddCRMSecurityService()
                           .AddDaoFactoryService()
                           .AddApiContextService()
                           .AddGlobalService()
                           .AddMessageTargetService()
                           .AddTaskWrapperHelperService()
                           .AddFileStorageService()
                           .AddNotifyClientService()
                           .AddSecurityContextService()
                           .AddUserManagerService()
                           .AddCurrencyProviderService();
                
        }
    }

}
