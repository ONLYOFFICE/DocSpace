export const DBConfig = {
  name: "AdsDb",
  version: 1,
  objectStoresMeta: [
    {
      store: "ads",
      storeConfig: { keyPath: "id", autoIncrement: true },
      storeSchema: [
        { name: "campaign", keypath: "campaign", options: { unique: true } },
        { name: "image", keypath: "image", options: { unique: false } },
        { name: "translate", keypath: "translate", options: { unique: false } },
      ],
    },
  ],
};
