using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;

namespace ASC.Web.Studio.Utility;

//  emp-invite - confirm ivite by email
//  portal-suspend - confirm portal suspending - Tenant.SetStatus(TenantStatus.Suspended)
//  portal-continue - confirm portal continuation  - Tenant.SetStatus(TenantStatus.Active)
//  portal-remove - confirm portal deletation - Tenant.SetStatus(TenantStatus.RemovePending)
//  DnsChange - change Portal Address and/or Custom domain name
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConfirmType
{
    EmpInvite,
    LinkInvite,
    PortalSuspend,
    PortalContinue,
    PortalRemove,
    DnsChange,
    PortalOwnerChange,
    Activation,
    EmailChange,
    EmailActivation,
    PasswordChange,
    ProfileRemove,
    PhoneActivation,
    PhoneAuth,
    Auth,
    TfaActivation,
    TfaAuth,
    Wizard
}
