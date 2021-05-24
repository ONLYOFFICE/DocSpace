import api from "@appserver/common/api";
import { makeAutoObservable } from "mobx";
const { Filter } = api;
import SelectionStore from "./SelectionStore";

class SettingsSetupStore {
  selectionStore = null;

  common = {
    whiteLabel: {
      logoSizes: [],
      logoText: null,
      logoUrls: [],
    },
  };
  security = {
    accessRight: {
      options: [],
      users: [],
      admins: [],
      owner: {},
      filter: Filter.getDefault(),
      selectorIsOpen: false,
    },
  };

  headerAction = {
    addUsers: "",
    removeAdmins: "",
  };

  integration = {
    consumers: [],
    selectedConsumer: {},
  };

  constructor() {
    this.selectionStore = new SelectionStore(this);
    makeAutoObservable(this);
  }

  setOptions = (options) => {
    this.security.accessRight.options = options;
  };

  setUsers = (users) => {
    this.security.accessRight.users = users;
  };

  setAdmins = (admins) => {
    this.security.accessRight.admins = admins;
  };

  setOwner = (owner) => {
    this.security.accessRight.owner = owner;
  };

  setFilter = (filter) => {
    this.security.accessRight.filter = filter;
  };

  setLogoText = (text) => {
    this.common.whiteLabel.logoText = text;
  };

  setLogoSizes = (sizes) => {
    this.common.whiteLabel.logoSizes = sizes;
  };

  setLogoUrls = (urls) => {
    this.common.whiteLabel.logoUrls = urls;
  };

  setConsumers = (consumers) => {
    this.integration.consumers = consumers;
  };

  setAddUsers = (func) => {
    this.headerAction.addUsers = func;
  };

  setRemoveAdmins = (func) => {
    this.headerAction.removeAdmins = func;
  };

  toggleSelector = (isOpen) => {
    this.security.accessRight.selectorIsOpen = isOpen;
  };

  setSelectedConsumer = (selectedConsumerName) => {
    this.integration.selectedConsumer =
      this.integration.consumers.find((c) => c.name === selectedConsumerName) ||
      {};
  };

  changeAdmins = async (userIds, productId, isAdmin) => {
    const requests = userIds.map((userId) =>
      api.people.changeProductAdmin(userId, productId, isAdmin)
    );

    await Promise.all(requests);
  };

  getPortalOwner = async (userId) => {
    const owner = await api.people.getUserById(userId);

    this.setOwner(owner);
  };

  getUsersByIds = async (Ids) => {
    const users = Ids.map((id) => {
      return api.people.getUserById(id);
    });

    return Promise.all(users);
  };

  fetchPeople = async (filter) => {
    let filterData = filter && filter.clone();
    if (!filterData) {
      filterData = Filter.getDefault();
    }

    const admins = await api.people.getListAdmins(filterData);

    filterData.total = admins.total;
    this.setAdmins(admins.items);
    this.setFilter(filterData);
  };

  updateListAdmins = (filter, withoutOwner) => {
    return new Promise(async (resolve, reject) => {
      try {
        let filterData = filter && filter.clone();
        if (!filterData) {
          filterData = Filter.getDefault();
        }
        const admins = await api.people.getListAdmins(filterData);

        if (withoutOwner) {
          admins.items = admins.items.filter((admin) => {
            if (admin.isOwner) return false;
            return true;
          });
        }

        filterData.total = admins.total;

        this.setAdmins(admins.items);
        this.setFilter(filterData);

        resolve(admins.items);
      } catch (e) {
        reject(e);
      }
    });
  };

  getWhiteLabelLogoText = async () => {
    const res = await api.settings.getLogoText();
    this.setLogoText(res);
  };

  getWhiteLabelLogoSizes = async () => {
    const res = await api.settings.getLogoSizes();
    this.setLogoSizes(res);
  };

  getWhiteLabelLogoUrls = async () => {
    const res = await api.settings.getLogoUrls();
    this.setLogoUrls(Object.values(res));
  };

  setLanguageAndTime = async (lng, timeZoneID) => {
    const res = await api.settings.setLanguageAndTime(lng, timeZoneID);
    console.log("setLanguageAndTime", res);
    if (res) this.setLanguageAndTime({ lng, timeZoneID });
  };

  setGreetingTitle = async (greetingTitle) => {
    const res = await api.settings.setGreetingSettings(greetingTitle);

    if (res) this.setGreetingSettings(greetingTitle);
  };

  setCurrentSchema = async (id) => {
    return api.settings.setCurrentSchema(id);
  };

  setCustomSchema = async (
    userCaption,
    usersCaption,
    groupCaption,
    groupsCaption,
    userPostCaption,
    regDateCaption,
    groupHeadCaption,
    guestCaption,
    guestsCaption
  ) => {
    return api.settings.setCustomSchema(
      userCaption,
      usersCaption,
      groupCaption,
      groupsCaption,
      userPostCaption,
      regDateCaption,
      groupHeadCaption,
      guestCaption,
      guestsCaption
    );
  };

  restoreGreetingTitle = async () => {
    const res = await api.settings.restoreGreetingSettings();
    if (res) this.setGreetingSettings(res.Content);
  };

  getConsumers = async () => {
    const res = await api.settings.getConsumersList();
    this.setConsumers(res);
  };

  updateConsumerProps = async (newProps) => {
    const res = await api.settings.updateConsumerProps(newProps);
    console.log("updateConsumerProps", res);
    await this.getConsumers();
  };

  changePassword = (userId, hash, key) => {
    return api.people.changePassword(userId, hash, key);
  };

  sendOwnerChange = (id) => {
    return api.settings.sendOwnerChange(id);
  };
}

export default SettingsSetupStore;
