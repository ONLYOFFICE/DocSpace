import api from "@appserver/common/api";
import { makeAutoObservable } from "mobx";
const { Filter } = api;
import SelectionStore from "./SelectionStore";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../package.json";

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
      adminsTotal: 0,
      owner: {},
      filter: Filter.getDefault(),
      selectorIsOpen: false,
      isLoading: false,
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

  dataManagement = {
    commonThirdPartyList: [],
  };

  constructor() {
    this.selectionStore = new SelectionStore(this);
    makeAutoObservable(this);
  }

  setIsLoading = (isLoading) => {
    this.security.accessRight.isLoading = isLoading;
  };

  setOptions = (options) => {
    this.security.accessRight.options = options;
  };

  setUsers = (users) => {
    this.security.accessRight.users = users;
  };

  setAdmins = (admins) => {
    this.security.accessRight.admins = admins;
  };

  setTotalAdmins = (total) => {
    this.security.accessRight.adminsTotal = total;
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

  setCommonThirdPartyList = (commonThirdPartyList) => {
    commonThirdPartyList.map((currentValue, index) => {
      commonThirdPartyList[index].key = `0-${index}`;
    });

    this.dataManagement.commonThirdPartyList = commonThirdPartyList;
  };
  setSelectedConsumer = (selectedConsumerName) => {
    this.integration.selectedConsumer =
      this.integration.consumers.find((c) => c.name === selectedConsumerName) ||
      {};
  };

  setFilterUrl = (filter) => {
    window.history.replaceState(
      "",
      "",
      combineUrl(
        AppServerConfig.proxyURL,
        `${config.homepage}/settings/security/access-rights/admins`,
        `/filter?page=${filter.page}`
      )
    );
  };

  setFilterParams = (data) => {
    this.setFilterUrl(data);
    this.setFilter(data);
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

  updateListAdmins = async (filter, withoutOwner) => {
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
    if (filter) {
      this.setFilterParams(filterData);
    }

    this.setAdmins(admins.items);
    this.setTotalAdmins(admins.total - 1);
    this.setFilter(filterData);
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

  setWhiteLabelSettings = async (data) => {
    const response = await api.settings.setWhiteLabelSettings(data);
    return Promise.resolve(response);
  };

  restoreWhiteLabelSettings = async (isDefault) => {
    const res = await api.settings.restoreWhiteLabelSettings(isDefault);
  };

  setLanguageAndTime = async (lng, timeZoneID) => {
    const res = await api.settings.setLanguageAndTime(lng, timeZoneID);
    //console.log("setLanguageAndTime", res);
    //if (res) this.setPortalLanguageAndTime({ lng, timeZoneID });
  };

  setPortalRename = async (alias) => {
    const res = await api.portal.setPortalRename(alias);
  };

  setMailDomainSettings = async (data) => {
    const res = await api.settings.setMailDomainSettings(data);
  };

  setDNSSettings = async (dnsName, enable) => {
    const res = await api.settings.setMailDomainSettings(dnsName, enable);
  };

  setIpRestrictions = async (data) => {
    const res = await api.settings.setIpRestrictions(data);
  };

  setIpRestrictionsEnable = async (data) => {
    const res = await api.settings.setIpRestrictionsEnable(data);
  };

  setMessageSettings = async (turnOn) => {
    const res = await api.settings.setMessageSettings(turnOn);
  };

  setCookieSettings = async (lifeTime) => {
    const res = await api.settings.setCookieSettings(lifeTime);
  };

  setLifetimeAuditSettings = async (data) => {
    const res = await api.settings.setLifetimeAuditSettings(data);
  };

  getAuditTrailReport = async () => {
    const res = await api.settings.getAuditTrailReport();
  };

  setGreetingTitle = async (greetingTitle) => {
    const res = await api.settings.setGreetingSettings(greetingTitle);

    //if (res) this.setGreetingSettings(greetingTitle);
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
    //if (res) this.setGreetingSettings(res.Content);
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

  getCommonThirdPartyList = async () => {
    const res = await api.settings.getCommonThirdPartyList();

    this.setCommonThirdPartyList(res);
  };
}

export default SettingsSetupStore;
