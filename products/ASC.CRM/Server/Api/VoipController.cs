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


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Api.CRM;
using ASC.Api.Utils;
using ASC.Common.Web;
using ASC.Core.Notify.Signalr;
using ASC.Core.Tenants;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.Data.Storage;
using ASC.VoipService;
using ASC.VoipService.Dao;
using ASC.VoipService.Twilio;
using ASC.Web.Api.Routing;
using ASC.Web.CRM.Classes;
using ASC.Web.Studio.Utility;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.CRM.Api
{
    public class VoIPController : BaseApiController
    {
        private readonly SignalrServiceClient _signalrServiceClient;
        private readonly ApiContext _apiContext;
        private readonly VoipEngine _voipEngine;
        private readonly TenantUtil _tenantUtil;
        private readonly SecurityContext _securityContext;
        private readonly CommonLinkUtility _commonLinkUtility;
        private readonly StorageFactory _storageFactory;
        private readonly ContactPhotoManager _contactPhotoManager;
        private readonly Global _global;

        public VoIPController(CrmSecurity crmSecurity,
             DaoFactory daoFactory,
             Global global,
             ContactPhotoManager contactPhotoManager,
             StorageFactory storageFactory,
             CommonLinkUtility commonLinkUtility,
             SecurityContext securityContext,
             TenantUtil tenantUtil,
             VoipEngine voipEngine,
             ApiContext apiContext,
             SignalrServiceClient signalrServiceClient,
             IMapper mapper)
    : base(daoFactory, crmSecurity, mapper)
        {
            _global = global;
            _contactPhotoManager = contactPhotoManager;
            _storageFactory = storageFactory;
            _commonLinkUtility = commonLinkUtility;
            _securityContext = securityContext;
            _tenantUtil = tenantUtil;
            _voipEngine = voipEngine;
            _apiContext = apiContext;
            _signalrServiceClient = signalrServiceClient;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [Read(@"voip/numbers/available")]
        public IEnumerable<VoipPhone> GetAvailablePhoneNumbers(PhoneNumberType numberType, string isoCountryCode)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            if (string.IsNullOrEmpty(isoCountryCode)) throw new ArgumentException();

            return _daoFactory.GetVoipDao().GetProvider().GetAvailablePhoneNumbers(numberType, isoCountryCode);

        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Read(@"voip/numbers/unlinked")]
        public IEnumerable<VoipPhone> GetUnlinkedPhoneNumbers()
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var listPhones = _daoFactory.GetVoipDao().GetProvider().GetExistingPhoneNumbers();
            var buyedPhones = _daoFactory.GetVoipDao().GetNumbers();

            return listPhones.Where(r => buyedPhones.All(b => r.Id != b.Id)).ToList();
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Read(@"voip/numbers/existing")]
        public IEnumerable<VoipPhone> GetExistingPhoneNumbers()
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            return _daoFactory.GetVoipDao().GetNumbers();
        }
        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Create(@"voip/numbers")]
        public Task<VoipPhone> BuyNumberAsync([FromBody] string number)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            return InternalBuyNumberAsync(number);
        }

        private async Task<VoipPhone> InternalBuyNumberAsync(string number)
        {
            var newPhone = _daoFactory.GetVoipDao().GetProvider().BuyNumber(number);

            _daoFactory.GetVoipDao().GetProvider().CreateQueue(newPhone);
            await SetDefaultAudioAsync(newPhone);

            _daoFactory.GetVoipDao().GetProvider().UpdateSettings(newPhone);
            return _daoFactory.GetVoipDao().SaveOrUpdateNumber(newPhone);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Create(@"voip/numbers/link")]
        public Task<VoipPhone> LinkNumberAsync([FromBody] string id)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            return InternalLinkNumberAsync(id);
        }

        private async Task<VoipPhone> InternalLinkNumberAsync([FromBody] string id)
        {
            var newPhone = _daoFactory.GetVoipDao().GetProvider().GetPhone(id);

            _daoFactory.GetVoipDao().GetProvider().CreateQueue(newPhone);
            await SetDefaultAudioAsync(newPhone);

            _daoFactory.GetVoipDao().GetProvider().UpdateSettings(newPhone);

            return _daoFactory.GetVoipDao().SaveOrUpdateNumber(newPhone);
        }

        public async System.Threading.Tasks.Task SetDefaultAudioAsync(VoipPhone newPhone)
        {
            var storage = _storageFactory.GetStorage("", "crm");
            const string path = "default/";
            var files = await storage.ListFilesRelativeAsync("voip", path, "*.*", true)
                               .SelectAwait(async filePath => new 
                               {
                                   path = _commonLinkUtility.GetFullAbsolutePath((await storage.GetUriAsync("voip", Path.Combine(path, filePath))).ToString()),
                                   audioType = (AudioType)Enum.Parse(typeof(AudioType), Directory.GetParent(filePath).Name, true)
                               }).ToListAsync();

            var audio = files.Find(r => r.audioType == AudioType.Greeting);
            newPhone.Settings.GreetingAudio = audio != null ? audio.path : "";

            audio = files.Find(r => r.audioType == AudioType.HoldUp);
            newPhone.Settings.HoldAudio = audio != null ? audio.path : "";

            audio = files.Find(r => r.audioType == AudioType.VoiceMail);
            newPhone.Settings.VoiceMail = audio != null ? audio.path : "";

            audio = files.Find(r => r.audioType == AudioType.Queue);
            newPhone.Settings.Queue.WaitUrl = audio != null ? audio.path : "";
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Delete(@"voip/numbers/{numberId:regex(\w+)}")]
        public VoipPhone DeleteNumber(string numberId)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var dao = _daoFactory.GetVoipDao();
            var phone = dao.GetNumber(numberId).NotFoundIfNull();

            _daoFactory.GetVoipDao().GetProvider().DisablePhone(phone);
            dao.DeleteNumber(numberId);

            new SignalRHelper(phone.Number, _signalrServiceClient).Reload();

            return phone;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Read(@"voip/numbers/{numberId:regex(\w+)}")]
        public VoipPhone GetNumber(string numberId)
        {
            return _daoFactory.GetVoipDao().GetNumber(numberId).NotFoundIfNull();
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        [Read(@"voip/numbers/current")]
        public VoipPhone GetCurrentNumber()
        {
            return _daoFactory.GetVoipDao().GetCurrentNumber().NotFoundIfNull();
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        [Read(@"voip/token")]
        public string GetToken()
        {
            return _daoFactory.GetVoipDao().GetProvider().GetToken(GetCurrentNumber().Caller);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"voip/numbers/{numberId:regex(\w+)}/settings")]
        public VoipPhone UpdateSettings(string numberId, string greeting, string holdUp, string wait, string voiceMail, WorkingHours workingHours, bool? allowOutgoingCalls, bool? record, string alias)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var dao = _daoFactory.GetVoipDao();
            var number = dao.GetNumber(numberId).NotFoundIfNull();

            number.Alias = Update.IfNotEmptyAndNotEquals(number.Alias, alias);
            number.Settings.GreetingAudio = Update.IfNotEmptyAndNotEquals(number.Settings.GreetingAudio, greeting);
            number.Settings.HoldAudio = Update.IfNotEmptyAndNotEquals(number.Settings.HoldAudio, holdUp);
            number.Settings.VoiceMail = Update.IfNotEmptyAndNotEquals(number.Settings.VoiceMail, voiceMail);
            number.Settings.WorkingHours = Update.IfNotEmptyAndNotEquals(number.Settings.WorkingHours, workingHours);

            if (!string.IsNullOrEmpty(wait))
            {
                number.Settings.Queue.WaitUrl = wait;
            }

            if (allowOutgoingCalls.HasValue)
            {
                number.Settings.AllowOutgoingCalls = allowOutgoingCalls.Value;
                if (!number.Settings.AllowOutgoingCalls)
                {
                    number.Settings.Operators.ForEach(r => r.AllowOutgoingCalls = false);
                }
            }

            if (record.HasValue)
            {
                number.Settings.Record = record.Value;
                if (!number.Settings.Record)
                {
                    number.Settings.Operators.ForEach(r => r.Record = false);
                }
            }

            dao.SaveOrUpdateNumber(number);

            return number;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"voip/numbers/settings")]
        public object UpdateSettings(Queue queue, bool pause)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var dao = _daoFactory.GetVoipDao();
            var numbers = dao.GetNumbers();

            if (queue != null)
            {
                foreach (var number in numbers)
                {
                    if (number.Settings.Queue == null || string.IsNullOrEmpty(number.Settings.Queue.Id))
                    {
                        var phone = number as TwilioPhone;
                        if (phone != null)
                        {
                            queue = phone.CreateQueue(phone.Number, queue.Size, queue.WaitUrl, queue.WaitTime * 60);
                        }

                        queue.Name = number.Number;
                        number.Settings.Queue = queue;
                    }
                    else
                    {
                        var oldQueue = number.Settings.Queue;
                        oldQueue.Size = Update.IfNotEmptyAndNotEquals(oldQueue.Size, queue.Size);
                        oldQueue.WaitTime = Update.IfNotEmptyAndNotEquals(oldQueue.WaitTime, queue.WaitTime * 60);
                        oldQueue.WaitUrl = Update.IfNotEmptyAndNotEquals(oldQueue.WaitUrl, queue.WaitUrl);
                    }

                    number.Settings.Pause = pause;

                    dao.SaveOrUpdateNumber(number);
                }
            }

            return new { queue, pause };
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>

        [Read(@"voip/numbers/settings")]
        public Task<object> GetVoipSettingsAsync()
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var dao = _daoFactory.GetVoipDao();
            var number = dao.GetNumbers().FirstOrDefault(r => r.Settings.Queue != null);
            if (number != null)
            {
                return System.Threading.Tasks.Task.FromResult<object>(new { queue = number.Settings.Queue, pause = number.Settings.Pause });
            }

            return InternalGetVoipSettingsAsync();
        }

        private async Task<object> InternalGetVoipSettingsAsync()
        {
            var files = _storageFactory.GetStorage("", "crm").ListFilesAsync("voip", "default/" + nameof(AudioType.Queue).ToLower(), "*.*", true);
            var file = await files.FirstOrDefaultAsync();
            return new { queue = new Queue(null, "Default", 5, file != null ? _commonLinkUtility.GetFullAbsolutePath(file.ToString()) : "", 5), pause = false };
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Read(@"voip/uploads")]
        public IAsyncEnumerable<VoipUpload> GetUploadedFilesUriAsync()
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var result = AsyncEnumerable.Empty<VoipUpload>();

            foreach (var audioType in Enum.GetNames(typeof(AudioType)))
            {
                var type = (AudioType)Enum.Parse(typeof(AudioType), audioType);

                var path = audioType.ToLower();
                var store = _global.GetStore();
                var filePaths = store.ListFilesRelativeAsync("voip", path, "*", true);
                result.Concat(
                    filePaths.SelectAwait(async filePath =>
                                     GetVoipUpload(await store.GetUriAsync("voip", Path.Combine(path, filePath)), Path.GetFileName(filePath), type)));

                path = "default/" + audioType.ToLower();
                store = _storageFactory.GetStorage("", "crm");
                filePaths = store.ListFilesRelativeAsync("voip", path, "*.*", true);
                result.Concat(
                    filePaths.SelectAwait(async filePath =>
                                     GetVoipUpload(await store.GetUriAsync("voip", Path.Combine(path, filePath)), Path.GetFileName(filePath), type, true)));
            }

            return result;
        }

        private VoipUpload GetVoipUpload(Uri link, string fileName, AudioType audioType, bool isDefault = false)
        {
            return new VoipUpload
            {
                Path = _commonLinkUtility.GetFullAbsolutePath(link.ToString()),
                Name = fileName,
                AudioType = audioType,
                IsDefault = isDefault
            };
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Delete(@"voip/uploads")]
        public Task<VoipUpload> DeleteUploadedFileAsync(AudioType audioType, string fileName)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            return InternalDeleteUploadedFileAsync(audioType, fileName);
        }

        private async Task<VoipUpload> InternalDeleteUploadedFileAsync(AudioType audioType, string fileName)
        {
            var store = _global.GetStore();
            var path = Path.Combine(audioType.ToString().ToLower(), fileName);
            var uri = await store.GetUriAsync(path);
            var result = new VoipUpload
            {
                AudioType = audioType,
                Name = fileName,
                Path = _commonLinkUtility.GetFullAbsolutePath(uri.ToString())
            };

            if (!await store.IsFileAsync("voip", path)) throw new ItemNotFoundException();
            await store.DeleteAsync("voip", path);

            var dao = _daoFactory.GetVoipDao();
            var numbers = dao.GetNumbers();

            var defAudio = await _storageFactory.GetStorage("", "crm").ListFilesAsync("voip", "default/" + audioType.ToString().ToLower(), "*.*", true).FirstOrDefaultAsync();
            if (defAudio == null) return result;

            foreach (var number in numbers)
            {
                switch (audioType)
                {
                    case AudioType.Greeting:
                        if (number.Settings.GreetingAudio == result.Path)
                        {
                            number.Settings.GreetingAudio = _commonLinkUtility.GetFullAbsolutePath(defAudio.ToString());
                        }
                        break;
                    case AudioType.HoldUp:
                        if (number.Settings.HoldAudio == result.Path)
                        {
                            number.Settings.HoldAudio = _commonLinkUtility.GetFullAbsolutePath(defAudio.ToString());
                        }
                        break;
                    case AudioType.Queue:
                        var queue = number.Settings.Queue;
                        if (queue != null && queue.WaitUrl == result.Path)
                        {
                            queue.WaitUrl = _commonLinkUtility.GetFullAbsolutePath(defAudio.ToString());
                        }
                        break;
                    case AudioType.VoiceMail:
                        if (number.Settings.VoiceMail == result.Path)
                        {
                            number.Settings.VoiceMail = _commonLinkUtility.GetFullAbsolutePath(defAudio.ToString());
                        }
                        break;
                }

                dao.SaveOrUpdateNumber(number);
            }

            return result;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Read(@"voip/numbers/{numberId:regex(\w+)}/oper")]
        public IEnumerable<Guid> GetOperators(string numberId)
        {
            return _daoFactory.GetVoipDao().GetNumber(numberId).Settings.Operators.Select(r => r.Id);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [Update(@"voip/numbers/{numberId:regex(\w+)}/oper")]
        public IEnumerable<Agent> AddOperators(string numberId, IEnumerable<Guid> operators)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            if (_daoFactory.GetVoipDao().GetNumbers().SelectMany(r => r.Settings.Operators).Any(r => operators.Contains(r.Id)))
            {
                throw new ArgumentException("Duplicate", "operators");
            }

            var dao = _daoFactory.GetVoipDao();
            var phone = dao.GetNumber(numberId);
            var lastOper = phone.Settings.Operators.LastOrDefault();
            var startOperId = lastOper != null ? Convert.ToInt32(lastOper.PostFix) + 1 : 100;

            var addedOperators = operators.Select(o => new Agent(o, AnswerType.Client, phone, (startOperId++).ToString(CultureInfo.InvariantCulture))).ToList();
            phone.Settings.Operators.AddRange(addedOperators);

            dao.SaveOrUpdateNumber(phone);
            return addedOperators;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Delete(@"voip/numbers/{numberId:regex(\w+)}/oper")]
        public Guid DeleteOperator(string numberId, Guid oper)
        {
            if (!_crmSecurity.IsAdmin) throw _crmSecurity.CreateSecurityException();

            var dao = _daoFactory.GetVoipDao();
            var phone = dao.GetNumber(numberId);
            var startOperId = 100;

            phone.Settings.Operators.RemoveAll(r => r.Id == oper);
            phone.Settings.Operators.ToList()
                 .ForEach(r =>
                     {
                         r.PhoneNumber = phone.Number;
                         r.PostFix = startOperId.ToString(CultureInfo.InvariantCulture);
                         startOperId++;
                     });

            dao.SaveOrUpdateNumber(phone);
            return oper;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"voip/opers/{operatorId}")]
        public Agent UpdateOperator(Guid operatorId, AgentStatus? status, bool? allowOutgoingCalls, bool? record, AnswerType? answerType, string redirectToNumber)
        {
            if (!_crmSecurity.IsAdmin && !operatorId.Equals(_securityContext.CurrentAccount.ID)) throw _crmSecurity.CreateSecurityException();

            var dao = _daoFactory.GetVoipDao();
            var phone = dao.GetNumbers().FirstOrDefault(r => r.Settings.Operators.Exists(a => a.Id == operatorId)).NotFoundIfNull();

            var oper = phone.Settings.Operators.Find(r => r.Id == operatorId);

            if (status.HasValue)
            {
                oper.Status = status.Value;
            }

            if (allowOutgoingCalls.HasValue)
            {
                oper.AllowOutgoingCalls = phone.Settings.AllowOutgoingCalls && allowOutgoingCalls.Value;
            }

            if (record.HasValue)
            {
                oper.Record = phone.Settings.Record && record.Value;
            }

            if (answerType.HasValue)
            {
                oper.Answer = answerType.Value;
            }

            if (!string.IsNullOrEmpty(redirectToNumber))
            {
                oper.RedirectToNumber = redirectToNumber;
            }

            dao.SaveOrUpdateNumber(phone);

            if (allowOutgoingCalls.HasValue)
            {
                new SignalRHelper(phone.Number, _signalrServiceClient).Reload(operatorId.ToString());
            }

            return oper;
        }


        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        /// <exception cref="SecurityException"></exception>
        [Create(@"voip/call")]
        public Task<VoipCallDto> MakeCallAsync([FromBody] CreateMakeCallRequestDto inDto)
        {
            var number = _daoFactory.GetVoipDao().GetCurrentNumber().NotFoundIfNull();

            if (!number.Settings.Caller.AllowOutgoingCalls) throw new SecurityException(CRMErrorsResource.AccessDenied);

            return InternalMakeCallAsync(inDto, number);

        }

        private async Task<VoipCallDto> InternalMakeCallAsync(CreateMakeCallRequestDto inDto, VoipPhone number)
        {
            var to = inDto.To;
            var contactId = inDto.ContactId;

            var contactPhone = to.TrimStart('+');

            ContactDto contact;

            if (String.IsNullOrEmpty(contactId))
            {
                var ids = _daoFactory.GetContactDao().GetContactIDsByContactInfo(ContactInfoType.Phone, contactPhone, null, null);

                contact = _daoFactory.GetContactDao().GetContacts(ids.ToArray()).ConvertAll(x => _mapper.Map<ContactDto>(x)).FirstOrDefault();

            }
            else
            {
                contact = _mapper.Map<ContactDto>(_daoFactory.GetContactDao().GetByID(Convert.ToInt32(contactId)));
            }

            if (contact == null)
            {
                contact = _mapper.Map<ContactDto>(_voipEngine.CreateContact(contactPhone));
            }

            contact = await GetContactWithFotosAsync(contact);

            var call = number.Call(to, contact.Id.ToString(CultureInfo.InvariantCulture));

            return _mapper.Map<VoipCallDto>(call);

        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        [Create(@"voip/call/{callId:regex(\w+)}/answer")]
        public VoipCallDto AnswerCall([FromRoute] string callId)
        {
            var dao = _daoFactory.GetVoipDao();
            var call = dao.GetCall(callId).NotFoundIfNull();
            var number = dao.GetCurrentNumber().NotFoundIfNull();

            number.AnswerQueueCall(call.Id);

            return _mapper.Map<VoipCallDto>(call);

        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        [Create(@"voip/call/{callId:regex(\w+)}/reject")]
        public VoipCallDto RejectCall([FromRoute] string callId)
        {
            var dao = _daoFactory.GetVoipDao();
            var call = dao.GetCall(callId).NotFoundIfNull();
            var number = dao.GetCurrentNumber().NotFoundIfNull();

            number.RejectQueueCall(call.Id);

            return _mapper.Map<VoipCallDto>(call);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        [Create(@"voip/call/{callId:regex(\w+)}/redirect")]
        public VoipCallDto ReditectCall([FromRoute] string callId, [FromBody] string to)
        {
            var dao = _daoFactory.GetVoipDao();
            var call = dao.GetCall(callId).NotFoundIfNull();
            var number = dao.GetCurrentNumber().NotFoundIfNull();

            if (call.ContactId != 0)
            {
                var contact = _daoFactory.GetContactDao().GetByID(call.ContactId);
                var managers = _crmSecurity.GetAccessSubjectGuidsTo(contact);

                if (!managers.Contains(Guid.Parse(to)))
                {
                    managers.Add(Guid.Parse(to));
                    _crmSecurity.SetAccessTo(contact, managers);
                }
            }

            number.RedirectCall(call.Id, to);

            return _mapper.Map<VoipCallDto>(call);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        [Create(@"voip/call/{callId:regex(\w+)}")]
        public VoipCallDto SaveCall(
            [FromRoute] string callId,
            [FromBody] CreateVoipCallRequestDto inDto)
        {

            var from = inDto.From;
            var to = inDto.To;
            var answeredBy = inDto.AnsweredBy;
            var contactId = inDto.ContactId;
            var status = inDto.Status;
            var price = inDto.Price;

            var dao = _daoFactory.GetVoipDao();

            var call = dao.GetCall(callId) ?? new VoipCall();

            call.Id = callId;
            call.From = Update.IfNotEmptyAndNotEquals(call.From, from);
            call.To = Update.IfNotEmptyAndNotEquals(call.To, to);
            call.AnsweredBy = Update.IfNotEmptyAndNotEquals(call.AnsweredBy, answeredBy);

            try
            {
                if (call.ContactId == 0)
                {
                    var contactPhone = call.Status == VoipCallStatus.Incoming || call.Status == VoipCallStatus.Answered ? call.From : call.To;
                    if (!string.IsNullOrEmpty(contactId))
                    {
                        call.ContactId = Convert.ToInt32(contactId);
                    }
                    else
                    {
                        _voipEngine.GetContact(call);
                    }

                    if (call.ContactId == 0)
                    {
                        contactPhone = contactPhone.TrimStart('+');

                        var peopleInst = new Person
                        {
                            FirstName = contactPhone,
                            LastName = _tenantUtil.DateTimeFromUtc(DateTime.UtcNow).ToString("yyyy-MM-dd hh:mm"),
                            ShareType = ShareType.None
                        };

                        peopleInst.ID = _daoFactory.GetContactDao().SaveContact(peopleInst);

                        _crmSecurity.SetAccessTo(peopleInst, new List<Guid> { _securityContext.CurrentAccount.ID });

                        var person = (PersonDto)_mapper.Map<ContactDto>(peopleInst);

                        _daoFactory.GetContactInfoDao().Save(new ContactInfo { ContactID = person.Id, IsPrimary = true, InfoType = ContactInfoType.Phone, Data = contactPhone });

                        call.ContactId = person.Id;
                    }
                }
            }
            catch (Exception)
            {

            }

            if (status.HasValue)
            {
                call.Status = status.Value;
            }

            if (call.Price == 0 && price.HasValue)
            {
                call.Price = price.Value;
            }

            call = dao.SaveOrUpdateCall(call);

            return _mapper.Map<VoipCallDto>(call);

        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        [Create(@"voip/price/{callId:regex(\w+)}")]
        public void SavePrice([FromRoute] string callId)
        {
            _voipEngine.SaveAdditionalInfo(callId);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        [Read(@"voip/call")]
        public async Task<IEnumerable<VoipCallDto>> GetCallsAsync(string callType, ApiDateTime from, ApiDateTime to, Guid? agent, int? client, int? contactID)
        {
            var voipDao = _daoFactory.GetVoipDao();

            var filter = new VoipCallFilter
            {
                Type = callType,
                FromDate = from != null ? from.UtcTime : (DateTime?)null,
                ToDate = to != null ? to.UtcTime.AddDays(1).AddMilliseconds(-1) : (DateTime?)null,
                Agent = agent,
                Client = client,
                ContactID = contactID,
                SortBy = _apiContext.SortBy,
                SortOrder = !_apiContext.SortDescending,
                SearchText = _apiContext.FilterValue,
                Offset = _apiContext.StartIndex,
                Max = _apiContext.Count,
            };

            _apiContext.SetDataPaginated();
            _apiContext.SetDataFiltered();
            _apiContext.SetDataSorted();
            _apiContext.TotalCount = voipDao.GetCallsCount(filter);

            var defaultSmallPhoto = await _contactPhotoManager.GetSmallSizePhotoAsync(-1, false);
            var calls = voipDao.GetCalls(filter).Select(
                r =>
                    {
                        ContactDto contact;

                        if (r.ContactId != 0)
                        {
                            contact = r.ContactIsCompany
                                          ? (ContactDto)new CompanyDto() { DisplayName = r.ContactTitle, Id = r.ContactId }
                                          : new PersonDto()
                                          {
                                              DisplayName = r.ContactTitle,
                                              Id = r.ContactId
                                          };

                            contact.SmallFotoUrl = _contactPhotoManager.GetSmallSizePhotoAsync(contact.Id, contact.IsCompany).Result;

                        }
                        else
                        {
                            contact = new PersonDto() { SmallFotoUrl = defaultSmallPhoto, Id = -1 };
                        }

                        var item = _mapper.Map<VoipCallDto>(r);

                        item.Contact = contact;

                        return item;

                    }).ToList();

            return calls;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        [Read(@"voip/call/missed")]
        public async Task<IEnumerable<VoipCallDto>> GetMissedCallsAsync()
        {
            var voipDao = _daoFactory.GetVoipDao();
            var defaultSmallPhoto = await _contactPhotoManager.GetSmallSizePhotoAsync(-1, false);

            var calls = voipDao.GetMissedCalls(_securityContext.CurrentAccount.ID, 10, DateTime.UtcNow.AddDays(-7)).Select(
                r =>
                {
                    ContactDto contact;

                    if (r.ContactId != 0)
                    {
                        contact = r.ContactIsCompany
                                      ? (ContactDto)new CompanyDto() { DisplayName = r.ContactTitle, Id = r.ContactId }
                                      : new PersonDto() { DisplayName = r.ContactTitle, Id = r.ContactId };

                        contact.SmallFotoUrl = _contactPhotoManager.GetSmallSizePhotoAsync(contact.Id, contact.IsCompany).Result;

                    }
                    else
                    {
                        contact = new PersonDto() { SmallFotoUrl = defaultSmallPhoto, Id = -1 };
                    }

                    var item = _mapper.Map<VoipCallDto>(r);

                    item.Contact = contact;

                    return item;

                }).ToList();

            _apiContext.SetDataPaginated();
            _apiContext.SetDataFiltered();
            _apiContext.SetDataSorted();
            _apiContext.TotalCount = calls.Count;

            return calls;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <short></short>
        /// <category>Voip</category>
        /// <returns></returns>
        [Read(@"voip/call/{callId:regex(\w+)}")]
        public VoipCallDto GetCall(string callId)
        {
            var call = _daoFactory.GetVoipDao().GetCall(callId);

            _voipEngine.GetContact(call);

            return _mapper.Map<VoipCallDto>(call);
        }

        private async Task<ContactDto> GetContactWithFotosAsync(ContactDto contact)
        {
            contact.SmallFotoUrl = await _contactPhotoManager.GetSmallSizePhotoAsync(contact.Id, contact.IsCompany);
            contact.MediumFotoUrl = await _contactPhotoManager.GetMediumSizePhotoAsync(contact.Id, contact.IsCompany);

            return contact;
        }
    }
}