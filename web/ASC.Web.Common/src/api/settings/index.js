import { request } from "../client";

export function getSettings() {
    return request({
      method: "get",
      url: "/settings.json"
    });
  }

  export function getPortalCultures() {
    return request({
      method: "get",
      url: "/settings/cultures.json"
    });
  }
  
  export function getPortalPasswordSettings(confirmKey = null) {
    const options = {
      method: "get",
      url: "/settings/security/password"
    };

    if(confirmKey)
      options.headers = { confirm: confirmKey };

    return request(options);
  }

  export function getPortalTimezones() {
    return request({
      method: "get",
      url: "/settings/timezones.json"
    });
  }

  export function setLanguageAndTime(lng, timeZoneID) {
    return request({
      method: "put",
      url: "/settings/timeandlanguage.json",
      data: { lng, timeZoneID }
    });
  }

  export function setGreetingSettings(title) {
    return request({
      method: "post",
      url: `/settings/greetingsettings.json`,
      data: { title }
    });
  }
  
  export function restoreGreetingSettings() {
    return request({
      method: "post",
      url: `/settings/greetingsettings/restore.json`
    });
  }
  
  export function getLogoText() {
    return request({
      method: "get",
      url: `/settings/whitelabel/logotext.json`
    });
  }
  export function getLogoSizes() {
    return request({
      method: "get",
      url: `/settings/whitelabel/sizes.json`
    });
  }
  
  export function getLogoUrls() {
    return request({
      method: "get",
      url: `/settings/whitelabel/logos.json`
    });
  }

  export function getCurrentCustomSchema(id) {
    return request({
      method: "get",
      url: `settings/customschemas/${id}.json`
    });
  }

  export function sendRecoverRequest(email, message) {
    const data = { email, message };
    return request({
      method: "post",
      url: `/settings/sendadmmail`,
      data
    });
  }