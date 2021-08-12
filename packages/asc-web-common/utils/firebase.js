import firebase from "firebase/app";
import "firebase/remote-config";

class FirebaseHelper {
  remoteConfig = null;
  firebaseConfig = null;
  constructor(settings) {
    this.firebaseConfig = settings;

    if (!this.isEnabled) return;
    firebase.initializeApp(this.config);

    this.remoteConfig = firebase.remoteConfig();

    this.remoteConfig.settings = {
      fetchTimeMillis: 60000,
      minimumFetchIntervalMillis: 1,
    };

    this.remoteConfig.defaultConfig = {
      maintenance: null,
    };

    this.remoteConfig
      .ensureInitialized()
      .then(() => {
        console.log("Firebase Remote Config is initialized");
      })
      .catch((err) => {
        console.error("Firebase Remote Config failed to initialize", err);
      });
  }

  get config() {
    return this.firebaseConfig;
  }

  get isEnabled() {
    return (
      this.config &&
      this.config["apiKey"] &&
      this.config["authDomain"] &&
      this.config["projectId"] &&
      this.config["storageBucket"] &&
      this.config["messagingSenderId"] &&
      this.config["appId"] &&
      this.config["measurementId"]
    );
  }

  async checkMaintenance() {
    if (!this.isEnabled) return Promise.reject("Not enabled");

    const res = await this.remoteConfig.fetchAndActivate();
    //console.log("fetchAndActivate", res);
    const maintenance = this.remoteConfig.getValue("maintenance");
    if (!maintenance) {
      return Promise.resolve(null);
    }
    return await Promise.resolve(JSON.parse(maintenance.asString()));
  }

  async checkCampaigns() {
    if (!this.isEnabled) return Promise.reject("Not enabled");

    const res = await this.remoteConfig.fetchAndActivate();

    const campaignsValue = this.remoteConfig.getValue("campaigns");
    const campaignsString = campaignsValue && campaignsValue.asString();

    if (!campaignsValue || !campaignsString) {
      return Promise.resolve([]);
    }

    const list = JSON.parse(campaignsString);

    if (!list || !(list instanceof Array)) return Promise.resolve([]);

    const campaigns = list.filter((element) => {
      return typeof element === "string" && element.length > 0;
    });

    return await Promise.resolve(campaigns);
  }
}

export default FirebaseHelper;
