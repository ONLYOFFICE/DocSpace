import api from "../api";
import { makeAutoObservable } from "mobx";

class ModuleStore {
  isLoading = false;
  isLoaded = false;
  modules = [];

  constructor() {
    makeAutoObservable(this);
  }

  getModules = async () => {
    const list = await api.modules.getModulesList();

    const extendedModules = [
      ...list,
      {
        id: "2A923037-8B2D-487b-9A22-5AC0918ACF3F",
        title: "Mail",
        link: "/products/mail/",
        originUrl: "/addons/mail/",
        helpUrl: "https://helpcenter.onlyoffice.com/userguides/mail.aspx",
        description:
          "Mail is a tool that allows to work with email messages right on your portal. It provides a variety of the standard capabilities implemented in any other email client.",
        isPrimary: false,
      },
      {
        id: "32D24CB5-7ECE-4606-9C94-19216BA42086",
        title: "Calendar",
        link: "/products/calendar/",
        originUrl: "/addons/calendar/",
        helpUrl: "https://helpcenter.onlyoffice.com/userguides/calendar.aspx",
        description:
          "Calendar is a built-in scheduling tool that allows you to always keep track of important events and meetings",
        isPrimary: false,
      },
      {
        id: "BF88953E-3C43-4850-A3FB-B1E43AD53A3E",
        title: "Talk",
        link: "/products/talk/",
        originUrl: "/addons/talk/",
        helpUrl: "https://helpcenter.onlyoffice.com/userguides/talk.aspx",
        description:
          "Talk is an instant messenger that provides a real-time communication between the co-workers. It offers all the traditional features you expect from a messenger: history archiving, file transfer, multi-user chat support, search function, emoticons.",
        isPrimary: false,
      },
    ].map((m) => this.toModuleWrapper(m));

    this.setModules(extendedModules);
  };

  toModuleWrapper = (
    item,
    noAction = true,
    iconName = null,
    iconUrl = null
  ) => {
    const id =
      item.id && typeof item.id === "string" ? item.id.toLowerCase() : null;

    const result = {
      id,
      appName: "none",
      title: item.title,
      link: item.link,
      originUrl: item.originUrl,
      helpUrl: item.helpUrl,
      notifications: 0,
      iconName: item.iconName || iconName || "/static/images/people.react.svg", //TODO: Change to URL
      iconUrl: item.iconUrl || iconUrl,
      imageUrl: item.imageUrl,
      isolateMode: item.isolateMode,
      isPrimary: item.isPrimary,
    };

    switch (id) {
      case "6743007c-6f95-4d20-8c88-a8601ce5e76d":
        result.appName = "crm";
        result.ready = false;
        result.iconName = "CrmIcon";
        result.iconUrl = "/static/images/crm.react.svg";
        result.imageUrl = "/images/crm.svg";
        result.helpUrl =
          "https://helpcenter.onlyoffice.com/userguides/crm.aspx";
        break;
      case "1e044602-43b5-4d79-82f3-fd6208a11960":
        result.appName = "projects";
        result.ready = false;
        result.iconName = "ProjectsIcon";
        result.iconUrl = "/static/images/projects.react.svg";
        result.imageUrl = "/images/projects.svg";
        result.helpUrl =
          "https://helpcenter.onlyoffice.com/userguides/projects.aspx";
        break;
      case "2a923037-8b2d-487b-9a22-5ac0918acf3f":
        result.appName = "mail";
        result.ready = false;
        result.iconName = "MailIcon";
        result.iconUrl = "/static/images/mail.react.svg";
        result.imageUrl = "/images/mail.svg";
        break;
      case "32d24cb5-7ece-4606-9c94-19216ba42086":
        result.appName = "calendar";
        result.ready = false;
        result.iconName = "CalendarCheckedIcon";
        result.iconUrl = "/static/images/calendar.checked.react.svg";
        result.imageUrl = "/images/calendar.svg";
        break;
      case "bf88953e-3c43-4850-a3fb-b1e43ad53a3e":
        result.appName = "chat";
        result.ready = false;
        result.iconName = "ChatIcon";
        result.iconUrl = "/static/images/chat.react.svg";
        result.imageUrl = "/images/talk.svg";
        result.isolateMode = true;
        break;
      case "e67be73d-f9ae-4ce1-8fec-1880cb518cb4":
        result.appName = "files";
        result.ready = true;
        break;
      case "f4d98afd-d336-4332-8778-3c6945c81ea0":
        result.appName = "people";
        result.ready = true;
        break;
      default:
        result.appName = "none";
        result.ready = false;
        break;
    }

    if (!noAction) {
      result.onClick = (e) => {
        if (e) {
          window.open(item.link, "_self");
          e.preventDefault();
        }
      };
      result.onBadgeClick = (e) => console.log(iconName + " Badge Clicked", e);
    } else {
      result.description = item.description;
    }

    return result;
  };

  get totalNotificationsCount() {
    let totalNotifications = 0;
    this.modules
      .filter((item) => !item.separator)
      .forEach((item) => (totalNotifications += item.notifications || 0));

    return totalNotifications;
  }

  init = async () => {
    this.setIsLoading(true);

    await this.getModules();

    this.setIsLoading(false);
    this.setIsLoaded(true);
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  setModules = (modules) => {
    this.modules = modules;
  };
}

export default ModuleStore;
