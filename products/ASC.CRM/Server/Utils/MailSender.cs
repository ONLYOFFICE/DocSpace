/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


#region Import

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Threading;
using ASC.Common.Threading.Progress;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.CRM.Classes;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.Web.Files.Api;

using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using MimeKit;

using File = System.IO.File;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

#endregion

namespace ASC.Web.CRM.Classes
{
    [Transient]
    public class SendBatchEmailsOperation : DistributedTaskProgress, IProgressItem, IDisposable
    {
        private bool _storeInHistory;
        private readonly ILog _log;
        private readonly SMTPServerSetting _smtpSetting;
        private readonly Guid _currUser;
        private readonly int _tenantID;
        private List<int> _contactID;
        private String _subject;
        private String _bodyTempate;
        private List<int> _fileID;
        private int historyCategory;
        private double _exactPercentageValue = 0;

        private DaoFactory _daoFactory;
        private FilesIntegration _filesIntegration;
        private AuthManager _authManager;
        private UserManager _userManager;
        private TenantManager _tenantManager;
        private SecurityContext _securityContext;
        private TenantUtil _tenantUtil;

        public object Error { get; set; }

        private SendBatchEmailsOperation()
        {
        }

        public SendBatchEmailsOperation(
              TenantUtil tenantUtil,
              IOptionsMonitor<ILog> logger,
              Global global,
              SecurityContext securityContext,
              TenantManager tenantManager,
              UserManager userManager,
              AuthManager authManager,
              SettingsManager settingsManager,
              DaoFactory daoFactory,
              CoreConfiguration coreConfiguration,
              FilesIntegration filesIntegration
             )
        {
            _tenantUtil = tenantUtil;
            _securityContext = securityContext;

            Percentage = 0;

            _log = logger.Get("ASC.CRM.MailSender");

            _tenantID = tenantManager.GetCurrentTenant().TenantId;

            var _crmSettings = settingsManager.Load<CrmSettings>();

            _smtpSetting = new SMTPServerSetting(coreConfiguration.SmtpSettings);
            _currUser = _securityContext.CurrentAccount.ID;

            _authManager = authManager;
            _userManager = userManager;
            _daoFactory = daoFactory;
            _tenantManager = tenantManager;
            _filesIntegration = filesIntegration;
        }


        public void Configure(List<int> fileID,
              List<int> contactID,
              String subject,
              String bodyTempate,
              bool storeInHistory)
        {
            _fileID = fileID ?? new List<int>();
            _contactID = contactID ?? new List<int>();
            _subject = subject;
            _bodyTempate = bodyTempate;
            _storeInHistory = storeInHistory;

            SetProperty("RecipientCount", _contactID.Count);
            SetProperty("EstimatedTime", 0);
            SetProperty("DeliveryCount", 0);
        }

        private void AddToHistory(int contactID, String content, DaoFactory _daoFactory)
        {
            if (contactID == 0 || String.IsNullOrEmpty(content)) return;

            var historyEvent = new RelationshipEvent()
            {
                ContactID = contactID,
                Content = content,
                CreateBy = _currUser,
                CreateOn = _tenantUtil.DateTimeNow(),
            };
            if (historyCategory == 0)
            {
                var listItemDao = _daoFactory.GetListItemDao();

                // HACK
                var listItem = listItemDao.GetItems(ListType.HistoryCategory).Find(item => item.AdditionalParams == "event_category_email.png");
                if (listItem == null)
                {
                    listItemDao.CreateItem(
                        ListType.HistoryCategory,
                        new ListItem { AdditionalParams = "event_category_email.png", Title = CRMCommonResource.HistoryCategory_Note });
                }
                historyCategory = listItem.ID;
            }

            historyEvent.CategoryID = historyCategory;

            var relationshipEventDao = _daoFactory.GetRelationshipEventDao();

            historyEvent = relationshipEventDao.CreateItem(historyEvent);

            if (historyEvent.ID > 0 && _fileID != null && _fileID.Count > 0)
            {
                relationshipEventDao.AttachFiles(historyEvent.ID, _fileID.ToArray());
            }
        }

        protected override void DoJob()
        {
            SmtpClient smtpClient = null;
            try
            {
                _tenantManager.SetCurrentTenant(_tenantID);
                _securityContext.AuthenticateMeWithoutCookie(_authManager.GetAccountByID(_tenantID, _currUser));

                smtpClient = GetSmtpClient();

                var userCulture = _userManager.GetUsers(_currUser).GetCulture();

                Thread.CurrentThread.CurrentCulture = userCulture;
                Thread.CurrentThread.CurrentUICulture = userCulture;

                var contactCount = _contactID.Count;

                if (contactCount == 0)
                {
                    Complete();
                    return;
                }

                var from = new MailboxAddress(_smtpSetting.SenderDisplayName, _smtpSetting.SenderEmailAddress);
                var filePaths = new List<string>();
                var fileDao = _filesIntegration.DaoFactory.GetFileDao<int>();

                foreach (var fileID in _fileID)
                {
                    var fileObj = fileDao.GetFileAsync(fileID);
                    if (fileObj == null) continue;
                    using (var fileStream = fileDao.GetFileStreamAsync(fileObj.Result).Result)
                    {
                        var directoryPath = Path.Combine(Path.GetTempPath(), "teamlab", _tenantID.ToString(),
                            "crm/files/mailsender/");

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        var filePath = Path.Combine(directoryPath, fileObj.Result.Title);

                        using (var newFileStream = File.Create(filePath))
                        {
                            fileStream.CopyTo(newFileStream);
                        }

                        filePaths.Add(filePath);

                    }
                }

                var templateManager = new MailTemplateManager(_daoFactory);
                var deliveryCount = 0;

                try
                {
                    Error = string.Empty;
                    foreach (var contactID in _contactID)
                    {
                        _exactPercentageValue += 100.0 / contactCount;
                        Percentage = Math.Round(_exactPercentageValue);
                        PublishChanges();

                        if (IsCompleted) break; // User selected cancel

                        var contactInfoDao = _daoFactory.GetContactInfoDao();

                        var startDate = DateTime.Now;

                        var contactEmails = contactInfoDao.GetList(contactID, ContactInfoType.Email, null, true);
                        if (contactEmails.Count == 0)
                        {
                            continue;
                        }

                        var recipientEmail = contactEmails[0].Data;

                        if (!recipientEmail.TestEmailRegex())
                        {
                            Error += string.Format(CRMCommonResource.MailSender_InvalidEmail, recipientEmail) +
                                     "<br/>";
                            continue;
                        }

                        var to = MailboxAddress.Parse(recipientEmail);

                        var mimeMessage = new MimeMessage
                        {
                            Subject = _subject
                        };

                        mimeMessage.From.Add(from);
                        mimeMessage.To.Add(to);

                        var bodyBuilder = new BodyBuilder
                        {
                            HtmlBody = templateManager.Apply(_bodyTempate, contactID)
                        };

                        foreach (var filePath in filePaths)
                        {
                            bodyBuilder.Attachments.Add(filePath);
                        }

                        mimeMessage.Body = bodyBuilder.ToMessageBody();

                        mimeMessage.Headers.Add("Auto-Submitted", "auto-generated");

                        _log.Debug(GetLoggerRow(mimeMessage));

                        var success = false;

                        try
                        {
                            smtpClient.Send(mimeMessage);

                            success = true;
                        }
                        catch (SmtpCommandException ex)
                        {
                            _log.Error(Error, ex);

                            Error += string.Format(CRMCommonResource.MailSender_FailedDeliverException, recipientEmail) + "<br/>";
                        }

                        if (success)
                        {
                            if (_storeInHistory)
                            {
                                AddToHistory(contactID, string.Format(CRMCommonResource.MailHistoryEventTemplate, mimeMessage.Subject), _daoFactory);
                            }

                            var endDate = DateTime.Now;
                            var waitInterval = endDate.Subtract(startDate);

                            deliveryCount++;

                            var estimatedTime =
                                TimeSpan.FromTicks(waitInterval.Ticks * (_contactID.Count - deliveryCount));

                            SetProperty("RecipientCount", _contactID.Count);
                            SetProperty("EstimatedTime", estimatedTime.ToString());
                            SetProperty("DeliveryCount", deliveryCount);
                        }

                        if (Percentage > 100)
                        {
                            Percentage = 100;
                            PublishChanges();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _log.Debug("cancel mail sender");
                }
                finally
                {
                    foreach (var filePath in filePaths)
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                }

                SetProperty("RecipientCount", _contactID.Count);
                SetProperty("EstimatedTime", TimeSpan.Zero.ToString());
                SetProperty("DeliveryCount", deliveryCount);
            }
            catch (SocketException e)
            {
                Error = e.Message;
                _log.Error(Error);
            }
            finally
            {
                if (smtpClient != null)
                {
                    smtpClient.Dispose();
                }
                Complete();
            }
        }

        public string GetLoggerRow(MimeMessage mailMessage)
        {
            if (mailMessage == null)
                return String.Empty;

            var result = new StringBuilder();

            result.AppendLine("From:" + mailMessage.From);
            result.AppendLine("To:" + mailMessage.To[0]);
            result.AppendLine("Subject:" + mailMessage.Subject);
            result.AppendLine("Body:" + mailMessage.Body);
            result.AppendLine("TenantID:" + _tenantID);

            foreach (var attachment in mailMessage.Attachments)
            {
                result.AppendLine("Attachment: " + attachment.ContentDisposition.FileName);
            }

            return result.ToString();
        }

        public object Clone()
        {
            var cloneObj = new SendBatchEmailsOperation();

            cloneObj.Error = Error;
            cloneObj.Id = Id;
            cloneObj.IsCompleted = IsCompleted;
            cloneObj.Percentage = Percentage;
            cloneObj.Status = Status;

            return cloneObj;
        }

        private void DeleteFiles()
        {
            if (_fileID == null || _fileID.Count == 0) return;

            var fileDao = _filesIntegration.DaoFactory.GetFileDao<int>();

            foreach (var fileID in _fileID)
            {
                var fileObj = fileDao.GetFileAsync(fileID);
                if (fileObj == null) continue;

                fileDao.DeleteFileAsync(fileObj.Result.ID).Wait();
            }

        }

        private SmtpClient GetSmtpClient()
        {
            var client = new SmtpClient
            {
                Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds
            };

            client.Connect(_smtpSetting.Host, _smtpSetting.Port,
                    _smtpSetting.EnableSSL ? SecureSocketOptions.Auto : SecureSocketOptions.None);

            if (_smtpSetting.RequiredHostAuthentication)
            {
                client.Authenticate(_smtpSetting.HostLogin, _smtpSetting.HostPassword);
            }

            return client;
        }

        private void Complete()
        {
            IsCompleted = true;
            Percentage = 100;
            _log.Debug("Completed");
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SendBatchEmailsOperation)) return false;

            var curOperation = (SendBatchEmailsOperation)obj;
            return (curOperation.Id == Id) && (curOperation._tenantID == _tenantID);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ _tenantID.GetHashCode();
        }

        public void Dispose()
        {
            DeleteFiles();
        }
    }

    [Scope]
    public class MailSender
    {
        private readonly Object _syncObj = new Object();
        private readonly DistributedTaskQueue _mailQueue;
        private readonly int quotas = 50;
        private readonly TenantManager _tenantManager;
        private readonly SettingsManager _settingsManager;
        private readonly SendBatchEmailsOperation _sendBatchEmailsOperation;
        private readonly int _tenantID;
        private readonly CoreConfiguration _coreConfiguration;
        private readonly IOptionsMonitor<ILog> _logManager;


        public MailSender(
                          IConfiguration configuration,
                          TenantManager tenantManager,
                          SettingsManager settingsManager,
                          DistributedTaskQueueOptionsManager progressQueueOptionsManager,
                          SendBatchEmailsOperation sendBatchEmailsOperation,
                          CoreConfiguration coreConfiguration,
                          IOptionsMonitor<ILog> logger
            )
        {
            _sendBatchEmailsOperation = sendBatchEmailsOperation;
            _tenantID = tenantManager.GetCurrentTenant().TenantId;
            _mailQueue = progressQueueOptionsManager.Get<SendBatchEmailsOperation>();
            _coreConfiguration = coreConfiguration;
            _logManager = logger;

            int parsed;

            if (int.TryParse(configuration["crm:mailsender:quotas"], out parsed))
            {
                quotas = parsed;
            }

            _tenantManager = tenantManager;
            _settingsManager = settingsManager;

            //            LogManager = logger.Get();
        }

        public int GetQuotas()
        {
            return quotas;
        }

        public IProgressItem Start(List<int> fileID, List<int> contactID, String subject, String bodyTemplate, bool storeInHistory)
        {
            lock (_syncObj)
            {
                var operation = _mailQueue.GetTasks<SendBatchEmailsOperation>().FirstOrDefault(x => Convert.ToInt32(x.Id) == _tenantID);

                if (operation != null && operation.IsCompleted)
                {
                    _mailQueue.RemoveTask(operation.Id);
                    operation = null;
                }

                if (operation == null)
                {
                    if (fileID == null)
                    {
                        fileID = new List<int>();
                    }
                    if (contactID == null || contactID.Count == 0 ||
                        String.IsNullOrEmpty(subject) || String.IsNullOrEmpty(bodyTemplate))
                    {
                        return null;
                    }

                    if (contactID.Count > GetQuotas())
                    {
                        contactID = contactID.Take(GetQuotas()).ToList();
                    }

                    _sendBatchEmailsOperation.Configure(fileID, contactID, subject, bodyTemplate, storeInHistory);

                    _mailQueue.QueueTask(_sendBatchEmailsOperation);
                }

                return operation;
            }
        }

        private SmtpClient GetSmtpClient(SMTPServerSetting smtpSetting)
        {
            var client = new SmtpClient
            {
                Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds
            };

            client.Connect(smtpSetting.Host, smtpSetting.Port,
                    smtpSetting.EnableSSL ? SecureSocketOptions.Auto : SecureSocketOptions.None);

            if (smtpSetting.RequiredHostAuthentication)
            {
                client.Authenticate(smtpSetting.HostLogin, smtpSetting.HostPassword);
            }

            return client;
        }

        public void StartSendTestMail(string recipientEmail, string mailSubj, string mailBody)
        {
            var log = _logManager.Get("ASC.CRM.MailSender");

            if (!recipientEmail.TestEmailRegex())
            {
                throw new Exception(string.Format(CRMCommonResource.MailSender_InvalidEmail, recipientEmail));
            }

            _tenantManager.SetCurrentTenant(_tenantID);
            var smtpSetting = new SMTPServerSetting(_coreConfiguration.SmtpSettings);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    var toAddress = MailboxAddress.Parse(recipientEmail);
                    var fromAddress = new MailboxAddress(smtpSetting.SenderDisplayName, smtpSetting.SenderEmailAddress);

                    var mimeMessage = new MimeMessage
                    {
                        Subject = mailSubj
                    };

                    mimeMessage.From.Add(fromAddress);

                    mimeMessage.To.Add(toAddress);

                    var bodyBuilder = new BodyBuilder
                    {
                        TextBody = mailBody
                    };

                    mimeMessage.Body = bodyBuilder.ToMessageBody();

                    mimeMessage.Headers.Add("Auto-Submitted", "auto-generated");

                    using (var smtpClient = GetSmtpClient(smtpSetting))
                    {
                        smtpClient.Send(FormatOptions.Default, mimeMessage, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            });
        }

        public IProgressItem GetStatus()
        {
            var findedItem = _mailQueue.GetTasks<SendBatchEmailsOperation>().FirstOrDefault(x => Convert.ToInt32(x.Id) == _tenantID);

            return findedItem;
        }

        public void Cancel()
        {
            lock (_syncObj)
            {
                var findedItem = _mailQueue.GetTasks<SendBatchEmailsOperation>().FirstOrDefault(x => Convert.ToInt32(x.Id) == _tenantID);

                if (findedItem == null) return;

                _mailQueue.RemoveTask(findedItem.Id);

            }
        }
    }

    public class MailTemplateTag
    {
        [JsonPropertyName("sysname")]
        public String SysName { get; set; }

        [JsonPropertyName("display_name")]
        public String DisplayName { get; set; }

        [JsonPropertyName("category")]
        public String Category { get; set; }

        [JsonPropertyName("is_company")]
        public bool isCompany { get; set; }

        [JsonPropertyName("name")]
        public String Name { get; set; }
    }

    public class MailTemplateManager
    {
        private readonly Dictionary<String, IEnumerable<MailTemplateTag>> _templateTagsCache = new Dictionary<String, IEnumerable<MailTemplateTag>>();

        private readonly DaoFactory _daoFactory;

        public MailTemplateManager(DaoFactory daoFactory)
        {
            _daoFactory = daoFactory;
        }

        private IEnumerable<MailTemplateTag> GetTagsFrom(String template)
        {
            if (_templateTagsCache.ContainsKey(template)) return _templateTagsCache[template];

            var tags = GetAllTags();

            var result = new List<MailTemplateTag>();

            var _regex = new Regex("\\$\\((Person|Company)\\.[^<>\\)]*\\)");


            if (!_regex.IsMatch(template))
                return new List<MailTemplateTag>();

            foreach (Match match in _regex.Matches(template))
            {
                var findedTag = tags.Find(item => string.Equals(item.Name, match.Value));

                if (findedTag == null) continue;

                if (!result.Contains(findedTag))
                    result.Add(findedTag);
            }

            _templateTagsCache.Add(template, result);

            return result;
        }

        private String Apply(String template, IEnumerable<MailTemplateTag> templateTags, int contactID)
        {
            var result = template;


            var contactDao = _daoFactory.GetContactDao();
            var contactInfoDao = _daoFactory.GetContactInfoDao();
            var customFieldDao = _daoFactory.GetCustomFieldDao();

            var contact = contactDao.GetByID(contactID);

            if (contact == null)
                throw new ArgumentException(CRMErrorsResource.ContactNotFound);

            foreach (var tag in templateTags)
            {
                var tagParts = tag.SysName.Split(new[] { '_' });

                var source = tagParts[0];

                var tagValue = String.Empty;

                switch (source)
                {
                    case "common":

                        if (contact is Person)
                        {

                            var person = (Person)contact;

                            switch (tagParts[1])
                            {

                                case "firstName":
                                    tagValue = person.FirstName;

                                    break;
                                case "lastName":
                                    tagValue = person.LastName;

                                    break;
                                case "jobTitle":
                                    tagValue = person.JobTitle;
                                    break;
                                case "companyName":
                                    var relativeCompany = contactDao.GetByID(((Person)contact).CompanyID);

                                    if (relativeCompany != null)
                                        tagValue = relativeCompany.GetTitle();


                                    break;
                                default:
                                    tagValue = String.Empty;
                                    break;

                            }

                        }
                        else
                        {

                            var company = (Company)contact;

                            switch (tagParts[1])
                            {
                                case "companyName":
                                    tagValue = company.CompanyName;
                                    break;
                                default:
                                    tagValue = String.Empty;
                                    break;
                            }
                        }

                        break;
                    case "customField":
                        var tagID = Convert.ToInt32(tagParts[tagParts.Length - 1]);

                        var entityType = contact is Company ? EntityType.Company : EntityType.Person;

                        tagValue = customFieldDao.GetValue(entityType, contactID, tagID);

                        break;
                    case "contactInfo":
                        var contactInfoType = (ContactInfoType)Enum.Parse(typeof(ContactInfoType), tagParts[1]);
                        var category = Convert.ToInt32(tagParts[2]);
                        var contactInfos = contactInfoDao.GetList(contactID, contactInfoType, category, true);

                        if (contactInfos == null || contactInfos.Count == 0) break;

                        var contactInfo = contactInfos[0];

                        if (contactInfoType == ContactInfoType.Address)
                        {
                            var addressPart = (AddressPart)Enum.Parse(typeof(AddressPart), tagParts[3]);

                            tagValue = JsonDocument.Parse(contactInfo.Data).RootElement.GetProperty(addressPart.ToString().ToLower()).GetString();
                        }
                        else
                            tagValue = contactInfo.Data;

                        break;
                    default:
                        throw new ArgumentException(tag.SysName);
                }

                result = result.Replace(tag.Name, tagValue);
            }

            return result;
        }

        public String Apply(String template, int contactID)
        {
            return Apply(template, GetTagsFrom(template), contactID);
        }

        private String ToTagName(String value, bool isCompany)
        {
            return $"$({(isCompany ? "Company" : "Person")}.{value})";
        }

        private List<MailTemplateTag> GetAllTags()
        {
            return GetTags(true).Union(GetTags(false)).ToList();
        }

        public List<MailTemplateTag> GetTags(bool isCompany)
        {
            var result = new List<MailTemplateTag>();

            if (isCompany)
            {

                result.Add(new MailTemplateTag
                {
                    DisplayName = CRMContactResource.CompanyName,
                    SysName = "common_companyName",
                    Category = CRMContactResource.GeneralInformation,
                    isCompany = isCompany,
                    Name = ToTagName("Company Name", isCompany)
                });

            }
            else
            {
                result.Add(new MailTemplateTag
                {
                    DisplayName = CRMContactResource.FirstName,
                    SysName = "common_firstName",
                    Category = CRMContactResource.GeneralInformation,
                    isCompany = false,
                    Name = ToTagName("First Name", isCompany)
                });

                result.Add(new MailTemplateTag
                {
                    DisplayName = CRMContactResource.LastName,
                    SysName = "common_lastName",
                    Category = CRMContactResource.GeneralInformation,
                    isCompany = false,
                    Name = ToTagName("Last Name", isCompany)
                });

                result.Add(new MailTemplateTag
                {
                    DisplayName = CRMContactResource.JobTitle,
                    SysName = "common_jobTitle",
                    Category = CRMContactResource.GeneralInformation,
                    isCompany = false,
                    Name = ToTagName("Job Title", isCompany)
                });


                result.Add(new MailTemplateTag
                {
                    DisplayName = CRMContactResource.CompanyName,
                    SysName = "common_companyName",
                    Category = CRMContactResource.GeneralInformation,
                    isCompany = false,
                    Name = ToTagName("Company Name", isCompany)
                });

            }

            foreach (ContactInfoType infoTypeEnum in Enum.GetValues(typeof(ContactInfoType)))
            {

                var localName = String.Format("contactInfo_{0}_{1}", infoTypeEnum, ContactInfo.GetDefaultCategory(infoTypeEnum));
                var localTitle = infoTypeEnum.ToLocalizedString();

                if (infoTypeEnum == ContactInfoType.Address)
                    foreach (AddressPart addressPartEnum in Enum.GetValues(typeof(AddressPart)))
                        result.Add(new MailTemplateTag
                        {
                            SysName = String.Format(localName + "_{0}_{1}", addressPartEnum, (int)AddressCategory.Work),
                            DisplayName = String.Format(localTitle + " {0}", addressPartEnum.ToLocalizedString()),
                            Category = CRMContactResource.GeneralInformation,
                            isCompany = isCompany,
                            Name = ToTagName($"{infoTypeEnum} {addressPartEnum}", isCompany)
                        });
                else
                    result.Add(new MailTemplateTag
                    {
                        SysName = localName,
                        DisplayName = localTitle,
                        Category = CRMContactResource.GeneralInformation,
                        isCompany = isCompany,
                        Name = ToTagName(infoTypeEnum.ToString(), isCompany)
                    });
            }

            var entityType = isCompany ? EntityType.Company : EntityType.Person;

            var customFieldsDao = _daoFactory.GetCustomFieldDao();

            var customFields = customFieldsDao.GetFieldsDescription(entityType);

            var category = CRMContactResource.GeneralInformation;

            foreach (var customField in customFields)
            {
                if (customField.Type == CustomFieldType.SelectBox) continue;
                if (customField.Type == CustomFieldType.CheckBox) continue;

                if (customField.Type == CustomFieldType.Heading)
                {
                    if (!String.IsNullOrEmpty(customField.Label))
                        category = customField.Label;

                    continue;
                }

                result.Add(new MailTemplateTag
                {
                    SysName = "customField_" + customField.ID,
                    DisplayName = customField.Label.HtmlEncode(),
                    Category = category,
                    isCompany = isCompany,
                    Name = ToTagName(customField.Label, isCompany)
                });
            }

            return result;
        }

    }
}