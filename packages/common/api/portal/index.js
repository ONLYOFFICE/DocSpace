import { request } from "../client";

export function getShortenedLink(link) {
  return request({
    method: "put",
    url: "/portal/getshortenlink.json",
    data: { link },
  });
}

const GUEST_INVITE_LINK = "guestInvitationLink";
const USER_INVITE_LINK = "userInvitationLink";
const INVITE_LINK_TTL = "localStorageLinkTtl";
const LINKS_TTL = 6 * 3600 * 1000;

export function getInvitationLink(type) {
  const curLinksTtl = localStorage.getItem(INVITE_LINK_TTL);
  const now = +new Date();

  if (!curLinksTtl) {
    localStorage.setItem(INVITE_LINK_TTL, now);
  } else if (now - curLinksTtl > LINKS_TTL) {
    localStorage.removeItem(GUEST_INVITE_LINK);
    localStorage.removeItem(USER_INVITE_LINK);
    localStorage.setItem(INVITE_LINK_TTL, now);
  }

  const link = localStorage.getItem(
    type === 2 ? GUEST_INVITE_LINK : USER_INVITE_LINK
  );

  return link && type !== 3
    ? Promise.resolve(link)
    : request({
        method: "get",
        url: `/portal/users/invite/${type}.json`,
      }).then((link) => {
        if (type !== 3) {
          localStorage.setItem(
            type === 2 ? GUEST_INVITE_LINK : USER_INVITE_LINK,
            link
          );
        }
        return Promise.resolve(link);
      });
}

export function getInvitationLinks() {
  return Promise.all([
    getInvitationLink(1),
    getInvitationLink(2),
    getInvitationLink(3),
  ]).then(
    ([
      userInvitationLinkResp,
      guestInvitationLinkResp,
      adminInvitationLinkResp,
    ]) => {
      return Promise.resolve({
        userLink: userInvitationLinkResp,
        guestLink: guestInvitationLinkResp,
        adminLink: adminInvitationLinkResp,
      });
    }
  );
}

export function startBackup(storageType, storageParams, backupMail = false) {
  const options = {
    method: "post",
    url: `/portal/startbackup`,
    data: {
      storageType,
      storageParams: storageParams,
      backupMail,
    },
  };

  return request(options);
}

export function getBackupProgress() {
  const options = {
    method: "get",
    url: "/portal/getbackupprogress",
  };
  return request(options);
}

export function deleteBackupSchedule() {
  const options = {
    method: "delete",
    url: "/portal/deletebackupschedule",
  };
  return request(options);
}

export function getBackupSchedule() {
  const options = {
    method: "get",
    url: "/portal/getbackupschedule",
  };
  return request(options);
}

export function createBackupSchedule(
  storageType,
  storageParams,
  backupsStored,
  Period,
  Hour,
  Day = null,
  backupMail = false
) {
  const cronParams = {
    Period: Period,
    Hour: Hour,
    Day: Day,
  };
  const options = {
    method: "post",
    url: "/portal/createbackupschedule",
    data: {
      storageType,
      storageParams,
      backupsStored,
      cronParams: cronParams,
      backupMail,
    },
  };
  return request(options);
}

export function deleteBackupHistory() {
  return request({ method: "delete", url: "/portal/deletebackuphistory" });
}

export function deleteBackup(id) {
  return request({ method: "delete", url: `/portal/deletebackup/${id}` });
}

export function getBackupHistory() {
  return request({ method: "get", url: "/portal/getbackuphistory" });
}

export function startRestore(backupId, storageType, storageParams, notify) {
  return request({
    method: "post",
    url: `/portal/startrestore`,
    data: {
      backupId,
      storageType,
      storageParams: storageParams,
      notify,
    },
  });
}

export function getRestoreProgress() {
  return request({ method: "get", url: "/portal/getrestoreprogress" });
}

export function enableRestore() {
  return request({ method: "get", url: "/portal/enablerestore" });
}

export function enableAutoBackup() {
  return request({ method: "get", url: "/portal/enableAutoBackup" });
}

export function setPortalRename(alias) {
  return request({
    method: "put",
    url: "/portal/portalrename.json",
    data: { alias },
  });
}

export function sendSuspendPortalEmail() {
  return request({
    method: "post",
    url: "/portal/suspend.json",
  });
}

export function sendDeletePortalEmail() {
  return request({
    method: "post",
    url: "/portal/delete.json",
  });
}

export function suspendPortal(confirmKey = null) {
  const options = {
    method: "put",
    url: "/portal/suspend.json",
  };

  if (confirmKey) options.headers = { confirm: confirmKey };

  return request(options);
}

export function continuePortal(confirmKey = null) {
  const options = {
    method: "put",
    url: "/portal/continue.json",
  };

  if (confirmKey) options.headers = { confirm: confirmKey };

  return request(options);
}

export function deletePortal(confirmKey = null) {
  const options = {
    method: "delete",
    url: "/portal/delete.json",
  };

  if (confirmKey) options.headers = { confirm: confirmKey };

  return request(options);
}

export function getPortalPaymentQuotas() {
  return request({ method: "get", url: "/portal/payment/quotas" });
}

export function getPortalQuota() {
  return request({ method: "get", url: "/portal/payment/quota" });
}

export function getPortalTariff() {
  return request({ method: "get", url: "/portal/tariff" });
}

export function getPaymentAccount() {
  return request({ method: "get", url: "/portal/payment/account" });
}

export function getPaymentLink(adminCount, cancelToken, backUrl) {
  return request({
    method: "put",
    url: `/portal/payment/url`,
    data: {
      quantity: { admin: adminCount },
      backUrl,
    },
    cancelToken,
  });
}

export function updatePayment(adminCount) {
  return request({
    method: "put",
    url: `/portal/payment/update`,
    data: {
      quantity: { admin: adminCount },
    },
  });
}

export function getCurrencies() {
  return request({ method: "get", url: "/portal/payment/currencies" });
}

export function getPaymentTariff() {
  return request({ method: "get", url: "/portal/payment/tariff" });
}

export function sendPaymentRequest(email, userName, message) {
  return request({
    method: "post",
    url: `/portal/payment/request `,
    data: {
      email,
      userName,
      message,
    },
  });
}
