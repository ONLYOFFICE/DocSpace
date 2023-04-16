import { runInAction, makeAutoObservable } from "mobx";
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

  getWebhookHistory = async (configId, eventId, logCount, startIndex) => {
    return await getWebhooksJournal(configId, eventId, logCount, startIndex);
  };

  get isWebhooksEmpty() {
    return this.webhooks.length === 0;
  }
}

export default WebhooksStore;
