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
  
  export function getPortalPasswordSettings() {
    return request({
      method: "get",
      url: "/settings/security/password"
    });
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