import authStore from "@appserver/common/store/AuthStore";
import { toCommunityHostname } from "@appserver/common/utils";
import history from "@appserver/common/history";

export const setDocumentTitle = (subTitle = null) => {
  const { isAuthenticated, settingsStore, product: currentModule } = authStore;
  const { organizationName } = settingsStore;

  let title;
  if (subTitle) {
    if (isAuthenticated && currentModule) {
      title = subTitle + " - " + currentModule.title;
    } else {
      title = subTitle + " - " + organizationName;
    }
  } else if (currentModule && organizationName) {
    title = currentModule.title + " - " + organizationName;
  } else {
    title = organizationName;
  }

  document.title = title;
};

export const checkIfModuleOld = (link) => {
  if (
    !link ||
    link.includes("files") ||
    link.includes("people") ||
    link.includes("settings")
  ) {
    return false;
  } else {
    return true;
  }
};

export const getLink = (link) => {
  if (!link) return;

  if (!checkIfModuleOld(link)) {
    return link;
  }

  if (link.includes("mail") || link.includes("calendar")) {
    link = link.replace("products", "addons");
  } else {
    link = link.replace("products", "Products");
    link = link.replace("crm", "CRM");
    link = link.replace("projects", "Projects");
  }

  const { protocol, hostname } = window.location;

  const communityHostname = toCommunityHostname(hostname);

  return `${protocol}//${communityHostname}${link}?desktop_view=true`;
};

export const onItemClick = (e) => {
  if (!e) return;
  e.preventDefault();

  const link = e.currentTarget.dataset.link;

  if (checkIfModuleOld(link)) {
    return window.open(link, "_blank");
  }

  history.push(link);
};

export const getPasswordErrorMessage = (t, settings) => {
  return `${t("Common:PasswordMinimumLength")} ${
    settings ? settings.minLength : 8
  } ${settings.digits ? t("Common:PasswordLimitDigits") : ""} ${
    settings.upperCase ? t("Common:PasswordLimitUpperCase") : ""
  } ${settings.specSymbols ? t("Common:PasswordLimitSpecialSymbols") : ""}`;
};
