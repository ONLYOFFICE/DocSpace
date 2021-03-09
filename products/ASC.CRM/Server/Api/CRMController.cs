using ASC.Api.Core;
using ASC.CRM.ApiModels;
using ASC.Api.Documents;
using ASC.Common;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Notify.Signalr;
using ASC.Core.Tenants;
using ASC.Core.Users;
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
using ASC.Web.CRM.Core.Search;
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
    [Scope]
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
                         TaskDtoHelper taskDtoHelper,
                         SecurityContext securityContext,
                         UserManager userManager,
                         UserFormatter userFormatter,
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
                         ContactDtoHelper contactBaseDtoHelper,
                         ContactPhotoManager contactPhotoManager,
                         CommonLinkUtility commonLinkUtility,
                         StorageFactory storageFactory,
                         TenantUtil tenantUtil,
                         SignalrServiceClient signalrServiceClient,
                         VoipEngine voipEngine,

                         Web.Files.Classes.FilesSettingsHelper filesSettingsHelper,
                         ASC.Files.Core.Data.DaoFactory filesDaoFactory,

                         FileWrapperHelper fileWrapperHelper,
                         DisplayUserSettingsHelper displayUserSettingsHelper,
                         OpportunityDtoHelper opportunityDtoHelper,
                         EmployeeWraperHelper employeeWraperHelper,
                         ApiDateTimeHelper apiDateTimeHelper,
                         CurrencyInfoDtoHelper currencyInfoDtoHelper,
                         CurrencyRateInfoDtoHelper currencyRateInfoDtoHelper,
                         CurrencyRateDtoHelper currencyRateDtoHelper,
                         CasesDtoHelper casesDtoHelper,
                         InvoiceDtoHelper invoiceDtoHelper,
                         InvoiceItemDtoHelper invoiceItemDtoHelper,
                         InvoiceBaseDtoHelper invoiceBaseDtoHelper,
                         InvoiceLineDtoHelper invoiceLineDtoHelper,
                         InvoiceTaxDtoHelper invoiceTaxDtoHelper,
                         ContactInfoDtoHelper contactInfoDtoHelper,
                         HistoryCategoryDtoHelper historyCategoryBaseDtoHelper,
                         TaskCategoryDtoHelper taskCategoryDtoHelper,
                         VoipCallDtoHelper voipCallDtoHelper,

                         RelationshipEventDtoHelper relationshipEventDtoHelper,
                         DocbuilderReportsUtilityHelper docbuilderReportsUtilityHelper,

                         FactoryIndexerContactInfo factoryIndexerContactInfo,
                         FactoryIndexerCase factoryIndexerCase,
                         FactoryIndexerFieldValue factoryIndexerFieldValue,

              //           ExportToCsv exportToCsv,
                         ImportFromCSV importFromCSV,
                         ImportFromCSVManager importFromCSVManager)
        {

            VoipEngine = voipEngine;
            SignalrServiceClient = signalrServiceClient;
             

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
            TaskDtoHelper = taskDtoHelper;
            SecurityContext = securityContext;
            UserManager = userManager;
            UserFormatter = userFormatter;
            CurrencyProvider = currencyProvider;
            OpportunityDtoHelper = opportunityDtoHelper;
            EmployeeWraperHelper = employeeWraperHelper;
            ApiDateTimeHelper = apiDateTimeHelper;
            CurrencyInfoDtoHelper = currencyInfoDtoHelper;
            CurrencyRateInfoDtoHelper = currencyRateInfoDtoHelper;
            CurrencyRateDtoHelper = currencyRateDtoHelper;
            CasesDtoHelper = casesDtoHelper;
            ServiceProvider = serviceProvider;

            FactoryIndexerCase = factoryIndexerCase;
            FactoryIndexerFieldValue = factoryIndexerFieldValue;
            FactoryIndexerContactInfo = factoryIndexerContactInfo;

            InvoiceDtoHelper = invoiceDtoHelper;
            InvoiceItemDtoHelper = invoiceItemDtoHelper;
            InvoiceBaseDtoHelper = invoiceBaseDtoHelper;
            InvoiceLineDtoHelper = invoiceLineDtoHelper;
            InvoiceTaxDtoHelper = invoiceTaxDtoHelper;
            RelationshipEventDtoHelper = relationshipEventDtoHelper;
            FileWrapperHelper = fileWrapperHelper;
            OrganisationLogoManager = organisationLogoManager;

            SetupInfo = setupInfo;
            FileSizeComment = fileSizeComment;
            TenantManager = tenantManager;

            AuthManager = authManager;
            PdfCreator = pdfCreator;
            FilesDaoFactory = filesDaoFactory;

            FilesSettingsHelper = filesSettingsHelper;
            FileUploader = fileUploader;
            Global = global;
            ReportHelper = reportHelper;
            HttpContextAccessor = httpContextAccessor;
            DocbuilderReportsUtilityHelper = docbuilderReportsUtilityHelper;
            InvoiceSetting = invoiceSetting;

            ContactInfoDtoHelper = contactInfoDtoHelper;
            HistoryCategoryDtoHelper = historyCategoryBaseDtoHelper;
            ContactDtoHelper = contactBaseDtoHelper;
            VoipCallDtoHelper = voipCallDtoHelper;

            //ExportToCsv = exportToCsv;
            ImportFromCSV = importFromCSV;
            ImportFromCSVManager = importFromCSVManager;
        }

        public ImportFromCSVManager ImportFromCSVManager { get; }
        public TaskCategoryDtoHelper TaskCategoryDtoHelper { get; }
        public HistoryCategoryDtoHelper HistoryCategoryDtoHelper { get; }
        public ContactInfoDtoHelper ContactInfoDtoHelper { get; }
        public VoipEngine VoipEngine { get; }
        public SignalrServiceClient SignalrServiceClient { get; }
        public IVoipProvider VoipProvider { get; }
        public TenantUtil TenantUtil { get; }
        public StorageFactory StorageFactory { get; }
        public CommonLinkUtility CommonLinkUtility { get; }
        public ContactPhotoManager ContactPhotoManager { get; }
        public ContactDtoHelper ContactDtoHelper { get; }
        public VoipCallDtoHelper VoipCallDtoHelper { get; }
        public OrganisationLogoManager OrganisationLogoManager { get; }
        public InvoiceSetting InvoiceSetting { get; }
        public DocbuilderReportsUtilityHelper DocbuilderReportsUtilityHelper { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public ReportHelper ReportHelper { get; }
        public FileUploader FileUploader { get; }
        public Web.Files.Classes.FilesSettingsHelper FilesSettingsHelper { get; }
        public ASC.Files.Core.Data.DaoFactory FilesDaoFactory { get; }
        public FileWrapperHelper FileWrapperHelper { get; }
        public RelationshipEventDtoHelper RelationshipEventDtoHelper { get; }
        public AuthManager AuthManager { get; }
        public FileSizeComment FileSizeComment { get; }
        public SetupInfo SetupInfo { get; }
        public TenantManager TenantManager { get; }
        public PdfCreator PdfCreator { get; }
        public InvoiceTaxDtoHelper InvoiceTaxDtoHelper { get; }
        public InvoiceLineDtoHelper InvoiceLineDtoHelper { get; }
        public InvoiceBaseDtoHelper InvoiceBaseDtoHelper { get; }
        public InvoiceItemDtoHelper InvoiceItemDtoHelper { get; }
        public InvoiceDtoHelper InvoiceDtoHelper { get; }
        public FactoryIndexerContactInfo FactoryIndexerContactInfo { get; }
        public FactoryIndexerFieldValue FactoryIndexerFieldValue { get; }
        public FactoryIndexerCase FactoryIndexerCase { get; }
        public IServiceProvider ServiceProvider { get; }
        public CasesDtoHelper CasesDtoHelper { get; }
        public CurrencyRateDtoHelper CurrencyRateDtoHelper { get; }
        public SettingsManager SettingsManager { get; }
        public Global Global { get; }
        public CurrencyInfoDtoHelper CurrencyInfoDtoHelper { get; }
        public CurrencyRateInfoDtoHelper CurrencyRateInfoDtoHelper { get; }
        public ApiDateTimeHelper ApiDateTimeHelper { get; }
        public EmployeeWraperHelper EmployeeWraperHelper { get; }
        public OpportunityDtoHelper OpportunityDtoHelper { get; }
        public DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        public CurrencyProvider CurrencyProvider { get; }
        public UserFormatter UserFormatter { get; }
        public UserManager UserManager { get; }
        public SecurityContext SecurityContext { get; }
        public TaskDtoHelper TaskDtoHelper { get; }
        public NotifyClient NotifyClient { get; }
        
        private readonly ApiContext ApiContext;        
        public MessageService MessageService { get; }
        public MessageTarget MessageTarget { get; }
        public CRMSecurity CRMSecurity { get; }
        public DaoFactory DaoFactory { get; }
     //   public ExportToCsv ExportToCsv { get; }
        public ImportFromCSV ImportFromCSV { get; }

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
                    var conatct = (entity = DaoFactory.GetContactDao().GetByID(entityId)) as ASC.CRM.Core.Entities.Contact;
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
}
