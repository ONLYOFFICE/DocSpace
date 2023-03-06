import { makeAutoObservable } from "mobx";

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

  addWebhook(webhook) {
    this.webhooks.push(webhook);
  }

  isWebhookExist(desiredWebhook) {
    return this.webhooks.find((webhook) => webhook.id === desiredWebhook.id) !== undefined;
  }

  get isWebhooksEmpty() {
    return this.webhooks.length === 0;
  }
}

export default WebhooksStore;
