import api from "@docspace/common/api";
import { makeAutoObservable } from "mobx";
const { Filter } = api;
import SelectionStore from "./SelectionStore";
//import CommonStore from "./CommonStore";
import authStore from "@docspace/common/store/AuthStore";
import {
  getSMTPSettings,
  resetSMTPSettings,
  setSMTPSettings,
} from "@docspace/common/api/settings";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import { isMobile } from "react-device-detect";

class SettingsSetupStore {
  selectionStore = null;
  authStore = null;
  isInit = false;
  logoutVisible = false;
  logoutAllVisible = false;
  viewAs = isMobile ? "row" : "table";

  isLoadingDownloadReport = false;

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
    smtpSettings: {
      initialSettings: {},
      settings: {
        credentialsUserName: "",
        credentialsUserPassword: "",
        enableAuth: false,
        enableSSL: false,
        useNtlm: false,
        host: "",
        port: "25",
        senderAddress: "",
        senderDisplayName: "",
      },
      isLoading: false,
      isDefaultSettings: false,
      errors: {},
    },
  };

  dataManagement = {
    commonThirdPartyList: [],
  };

  securityLifetime = [];

  sessionsIsInit = false;
  sessions = [];
  currentSession = [];

  constructor() {
    this.selectionStore = new SelectionStore(this);
    this.authStore = authStore;
    makeAutoObservable(this);
  }

  initSettings = async () => {
    if (this.isInit) return;

    if (authStore.isAuthenticated) {
      await authStore.settingsStore.getPortalPasswordSettings();
      await authStore.tfaStore.getTfaType();
      await authStore.settingsStore.getIpRestrictionsEnable();
      await authStore.settingsStore.getIpRestrictions();
      await authStore.settingsStore.getSessionLifetime();
      await authStore.settingsStore.getBruteForceProtection();
    }

    this.isInit = true;
  };

  setIsLoadingDownloadReport = (state) => {
    this.isLoadingDownloadReport = state;
  };

  resetIsInit = () => {
    this.isInit = false;
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

  get isSMTPInitialSettings() {
    const settings = this.integration.smtpSettings.settings;
    const initialSettings = this.integration.smtpSettings.initialSettings;

    const fields = Object.keys(settings).filter(
      (key) => settings[key] !== initialSettings[key]
    );

    return fields.length === 0;
  }

  setSMTPFields = (result) => {
    const { isDefaultSettings, ...settings } = result;

    const storeSettings = this.integration.smtpSettings.settings;

    this.integration.smtpSettings.isDefaultSettings = isDefaultSettings;

    for (var key in settings) {
      if (settings[key] === null) continue;
      storeSettings[key] = settings[key];
    }

    this.integration.smtpSettings.errors = {};
    this.integration.smtpSettings.initialSettings = { ...storeSettings };
  };
  setInitSMTPSettings = async () => {
    const result = await getSMTPSettings();

    if (!result) return;

    this.setSMTPFields(result);
  };

  resetSMTPSettings = async () => {
    const result = await resetSMTPSettings(
      this.integration.smtpSettings.settings
    );

    if (!result) return;

    this.setSMTPFields(result);
  };

  updateSMTPSettings = async () => {
    await setSMTPSettings(this.integration.smtpSettings.settings);

    this.setInitSMTPSettings();
  };

  setSMTPSettings = (settings) => {
    this.integration.smtpSettings.settings = settings;
  };

  setSMTPSettingsLoading = (loading) => {
    this.integration.smtpSettings.isLoading = loading;
  };

  setSMTPErrors = (errorsArray) => {
    let errors = {};
    errorsArray.forEach((elem) => {
      errors = {
        ...errors,
        [elem.name]: elem.hasError,
      };
    });

    this.integration.smtpSettings.errors = { ...errors };
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
        window.DocSpaceConfig?.proxy?.url,
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

  setLanguageAndTime = async (lng, timeZoneID) => {
    return api.settings.setLanguageAndTime(lng, timeZoneID);
  };

  setPortalRename = async (alias) => {
    return api.portal.setPortalRename(alias);
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
    this.getLifetimeAuditSettings();
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
    try {
      this.setIsLoadingDownloadReport(true);
      const res = await api.settings.getAuditTrailReport();
      window.open(res);
      return this.setAuditTrailReport(res);
    } catch (error) {
      console.error(error);
    } finally {
      this.setIsLoadingDownloadReport(false);
    }
  };

  setGreetingTitle = async (greetingTitle) => {
    return api.settings.setGreetingSettings(greetingTitle);
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
    return api.settings.restoreGreetingSettings();
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

  dataReassignment = (fromUserId, toUserId, deleteProfile) => {
    return api.settings.dataReassignment(fromUserId, toUserId, deleteProfile);
  };

  dataReassignmentProgress = (id) => {
    return api.settings.dataReassignmentProgress(id);
  };

  dataReassignmentTerminate = (userId) => {
    return api.settings.dataReassignmentTerminate(userId);
  };

  getCommonThirdPartyList = async () => {
    const res = await api.settings.getCommonThirdPartyList();

    this.setCommonThirdPartyList(res);
  };

  getAllSessions = () => {
    return api.settings.getAllActiveSessions();
  };

  removeAllSessions = () => {
    return api.settings.removeAllActiveSessions();
  };

  removeAllExecptThis = () => {
    return api.settings.removeAllExceptThisSession();
  };

  removeSession = (id) => {
    return api.settings.removeActiveSession(id);
  };

  getAllSettings = () => {
    return api.settings.getSettings();
  };

  setLogoutVisible = (visible) => (this.logoutVisible = visible);

  setLogoutAllVisible = (visible) => (this.logoutAllVisible = visible);

  getSessions = () => {
    if (this.sessionsIsInit) return;
    this.getAllSessions().then((res) => {
      this.sessions = res.items;
      this.currentSession = res.loginEvent;
      this.sessionsIsInit = true;
    });
  };
}

export default SettingsSetupStore;
