import api from "@docspace/common/api";
import { makeAutoObservable } from "mobx";
const { Filter } = api;
import SelectionStore from "./SelectionStore";
//import CommonStore from "./CommonStore";
import authStore from "@docspace/common/store/AuthStore";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import { isMobile } from "react-device-detect";

class SettingsSetupStore {
  selectionStore = null;
  authStore = null;
  isInit = false;
  viewAs = isMobile ? "row" : "table";

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
    loginHistory: {
      users: [],
    },
    auditTrail: {
      users: [],
    },
    trailReport: [],
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

  securityLifetime = [];

  constructor() {
    this.selectionStore = new SelectionStore(this);
    this.authStore = authStore;
    makeAutoObservable(this);
  }

  initSettings = async () => {
    if (this.isInit) return;
    this.isInit = true;

    if (authStore.isAuthenticated) {
      await authStore.settingsStore.getPortalPasswordSettings();
      await authStore.tfaStore.getTfaType();
      await authStore.settingsStore.getIpRestrictionsEnable();
      await authStore.settingsStore.getIpRestrictions();
      await authStore.settingsStore.getSessionLifetime();
    }
  };

  setIsInit = (isInit) => {
    this.isInit = isInit;
  };

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

  setViewAs = (viewAs) => {
    this.viewAs = viewAs;
  };

  setOwner = (owner) => {
    this.security.accessRight.owner = owner;
  };

  setFilter = (filter) => {
    this.security.accessRight.filter = filter;
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
        `${config.homepage}/portal-settings/security/access-rights/admins`,
        `/filter?page=${filter.page}` //TODO: Change url by category
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

  setDNSSettings = async (dnsName, enable) => {
    const res = await api.settings.setMailDomainSettings(dnsName, enable);
  };

  getLifetimeAuditSettings = async (data) => {
    const res = await api.settings.getLifetimeAuditSettings(data);
    this.setSecurityLifeTime(res);
  };

  setLifetimeAuditSettings = async (data) => {
    await api.settings.setLifetimeAuditSettings(data);
  };

  setSecurityLifeTime = (lifetime) => {
    this.securityLifetime = lifetime;
  };

  setLoginHistoryUsers = (users) => {
    this.security.loginHistory.users = users;
  };

  setAuditTrailUsers = (users) => {
    this.security.auditTrail.users = users;
  };

  getLoginHistory = async () => {
    const res = await api.settings.getLoginHistory();
    return this.setLoginHistoryUsers(res);
  };

  getAuditTrail = async () => {
    const res = await api.settings.getAuditTrail();
    return this.setAuditTrailUsers(res);
  };

  getLoginHistoryReport = async () => {
    const res = await api.settings.getLoginHistoryReport();
    window.open(res);
    return this.setAuditTrailReport(res);
  };

  getAuditTrailReport = async () => {
    const res = await api.settings.getAuditTrailReport();
    window.open(res);
    return this.setAuditTrailReport(res);
  };

  setGreetingTitle = async (greetingTitle) => {
    const res = await api.settings.setGreetingSettings(greetingTitle);

    //if (res) this.setGreetingSettings(greetingTitle);
  };

  setAuditTrailReport = (report) => {
    this.security.trailReport = report;
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
