// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.VoipService;

public class VoipSettings
{
    public string VoiceUrl { get; set; }
    public string Name { get; set; }
    public List<Agent> Operators { get; set; }
    public Queue Queue { get; set; }
    public Agent Caller { get { return Operators.FirstOrDefault(r => r.Id == AuthContext.CurrentAccount.ID); } }
    public WorkingHours WorkingHours { get; set; }
    public string VoiceMail { get; set; }
    public string GreetingAudio { get; set; }
    public string HoldAudio { get; set; }
    public bool AllowOutgoingCalls { get; set; }
    public bool Pause { get; set; }
    public bool Record { get; set; }
    internal string JsonSettings
    {
        get
        {
            return JsonConvert.SerializeObject(
                new
                {
                    Operators,
                    GreetingAudio,
                    Name,
                    Queue,
                    WorkingHours,
                    VoiceMail,
                    HoldAudio,
                    AllowOutgoingCalls,
                    Pause,
                    Record
                },
                new JsonSerializerSettings { ContractResolver = CustomSerializeContractResolver.Instance });
        }
        set
        {
            try
            {
                var settings = JsonConvert.DeserializeObject<VoipSettings>(value, new JsonSerializerSettings { ContractResolver = CustomSerializeContractResolver.Instance });

                Operators = settings.Operators ?? new List<Agent>();
                Name = settings.Name;
                Queue = settings.Queue;
                WorkingHours = settings.WorkingHours;
                GreetingAudio = settings.GreetingAudio;
                VoiceMail = settings.VoiceMail;
                HoldAudio = settings.HoldAudio;
                AllowOutgoingCalls = settings.AllowOutgoingCalls;
                Pause = settings.Pause;
                Record = settings.Record;
            }
            catch (Exception)
            {

            }

        }
    }

    protected AuthContext AuthContext { get; }
    protected TenantUtil TenantUtil { get; }
    protected SecurityContext SecurityContext { get; }
    protected BaseCommonLinkUtility BaseCommonLinkUtility { get; }

    public VoipSettings(AuthContext authContext, TenantUtil tenantUtil, SecurityContext securityContext, BaseCommonLinkUtility baseCommonLinkUtility)
    {
        Operators = new List<Agent>();
        AuthContext = authContext;
        TenantUtil = tenantUtil;
        SecurityContext = securityContext;
        BaseCommonLinkUtility = baseCommonLinkUtility;
    }

    public VoipSettings(string settings, AuthContext authContext)
    {
        JsonSettings = settings;
        AuthContext = authContext;
    }

    public virtual string Connect(bool user = true, string contactId = null)
    {
        throw new NotImplementedException();
    }

    public virtual string Redirect(string to)
    {
        throw new NotImplementedException();
    }

    public virtual string Dequeue(bool reject)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return JsonSettings;
    }

    public VoipSettings GetSettings(string settings)
    {
        return new VoipSettings(AuthContext, TenantUtil, SecurityContext, BaseCommonLinkUtility) { JsonSettings = settings };
    }
}

class CustomSerializeContractResolver : CamelCasePropertyNamesContractResolver
{
    public static readonly CustomSerializeContractResolver Instance = new CustomSerializeContractResolver();

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (property.PropertyName == "voiceMail")
        {
            property.Converter = new VoiceMailConverter();
        }

        return property;
    }
}

class VoiceMailConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.ValueType != null && reader.ValueType.Name == "String")
        {
            return reader.Value;
        }

        var jObject = JObject.Load(reader);
        var url = jObject.Value<string>("url");

        return !string.IsNullOrEmpty(url) ? url : "";
    }

    public override bool CanConvert(Type objectType)
    {
        return true;
    }
}