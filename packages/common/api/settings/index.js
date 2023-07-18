import { request } from "../client";
import axios from "axios";

export function getSettings(withPassword = false) {
  return request({
    method: "get",
    url: `/settings?withPassword=${withPassword}`,
  });
}

export function getPortalCultures() {
  return request({
    method: "get",
    url: "/settings/cultures",
  });
}

export function getPortalPasswordSettings(confirmKey = null) {
  const options = {
    method: "get",
    url: "/settings/security/password",
  };

  if (confirmKey) options.headers = { confirm: confirmKey };

  return request(options);
}

export function setPortalPasswordSettings(
  minLength,
  upperCase,
  digits,
  specSymbols
) {
  return request({
    method: "put",
    url: "/settings/security/password",
    data: { minLength, upperCase, digits, specSymbols },
  });
}

export function setMailDomainSettings(data) {
  return request({
    method: "post",
    url: "/settings/maildomainsettings",
    data,
  });
}

// export function setDNSSettings(dnsName, enable) {
//   return request({
//     method: "post",
//     url: "/settings/maildomainsettings",
//     data: { dnsName, enable },
//   });
// }

export function setDNSSettings(dnsName, enable) {
  return request({
    method: "put",
    url: "/settings/dns",
    data: { dnsName, enable },
  });
}

export function getIpRestrictions() {
  return request({
    method: "get",
    url: "/settings/iprestrictions",
  });
}

export function setIpRestrictions(data) {
  return request({
    method: "put",
    url: "/settings/iprestrictions",
    data,
  });
}

export function getIpRestrictionsEnable() {
  return request({
    method: "get",
    url: "/settings/iprestrictions/settings",
  });
}

export function setIpRestrictionsEnable(data) {
  return request({
    method: "put",
    url: "/settings/iprestrictions/settings",
    data,
  });
}

export function setMessageSettings(turnOn) {
  return request({
    method: "post",
    url: "/settings/messagesettings",
    data: { turnOn },
  });
}

export function setCookieSettings(lifeTime, enabled) {
  return request({
    method: "put",
    url: "/settings/cookiesettings",
    data: { lifeTime, enabled },
  });
}

export function getCookieSettings() {
  return request({
    method: "get",
    url: "/settings/cookiesettings",
  });
}

export function setLifetimeAuditSettings(data) {
  return request({
    method: "post",
    url: "/security/audit/settings/lifetime",
    data,
  });
}

export function getLoginHistoryReport() {
  return request({
    method: "post",
    url: "/security/audit/login/report",
  });
}

export function getAuditTrailReport() {
  return request({
    method: "post",
    url: "/security/audit/events/report",
  });
}

export function getPortalTimezones(confirmKey = null) {
  const options = {
    method: "get",
    url: "/settings/timezones",
  };

  if (confirmKey) options.headers = { confirm: confirmKey };

  return request(options);
}

export function setLanguageAndTime(lng, timeZoneID) {
  return request({
    method: "put",
    url: "/settings/timeandlanguage",
    data: { lng, timeZoneID },
  });
}

export function setGreetingSettings(title) {
  return request({
    method: "post",
    url: `/settings/greetingsettings`,
    data: { title },
  });
}

export function getGreetingSettingsIsDefault() {
  return request({
    method: "get",
    url: `/settings/greetingsettings/isDefault`,
  });
}

export function restoreGreetingSettings() {
  return request({
    method: "post",
    url: `/settings/greetingsettings/restore`,
  });
}

export function getAppearanceTheme() {
  return request({
    method: "get",
    url: "/settings/colortheme",
  });
}

export function sendAppearanceTheme(data) {
  return request({
    method: "put",
    url: "/settings/colortheme",
    data,
  });
}

export function deleteAppearanceTheme(id) {
  return request({
    method: "delete",
    url: `/settings/colortheme?id=${id}`,
  });
}

export function getLogoText() {
  return request({
    method: "get",
    url: `/settings/whitelabel/logotext`,
  });
}

export function getLogoUrls() {
  return request({
    method: "get",
    url: `/settings/whitelabel/logos`,
  });
}

export function setWhiteLabelSettings(data) {
  const options = {
    method: "post",
    url: "/settings/whitelabel/save",
    data,
  };

  return request(options);
}

export function restoreWhiteLabelSettings(isDefault) {
  return request({
    method: "put",
    url: "/settings/whitelabel/restore",
    data: { isDefault },
  });
}

export function setCompanyInfoSettings(
  address,
  companyName,
  email,
  phone,
  site
) {
  const data = {
    settings: { address, companyName, email, phone, site },
  };

  return request({
    method: "post",
    url: `/settings/rebranding/company`,
    data,
  });
}

export function getCompanyInfoSettings() {
  return request({
    method: "get",
    url: `/settings/rebranding/company`,
  });
}

export function restoreCompanyInfoSettings() {
  return request({
    method: "delete",
    url: `/settings/rebranding/company`,
  });
}

export function getCustomSchemaList() {
  return request({
    method: "get",
    url: `settings/customschemas`,
  });
}

export function setAdditionalResources(
  feedbackAndSupportEnabled,
  videoGuidesEnabled,
  helpCenterEnabled
) {
  const data = {
    settings: {
      helpCenterEnabled,
      feedbackAndSupportEnabled,
      videoGuidesEnabled,
    },
  };

  return request({
    method: "post",
    url: `/settings/rebranding/additional`,
    data,
  });
}

export function getAdditionalResources() {
  return request({
    method: "get",
    url: `/settings/rebranding/additional`,
  });
}

export function restoreAdditionalResources() {
  return request({
    method: "delete",
    url: `/settings/rebranding/additional`,
  });
}

export function setCurrentSchema(id) {
  return request({
    method: "post",
    url: "settings/customschemas",
    data: { id },
  });
}
export function setCustomSchema(
  userCaption,
  usersCaption,
  groupCaption,
  groupsCaption,
  userPostCaption,
  regDateCaption,
  groupHeadCaption,
  guestCaption,
  guestsCaption
) {
  const data = {
    userCaption,
    usersCaption,
    groupCaption,
    groupsCaption,
    userPostCaption,
    regDateCaption,
    groupHeadCaption,
    guestCaption,
    guestsCaption,
  };
  return request({
    method: "put",
    url: `settings/customschemas`,
    data,
  });
}

export function getCurrentCustomSchema(id) {
  return request({
    method: "get",
    url: `settings/customschemas/${id}`,
  });
}

export function sendRecoverRequest(email, message) {
  const data = { email, message };
  return request({
    method: "post",
    url: `/settings/sendadmmail`,
    data,
  });
}

export function sendRegisterRequest(email) {
  const data = { email };
  return request({
    method: "post",
    url: `/settings/sendjoininvite`,
    data,
  });
}

export function sendOwnerChange(ownerId) {
  const data = { ownerId };
  return request({
    method: "post",
    url: `/settings/owner`,
    data,
  });
}

export function ownerChange(ownerId, confirmKey = null) {
  const data = { ownerId };

  const options = {
    method: "put",
    url: `/settings/owner`,
    data,
  };

  if (confirmKey) options.headers = { confirm: confirmKey };

  return request(options);
}

export function getMachineName(confirmKey = null) {
  const options = {
    method: "get",
    url: "/settings/machine",
  };

  if (confirmKey) options.headers = { confirm: confirmKey };

  return request(options);
}

export function setPortalOwner(
  email,
  hash,
  lng,
  timeZone,
  confirmKey = null,
  analytics
) {
  const options = {
    method: "put",
    url: "/settings/wizard/complete",
    data: {
      email: email,
      PasswordHash: hash,
      lng: lng,
      timeZone: timeZone,
      analytics: analytics,
    },
  };

  if (confirmKey) {
    options.headers = { confirm: confirmKey };
  }
  return request(options);
}

export function getIsLicenseRequired() {
  return request({
    method: "get",
    url: "/settings/license/required",
  });
}

export function setLicense(confirmKey = null, data) {
  const options = {
    method: "post",
    url: `/settings/license`,
    data,
  };

  if (confirmKey) {
    options.headers = { confirm: confirmKey };
  }

  return request(options);
}

export function getPaymentSettings() {
  return request({
    method: "get",
    url: `/settings/payment`,
  });
}
export function acceptLicense() {
  return request({
    method: "post",
    url: `/settings/license/accept`,
  });
}
export function getConsumersList() {
  return request({
    method: "get",
    url: `/settings/authservice`,
  });
}

export function getAuthProviders() {
  return request({
    method: "get",
    url: `/people/thirdparty/providers`,
  });
}

export function updateConsumerProps(newProps) {
  const options = {
    method: "post",
    url: `/settings/authservice`,
    data: newProps,
  };

  return request(options);
}

export function getTfaSettings() {
  return request({
    method: "get",
    url: `/settings/tfaapp`,
  });
}

export function setTfaSettings(type) {
  return request({
    method: "put",
    url: "/settings/tfaappwithlink",
    data: { type: type },
  });
}

export function getTfaBackupCodes() {
  return request({
    method: "get",
    url: "/settings/tfaappcodes",
  });
}

export function getTfaNewBackupCodes() {
  return request({
    method: "put",
    url: "/settings/tfaappnewcodes",
  });
}

export function getTfaConfirmLink() {
  return request({
    method: "get",
    url: "/settings/tfaapp/confirm",
  });
}

export function unlinkTfaApp(id) {
  const data = {
    id,
  };
  return request({
    method: "put",
    url: "/settings/tfaappnewapp",
    data,
  });
}

export function getTfaSecretKeyAndQR(confirmKey = null) {
  const options = {
    method: "get",
    url: "/settings/tfaapp/setup",
  };

  if (confirmKey) options.headers = { confirm: confirmKey };

  return request(options);
}

export function validateTfaCode(code, confirmKey = null) {
  const data = {
    code,
  };

  const options = {
    method: "post",
    url: "/settings/tfaapp/validate",
    skipLogout: true,
    data,
  };

  if (confirmKey) options.headers = { confirm: confirmKey };

  return request(options);
}

export function getBackupStorage() {
  const options = {
    method: "get",
    url: "/settings/storage/backup",
  };
  return request(options);
}

export function getBuildVersion() {
  const options = {
    method: "get",
    url: "/settings/version/build",
  };
  return request(options);
}

export function getCapabilities() {
  const options = {
    method: "get",
    url: "/capabilities",
  };
  return request(options);
}

export function getTipsSubscription() {
  const options = {
    method: "get",
    url: "/settings/tips/subscription",
  };
  return request(options);
}

export function getNotificationSubscription(notificationType) {
  const options = {
    method: "get",
    url: `/settings/notification/${notificationType}`,
  };
  return request(options);
}

export function changeNotificationSubscription(notificationType, isEnabled) {
  const data = {
    Type: notificationType,
    isEnabled,
  };
  const options = {
    method: "post",
    url: "/settings/notification",
    data,
  };

  return request(options);
}

export function toggleTipsSubscription() {
  const options = {
    method: "put",
    url: "/settings/tips/change/subscription",
  };
  return request(options);
}

export function getCurrentSsoSettings() {
  const options = {
    method: "get",
    url: "/settings/ssov2",
  };

  return request(options);
}

export function submitSsoForm(data) {
  const options = {
    method: "post",
    url: "/settings/ssov2",
    data,
  };

  return request(options);
}

export function resetSsoForm() {
  const options = {
    method: "delete",
    url: "/settings/ssov2",
  };

  return request(options);
}

export function getLifetimeAuditSettings(data) {
  return request({
    method: "get",
    url: "/security/audit/settings/lifetime",
    data,
  });
}

export function getLoginHistory() {
  return request({
    method: "get",
    url: "/security/audit/login/last",
  });
}

export function getAuditTrail() {
  return request({
    method: "get",
    url: "/security/audit/events/last",
  });
}

export function loadXmlMetadata(data) {
  return axios.post("/sso/loadmetadata", data);
}

export function uploadXmlMetadata(data) {
  return axios.post("/sso/uploadmetadata", data);
}

export function validateCerts(data) {
  return axios.post("/sso/validatecerts", data);
}

export function generateCerts() {
  return axios.get("/sso/generatecert");
}

export function getMetadata() {
  return axios.get("/sso/metadata");
}

export function getOforms(url) {
  return axios.get(url);
}

export function getStorageRegions() {
  const options = {
    method: "get",
    url: "/settings/storage/s3/regions",
  };
  return request(options);
}

export function getPortalQuota() {
  return request({
    method: "get",
    url: `/settings/quota`,
  });
}

export function getAllActiveSessions() {
  return request({
    method: "get",
    url: "/security/activeconnections",
  });
}

export function removeAllActiveSessions() {
  return request({
    method: "put",
    url: "/security/activeconnections/logoutallchangepassword",
  });
}

export function removeAllExceptThisSession() {
  return request({
    method: "put",
    url: "/security/activeconnections/logoutallexceptthis",
  });
}

export function removeActiveSession(eventId) {
  return request({
    method: "put",
    url: `/security/activeconnections/logout/${eventId}`,
    data: { eventId },
  });
}

export function muteRoomNotification(id, isMute) {
  const options = {
    method: "post",
    url: `/settings/notification/rooms`,
    data: { RoomsId: id, Mute: isMute },
  };

  return request(options);
}

export function setSMTPSettings(data) {
  const options = {
    method: "post",
    url: `/smtpsettings/smtp`,
    data,
  };

  return request(options);
}

export function getSMTPSettings() {
  return request({
    method: "get",
    url: `/smtpsettings/smtp`,
  });
}

export function resetSMTPSettings() {
  return request({
    method: "delete",
    url: `/smtpsettings/smtp`,
  });
}

export function sendingTestMail() {
  return request({
    method: "get",
    url: `/smtpsettings/smtp/test`,
  });
}

export function getSendingTestMailStatus() {
  return request({
    method: "get",
    url: `/smtpsettings/smtp/test/status`,
  });
}
