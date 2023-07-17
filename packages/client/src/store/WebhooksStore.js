import {
  createWebhook,
  getAllWebhooks,
  getWebhooksJournal,
  removeWebhook,
  retryWebhook,
  retryWebhooks,
  toggleEnabledWebhook,
  updateWebhook,
} from "@docspace/common/api/settings";
import { makeAutoObservable, runInAction } from "mobx";

class WebhooksStore {
  webhooks = [];
  checkedEventIds = [];
  historyFilters = null;
  historyItems = [];
  startIndex = 0;
  totalItems = 0;
  currentWebhook = {};
  eventDetails = {};
  FETCH_COUNT = 100;
  isRetryPending = false;

  constructor() {
    makeAutoObservable(this);
  }

  setRetryPendingFalse = () => {
    this.isRetryPending = false;
  };

  setRetryPendingTrue = () => {
    this.isRetryPending = true;
  };

  setCurrentWebhook = (webhook) => {
    this.currentWebhook = webhook;
  };

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
      });
    } catch (error) {
      console.error(error);
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

  fetchHistoryItems = async (params) => {
    this.totalItems = 0;
    this.startIndex = 0;
    const count = params.count ? params.count : this.FETCH_COUNT;
    const historyData = await getWebhooksJournal({
      ...params,
      startIndex: this.startIndex,
      count: count,
    });
    runInAction(() => {
      this.startIndex = count;
      this.historyItems = historyData.items;
      this.totalItems = historyData.total;
    });
  };
  fetchMoreItems = async (params) => {
    const count = params.count ? params.count : this.FETCH_COUNT;
    const historyData = await getWebhooksJournal({
      ...params,
      startIndex: this.startIndex,
      count: count,
    });
    runInAction(() => {
      this.startIndex = this.startIndex + count;
      this.historyItems = [...this.historyItems, ...historyData.items];
    });
  };
  fetchEventData = async (eventId) => {
    const data = await getWebhooksJournal({ eventId });
    this.eventDetails = data.items[0];
  };
  get hasMoreItems() {
    return this.totalItems > this.startIndex;
  }

  get isWebhooksEmpty() {
    return this.webhooks.length === 0;
  }

  setHistoryFilters = (filters) => {
    this.historyFilters = filters;
  };
  clearHistoryFilters = () => {
    this.historyFilters = null;
  };
  clearDate = () => {
    this.historyFilters = { ...this.historyFilters, deliveryDate: null };
  };
  unselectStatus = (statusCode) => {
    this.historyFilters = {
      ...this.historyFilters,
      status: this.historyFilters.status.filter((item) => item !== statusCode),
    };
  };

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

  toggleEventId = (id) => {
    this.checkedEventIds = this.checkedEventIds.includes(id)
      ? this.checkedEventIds.filter((checkedId) => checkedId !== id)
      : [...this.checkedEventIds, id];
  };
  isIdChecked = (id) => {
    return this.checkedEventIds.includes(id);
  };
  checkAllIds = () => {
    this.checkedEventIds = this.historyItems.map((event) => event.id);
  };
  emptyCheckedIds = () => {
    this.checkedEventIds = [];
  };
  get areAllIdsChecked() {
    return this.checkedEventIds.length === this.historyItems.length;
  }
  get isIndeterminate() {
    return this.checkedEventIds.length > 0 && !this.areAllIdsChecked;
  }

  get isGroupMenuVisible() {
    return this.checkedEventIds.length !== 0;
  }
}

export default WebhooksStore;
