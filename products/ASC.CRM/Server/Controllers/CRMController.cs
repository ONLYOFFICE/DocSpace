using ASC.Api.Core;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Documents;
using ASC.Common;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Notify.Signalr;
using ASC.Core.Tenants;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.Data.Storage;
using ASC.ElasticSearch;
using ASC.MessagingSystem;
using ASC.VoipService;
using ASC.VoipService.Dao;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Users;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Services.NotifyService;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using Microsoft.AspNetCore.Http;
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
                         CurrencyProvider currencyProvider,
                         Global global,
                         SettingsManager settingsManager,
                         IServiceProvider serviceProvider,
                         PdfCreator pdfCreator,
                         TenantManager tenantManager,
                         SetupInfo setupInfo,
                         FileSizeComment fileSizeComment,
                         AuthManager authManager,
                         FileUploader fileUploader,
                         ReportHelper reportHelper,
                         IHttpContextAccessor httpContextAccessor,
                         InvoiceSetting invoiceSetting,
                         OrganisationLogoManager organisationLogoManager,
                         ContactBaseWrapperHelper contactBaseWrapperHelper,
                         ContactPhotoManager contactPhotoManager,
                         CommonLinkUtility commonLinkUtility,
                         StorageFactory storageFactory,
                         TenantUtil tenantUtil,
                         IVoipProvider voipProvider,
                         SignalrServiceClient signalrServiceClient,
                         VoipEngine voipEngine,

                         Web.Files.Classes.FilesSettings filesSettings,
                         ASC.Files.Core.Data.DaoFactory filesDaoFactory,

                         FileWrapperHelper fileWrapperHelper,
                         DisplayUserSettingsHelper displayUserSettingsHelper,
                         OpportunityWrapperHelper opportunityWrapperHelper,
                         EmployeeWraperHelper employeeWraperHelper,
                         ApiDateTimeHelper apiDateTimeHelper,
                         CurrencyInfoWrapperHelper currencyInfoWrapperHelper,
                         CurrencyRateInfoWrapperHelper currencyRateInfoWrapperHelper,
                         CurrencyRateWrapperHelper currencyRateWrapperHelper,
                         CasesWrapperHelper casesWrapperHelper,
                         InvoiceWrapperHelper invoiceWrapperHelper,
                         InvoiceItemWrapperHelper invoiceItemWrapperHelper,
                         InvoiceBaseWrapperHelper invoiceBaseWrapperHelper,
                         InvoiceLineWrapperHelper invoiceLineWrapperHelper,
                         InvoiceTaxWrapperHelper invoiceTaxWrapperHelper,
                         RelationshipEventWrapperHelper relationshipEventWrapperHelper,
                         DocbuilderReportsUtility docbuilderReportsUtility,

                         FactoryIndexer<Web.CRM.Core.Search.CasesWrapper> factoryIndexerCasesWrapper,
                         FactoryIndexer<Web.CRM.Core.Search.FieldsWrapper> factoryIndexerFieldsWrapper)
        {

            VoipEngine = voipEngine;
            SignalrServiceClient = signalrServiceClient;
            //            voipProvider = VoipDao.GetProvider();

            TenantUtil = tenantUtil;
            StorageFactory = storageFactory;
            CommonLinkUtility = commonLinkUtility;
            ContactPhotoManager = contactPhotoManager;
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
            OpportunityWrapperHelper = opportunityWrapperHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            ApiDateTimeHelper = apiDateTimeHelper;
            CurrencyInfoWrapperHelper = currencyInfoWrapperHelper;
            CurrencyRateInfoWrapperHelper = currencyRateInfoWrapperHelper;
            CurrencyRateWrapperHelper = currencyRateWrapperHelper;
            CasesWrapperHelper = casesWrapperHelper;
            ServiceProvider = serviceProvider;
            FactoryIndexerCasesWrapper = factoryIndexerCasesWrapper;
            FactoryIndexerFieldsWrapper = factoryIndexerFieldsWrapper;
            InvoiceWrapperHelper = invoiceWrapperHelper;
            InvoiceItemWrapperHelper = invoiceItemWrapperHelper;
            InvoiceBaseWrapperHelper = invoiceBaseWrapperHelper;
            InvoiceLineWrapperHelper = invoiceLineWrapperHelper;
            InvoiceTaxWrapperHelper = invoiceTaxWrapperHelper;
            RelationshipEventWrapperHelper = relationshipEventWrapperHelper;
            FileWrapperHelper = fileWrapperHelper;
            OrganisationLogoManager = organisationLogoManager;

            SetupInfo = setupInfo;
            FileSizeComment = fileSizeComment;
            TenantManager = tenantManager;

            AuthManager = authManager;
            PdfCreator = pdfCreator;
            FilesDaoFactory = filesDaoFactory;

            FilesSettings = filesSettings;
            FileUploader = fileUploader;
            Global = global;
            ReportHelper = reportHelper;
            HttpContextAccessor = httpContextAccessor;
            DocbuilderReportsUtility = docbuilderReportsUtility;
            InvoiceSetting = invoiceSetting;

            ContactBaseWrapperHelper = contactBaseWrapperHelper;
        }


        public VoipEngine VoipEngine { get; }
        public SignalrServiceClient SignalrServiceClient { get; }
        public IVoipProvider VoipProvider { get; }
        public TenantUtil TenantUtil { get; }
        public StorageFactory StorageFactory { get; }
        public CommonLinkUtility CommonLinkUtility { get; }
        public ContactPhotoManager ContactPhotoManager { get; }
        public ContactBaseWrapperHelper ContactBaseWrapperHelper { get; }
        public OrganisationLogoManager OrganisationLogoManager { get; }
        public InvoiceSetting InvoiceSetting { get; }
        public DocbuilderReportsUtility DocbuilderReportsUtility { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public ReportHelper ReportHelper { get; }
        public FileUploader FileUploader { get; }
        public Web.Files.Classes.FilesSettings FilesSettings { get; }
        public ASC.Files.Core.Data.DaoFactory FilesDaoFactory { get; }
        public FileWrapperHelper FileWrapperHelper { get; }
        public RelationshipEventWrapperHelper RelationshipEventWrapperHelper { get; }
        public AuthManager AuthManager { get; }
        public FileSizeComment FileSizeComment { get; }
        public SetupInfo SetupInfo { get; }
        public TenantManager TenantManager { get; }
        public PdfCreator PdfCreator { get; }
        public InvoiceTaxWrapperHelper InvoiceTaxWrapperHelper { get; }
        public InvoiceLineWrapperHelper InvoiceLineWrapperHelper { get; }
        public InvoiceBaseWrapperHelper InvoiceBaseWrapperHelper { get; }
        public InvoiceItemWrapperHelper InvoiceItemWrapperHelper { get; }
        public InvoiceWrapperHelper InvoiceWrapperHelper { get; }
        public FactoryIndexer<Web.CRM.Core.Search.FieldsWrapper> FactoryIndexerFieldsWrapper { get; }
        public FactoryIndexer<Web.CRM.Core.Search.CasesWrapper> FactoryIndexerCasesWrapper { get; }
        public IServiceProvider ServiceProvider { get; }
        public CasesWrapperHelper CasesWrapperHelper { get; }
        public CurrencyRateWrapperHelper CurrencyRateWrapperHelper { get; }
        public SettingsManager SettingsManager { get; }
        public Global Global { get; }
        public CurrencyInfoWrapperHelper CurrencyInfoWrapperHelper { get; }
        public CurrencyRateInfoWrapperHelper CurrencyRateInfoWrapperHelper { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }
        public OpportunityWrapperHelper OpportunityWrapperHelper { get; }
        public DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
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
