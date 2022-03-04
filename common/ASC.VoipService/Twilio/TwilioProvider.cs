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


using RecordingResource = Twilio.Rest.Api.V2010.Account.Call.RecordingResource;


namespace ASC.VoipService.Twilio
{
    public class TwilioProvider : IVoipProvider
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly TwilioRestClient _client;
        private readonly AuthContext _authContext;
        private readonly TenantUtil _tenantUtil;
        private readonly SecurityContext _securityContext;
        private readonly BaseCommonLinkUtility _baseCommonLinkUtility;

        public TwilioProvider(string accountSid, string authToken, AuthContext authContext, TenantUtil tenantUtil, SecurityContext securityContext, BaseCommonLinkUtility baseCommonLinkUtility)
        {
            if (string.IsNullOrEmpty(accountSid)) throw new ArgumentNullException(nameof(accountSid));
            if (string.IsNullOrEmpty(authToken)) throw new ArgumentNullException(nameof(authToken));

            _authToken = authToken;
            _authContext = authContext;
            _tenantUtil = tenantUtil;
            _securityContext = securityContext;
            _baseCommonLinkUtility = baseCommonLinkUtility;
            _accountSid = accountSid;

            _client = new TwilioRestClient(accountSid, authToken);
        }

        #region Call

        public VoipRecord GetRecord(string callId, string recordSid)
        {
            var result = new VoipRecord { Sid = recordSid };
            var count = 6;

            while (count > 0)
            {
                try
                {
                    var record = RecordingResource.Fetch(callId, recordSid, client: _client);

                    if (!record.Price.HasValue)
                    {
                        count--;
                        Thread.Sleep(10000);
                        continue;
                    }

                    result.Price = (-1) * record.Price.Value;

                    result.Duration = Convert.ToInt32(record.Duration);
                    if (record.Uri != null)
                    {
                        result.Uri = record.Uri;
                    }
                    break;
                }
                catch (ApiException)
                {
                    count--;
                    Thread.Sleep(10000);
                }
            }

            return result;
        }

        public void CreateQueue(VoipPhone newPhone)
        {
            newPhone.Settings.Queue = ((TwilioPhone)newPhone).CreateQueue(newPhone.Number, 5, string.Empty, 5);
        }

        #endregion

        #region Numbers

        public VoipPhone BuyNumber(string phoneNumber)
        {
            var newNumber = IncomingPhoneNumberResource.Create(
                new CreateIncomingPhoneNumberOptions
                {
                    PathAccountSid = _accountSid,
                    PhoneNumber = new PhoneNumber(phoneNumber)
                }, _client);

            return new TwilioPhone(_client, _authContext, _tenantUtil, _securityContext, _baseCommonLinkUtility) { Id = newNumber.Sid, Number = phoneNumber.Substring(1) };
        }

        public VoipPhone DeleteNumber(VoipPhone phone)
        {
            IncomingPhoneNumberResource.Delete(phone.Id, client: _client);
            return phone;
        }

        public IEnumerable<VoipPhone> GetExistingPhoneNumbers()
        {
            var result = IncomingPhoneNumberResource.Read(client: _client);
            return result.Select(r => new TwilioPhone(_client, _authContext, _tenantUtil, _securityContext, _baseCommonLinkUtility) { Id = r.Sid, Number = r.PhoneNumber.ToString() });
        }

        public IEnumerable<VoipPhone> GetAvailablePhoneNumbers(PhoneNumberType phoneNumberType, string isoCountryCode)
        {
            return phoneNumberType switch
            {
                PhoneNumberType.Local => LocalResource.Read(isoCountryCode, voiceEnabled: true, client: _client).Select(r => new TwilioPhone(_client, _authContext, _tenantUtil, _securityContext, _baseCommonLinkUtility) { Number = r.PhoneNumber.ToString() }),
                PhoneNumberType.TollFree => TollFreeResource.Read(isoCountryCode, voiceEnabled: true, client: _client).Select(r => new TwilioPhone(_client, _authContext, _tenantUtil, _securityContext, _baseCommonLinkUtility) { Number = r.PhoneNumber.ToString() }),

                _ => new List<VoipPhone>(),
            };
        }

        public VoipPhone GetPhone(string phoneSid)
        {
            var phone = IncomingPhoneNumberResource.Fetch(phoneSid, client: _client);

            var result = new TwilioPhone(_client, _authContext, _tenantUtil, _securityContext, _baseCommonLinkUtility)
            {
                Id = phone.Sid,
                Number = phone.PhoneNumber.ToString(),
                Settings = new TwilioVoipSettings(_authContext, _tenantUtil, _securityContext, _baseCommonLinkUtility)
            };

            if (phone.VoiceUrl == null)
            {
                result.Settings.VoiceUrl = result.Settings.Connect(false);
            }

            return result;
        }

        public VoipPhone GetPhone(VoipNumber data)
        {
            return new TwilioPhone(_client, _authContext, _tenantUtil, _securityContext, _baseCommonLinkUtility)
            {
                Id = data.Id,
                Number = data.Number,
                Alias = data.Alias,
                Settings = new TwilioVoipSettings(data.Settings, _authContext)
            };
        }

        public VoipCall GetCall(string callId)
        {
            var result = new VoipCall { Id = callId };
            var count = 6;

            while (count > 0)
            {
                try
                {
                    var call = CallResource.Fetch(result.Id, client: _client);
                    if (!call.Price.HasValue || string.IsNullOrEmpty(call.Duration))
                    {
                        count--;
                        Thread.Sleep(10000);
                        continue;
                    }

                    result.Price = (-1) * call.Price.Value;
                    result.DialDuration = Convert.ToInt32(call.Duration);
                    break;
                }
                catch (ApiException)
                {
                    count--;
                    Thread.Sleep(10000);
                }
            }

            return result;
        }

        public string GetToken(Agent agent, int seconds = 60 * 60 * 24)
        {
            var scopes = new HashSet<IScope>
            {
                new IncomingClientScope(agent.ClientID)
            };
            var capability = new ClientCapability(_accountSid, _authToken, scopes: scopes);

            return capability.ToJwt();
        }

        public void UpdateSettings(VoipPhone phone)
        {
            IncomingPhoneNumberResource.Update(phone.Id, voiceUrl: new Uri(phone.Settings.Connect(false)), client: _client);
        }

        public void DisablePhone(VoipPhone phone)
        {
            IncomingPhoneNumberResource.Update(phone.Id, voiceUrl: new Uri("https://demo.twilio.com/welcome/voice/"), client: _client);
        }

        #endregion
    }



    public enum PhoneNumberType
    {
        Local,
        /*            Mobile,*/
        TollFree
    }
}