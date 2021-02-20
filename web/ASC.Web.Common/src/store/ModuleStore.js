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

  toModuleWrapper = (item, noAction = true, iconName = null) => {
    switch (item.id) {
      case "6743007c-6f95-4d20-8c88-a8601ce5e76d":
        item.iconName = "CrmIcon";
        item.iconUrl = "";
        item.imageUrl = "/images/crm.svg";
        item.helpUrl = "https://helpcenter.onlyoffice.com/userguides/crm.aspx";
        break;
      case "1e044602-43b5-4d79-82f3-fd6208a11960":
        item.iconName = "ProjectsIcon";
        item.iconUrl = "";
        item.imageUrl = "/images/projects.svg";
        item.helpUrl =
          "https://helpcenter.onlyoffice.com/userguides/projects.aspx";
        break;
      case "2A923037-8B2D-487b-9A22-5AC0918ACF3F":
        item.iconName = "MailIcon";
        item.iconUrl = "";
        item.imageUrl = "/images/mail.svg";
        break;
      case "32D24CB5-7ECE-4606-9C94-19216BA42086":
        item.iconName = "CalendarCheckedIcon";
        item.iconUrl = "";
        item.imageUrl = "/images/calendar.svg";
        break;
      case "BF88953E-3C43-4850-A3FB-B1E43AD53A3E":
        item.iconName = "ChatIcon";
        item.iconUrl = "";
        item.imageUrl = "/images/talk.svg";
        item.isolateMode = true;
        break;
      default:
        break;
    }

    const actions = noAction
      ? null
      : {
          onClick: (e) => {
            if (e) {
              window.open(item.link, "_self");
              e.preventDefault();
            }
          },
          onBadgeClick: (e) => console.log(iconName + " Badge Clicked", e),
        };

    const description = noAction ? { description: item.description } : null;

    return {
      id: item.id,
      title: item.title,
      link: item.link,
      originUrl: item.originUrl,
      helpUrl: item.helpUrl,
      iconName: item.iconName || iconName || "PeopleIcon", //TODO: Change to URL
      iconUrl: item.iconUrl,
      imageUrl: item.imageUrl,
      notifications: 0,
      isolateMode: item.isolateMode,
      isPrimary: item.isPrimary,
      ...description,
      ...actions,
    };
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
