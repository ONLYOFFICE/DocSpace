import { makeAutoObservable } from "mobx";
import {
  deletePortal,
  getDomainName,
  setDomainName,
  setPortalName,
  createNewPortal,
  getAllPortals,
} from "@docspace/common/api/management";

class SpacesStore {
  isInit = false;

  portals = [];
  domain: string | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  initStore = async () => {
    if (this.isInit) return;
    this.isInit = true;

    const requests = [];
    requests.push(this.getPortalDomain(), this.getAllPortals());

    return Promise.all(requests);
  };

  deletePortal = async (portalName: string) => {
    const data = {
      portalName,
    };

    console.log(data);
    const res = await deletePortal(data);
    await this.getAllPortals();
  };

  getPortalDomain = async () => {
    const res = await getDomainName();
    const { settings } = res;

    this.domain = settings;
    // const status = await getPortalStatus(settings);
  };

  get isConnected() {
    return !!this.domain;
  }

  setPortalSettings = async (domain: string, portalName: string) => {
    const dmn = await setDomainName(domain);
    const { settings } = dmn;
    this.domain = settings;
    if (!portalName) return;
    const name = await setPortalName(portalName);
    //   console.log(name);
  };

  createNewPortal = async (portalName: string) => {
    //  const { firstName, lastName, email } = authStore.userStore.user;
    //   console.log(firstName, lastName, email);
    //    const data = { firstName, lastName, email, portalName };
    //  const register = await createNewPortal(data);
    // console.log(register);
  };

  getAllPortals = async () => {
    const res = await getAllPortals();
    this.portals = res.tenants;
    return res;
  };
}

export default SpacesStore;
