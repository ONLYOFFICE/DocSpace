import firebase from "firebase/app";
import "firebase/remote-config";
import firebaseConfig from "../../firebase.json";
//import "firebase/database";

// Initialize Firebase
// firebase.initializeApp(firebaseConfig);

// const remoteConfig = firebase.remoteConfig();
// remoteConfig.settings = {
//   fetchTimeMillis: 60000,
//   minimumFetchIntervalMillis: 1,
// };

// remoteConfig.defaultConfig = {
//   maintenance: null,
// };

// remoteConfig
//   .ensureInitialized()
//   .then(() => {
//     console.log("Firebase Remote Config is initialized");
//   })
//   .catch((err) => {
//     console.error("Firebase Remote Config failed to initialize", err);
//   });

// export const checkMaintenance = async () => {
//   const res = await remoteConfig.fetchAndActivate();
//   //console.log("fetchAndActivate", res);
//   const maintenance = remoteConfig.getValue("maintenance");
//   if (!maintenance) {
//     return Promise.resolve(null);
//   }
//   return await Promise.resolve(JSON.parse(maintenance.asString()));
// };

class FirebaseHelper {
  remoteConfig = null;
  constructor() {
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
    return firebaseConfig;
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

    const campaigns = this.remoteConfig.getValue("campaigns");
    if (!campaigns) {
      return Promise.resolve(null);
    }
    return await Promise.resolve(JSON.parse(campaigns.asString()));
  }
}

export default new FirebaseHelper();
