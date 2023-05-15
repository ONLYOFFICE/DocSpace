import { runInAction, makeAutoObservable } from "mobx";
import moment from "moment";
import {
  createWebhook,
  getAllWebhooks,
  updateWebhook,
  removeWebhook,
  toggleEnabledWebhook,
  getWebhooksJournal,
  retryWebhook,
  retryWebhooks,
} from "@docspace/common/api/settings";

class WebhooksStore {
  webhooks = [];
  state = "pending"; // "pending", "done" or "error"
  filterSettings = {
    deliveryDate: null,
    deliveryFrom: moment().startOf("day"),
    deliveryTo: moment().endOf("day"),
    status: [],
  };
  checkedEventIds = [];
  isTitleVisible = true;
  startIndex = 0;

  constructor() {
    makeAutoObservable(this);
  }

  loadWebhooks = async () => {
    try {
      const webhooksData = await getAllWebhooks();
      runInAction(() => {
        this.webhooks = webhooksData.map((data) => ({
          id: data.configs.id,
          name: data.configs.name,
          uri: data.configs.uri,
          secretKey: data.configs.secretKey,
          enabled: data.configs.enabled,
          ssl: data.configs.ssl,
          status: data.status,
        }));
        this.state = "success";
      });
    } catch (error) {
      console.error(error);
      this.state = "error";
    }
  };

  addWebhook = async (webhook) => {
    const webhookData = await createWebhook(
      webhook.name,
      webhook.uri,
      webhook.secretKey,
      webhook.ssl,
    );

    this.webhooks = [
      ...this.webhooks,
      {
        id: webhookData.id,
        uri: webhookData.uri,
        name: webhookData.name,
        enabled: webhookData.enabled,
        secretKey: webhookData.secretKey,
        ssl: webhookData.ssl,
      },
    ];
  };

  isWebhookExist = (desiredWebhook) => {
    return this.webhooks.some((webhook) => webhook.id === desiredWebhook.id);
  };

  toggleEnabled = async (desiredWebhook) => {
    await toggleEnabledWebhook(desiredWebhook);
    const index = this.webhooks.findIndex((webhook) => webhook.id === desiredWebhook.id);
    this.webhooks[index].enabled = !this.webhooks[index].enabled;
  };

  deleteWebhook = async (webhook) => {
    await removeWebhook(webhook.id);
    this.webhooks = this.webhooks.filter((currentWebhook) => currentWebhook.id !== webhook.id);
  };

  editWebhook = async (prevWebhook, webhookInfo) => {
    await updateWebhook(
      prevWebhook.id,
      webhookInfo.name,
      webhookInfo.uri,
      webhookInfo.secretKey || prevWebhook.secretKey,
      webhookInfo.ssl,
    );
    this.webhooks = this.webhooks.map((webhook) =>
      webhook.id === prevWebhook.id ? { ...prevWebhook, ...webhookInfo } : webhook,
    );
  };

  retryWebhookEvent = async (id) => {
    return await retryWebhook(id);
  };

  retryWebhookEvents = async (ids) => {
    return await retryWebhooks(ids);
  };

  getWebhookHistory = async (params) => {
    return await getWebhooksJournal(params);
  };
  getWebhookHistoryBatch = async (params) => {
    const historyWebhooks = await getWebhooksJournal({ ...params, logCount: 10, startIndex });
    this.startIndex = this.startIndex + 10;
    return historyWebhooks;
  };

  get isWebhooksEmpty() {
    return this.webhooks.length === 0;
  }

  formatFilters = (filters) => {
    const params = {};
    if (filters.deliveryDate !== null) {
      params.deliveryFrom =
        filters.deliveryDate.format("YYYY-MM-DD") + "T" + filters.deliveryFrom.format("HH:mm:ss");

      params.deliveryTo =
        filters.deliveryDate.format("YYYY-MM-DD") + "T" + filters.deliveryTo.format("HH:mm:ss");
    }

    const statusEnum = {
      "Not sent": 1,
      "2XX": 2,
      "3XX": 4,
      "4XX": 8,
      "5XX": 16,
    };

    if (filters.status.length > 0) {
      const statusFlag = filters.status.reduce(
        (sum, currentValue) => sum + statusEnum[currentValue],
        0,
      );
      params.groupStatus = statusFlag;
    }

    return params;
  };

  setDeliveryDate = (date) => {
    this.filterSettings = { ...this.filterSettings, deliveryDate: date };
  };
  setDeliveryFrom = (date) => {
    this.filterSettings = { ...this.filterSettings, deliveryFrom: date };
  };
  setDeliveryTo = (date) => {
    this.filterSettings = { ...this.filterSettings, deliveryTo: date };
  };
  toggleStatus = (statusCode) => {
    this.filterSettings = {
      ...this.filterSettings,
      status: this.filterSettings.status.includes(statusCode)
        ? this.filterSettings.status.filter((statusItem) => statusItem !== statusCode)
        : [...this.filterSettings.status, statusCode],
    };
  };
  isStatusSelected = (statusCode) => {
    return this.filterSettings.status.includes(statusCode);
  };
  clearFilterSettings = () => {
    this.filterSettings = {
      deliveryDate: null,
      deliveryFrom: moment().startOf("day"),
      deliveryTo: moment().endOf("day"),
      status: [],
    };
  };
  clearFilterDate = () => {
    this.filterSettings = {
      deliveryDate: null,
      deliveryFrom: moment().startOf("day"),
      deliveryTo: moment().endOf("day"),
      status: this.filterSettings.status,
    };
  };
  clearFilterStatus = () => {
    this.filterSettings = { ...this.filterSettings, status: [] };
  };

  toggleEventId = (id) => {
    this.checkedEventIds = this.checkedEventIds.includes(id)
      ? this.checkedEventIds.filter((checkedId) => checkedId !== id)
      : [...this.checkedEventIds, id];
  };
  isIdChecked = (id) => {
    return this.checkedEventIds.includes(id);
  };
  checkAllIds = (webhookEvents) => {
    this.checkedEventIds = webhookEvents.map((event) => event.id);
  };
  emptyCheckedIds = () => {
    this.checkedEventIds = [];
  };

  get isCheckedIdsEmpty() {
    return this.checkedEventIds.length === 0;
  }
  
  get isHeaderVisible() {
    return this.checkedEventIds.length !== 0;
  }

  hideTitle = () => {
    this.isTitleVisible = false;
  };
  showTitle = () => {
    this.isTitleVisible = true;
  };
}

export default WebhooksStore;
