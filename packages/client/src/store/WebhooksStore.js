import { makeAutoObservable } from "mobx";
import {
  createWebhook,
  getAllWebhooks,
  updateWebhook,
  removeWebhook,
  getJournal,
  retryWebhook,
  retryWebhooks,
} from "@docspace/common/api/settings";

class WebhooksStore {
  webhooks = [
    {
      id: 0,
      title: "Webhook 1",
      responseCode: "200",
      responseStatus: "success",
      url: "https://webhook.site/3d9f41d8-30dc-4f55-8b78-1649f4118c56",
      isEnabled: true,
    },
    {
      id: 1,
      title: "Webhook 2",
      responseCode: "404",
      responseStatus: "error",
      url: "https://webhook.site/3d9f41d8-30dc-4f55-8b78-16",
      isEnabled: false,
    },
  ];

  constructor() {
    makeAutoObservable(this);
  }

  loadWebhookhs = async () => {
    const webhooks = await getAllWebhooks();
    this.webhooks = webhooks;
  };

  addWebhook = async (webhook) => {
    // const webhookData = await createWebhook(webhook.title, webhook.url, webhook.secretKey);
    // this.webhooks.push({
    //   id: webhookData.Id,
    //   url: webhookData.Uri,
    //   title: "default",
    //   isEnabled: webhookData.Enabled,
    // });
    this.webhooks.push(webhook);
  };

  isWebhookExist = (desiredWebhook) => {
    return this.webhooks.some((webhook) => webhook.id === desiredWebhook.id);
  };

  toggleEnabled = (desiredWebhook) => {
    const index = this.webhooks.findIndex((webhook) => webhook.id === desiredWebhook.id);
    this.webhooks[index].isEnabled = !this.webhooks[index].isEnabled;
  };

  deleteWebhook = async (webhook) => {
    // await removeWebhook(webhook.id);
    this.webhooks = this.webhooks.filter((currentWebhook) => currentWebhook.id !== webhook.id);
  };

  editWebhook = async (prevWebhook, webhookInfo) => {
    // await updateWebhook(webhookInfo.title, webhookInfo.url, webhookInfo.secretKey);
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

  getWebhookHistory = async (hookname, logCount) => {
    return await getJournal(hookname, logCount);
  };

  get isWebhooksEmpty() {
    return this.webhooks.length === 0;
  }
}

export default WebhooksStore;
