import { makeAutoObservable } from "mobx";
import {
  deletePortal,
  getDomainName,
  setDomainName,
  setPortalName,
  createNewPortal,
  getAllPortals,
} from "@docspace/common/api/management";

type NewPortalData = {
  firstName: string;
  lastName: string;
  email: string;
  portalName: string;
};

class SpacesStore {
  isInit = false;

  portals = []; // need interface
  domain: string | null = null;

  createPortalDialogVisible = false;
  domainDialogVisible = false;

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

  createNewPortal = async (data: NewPortalData) => {
    const register = await createNewPortal(data);
  };

  getAllPortals = async () => {
    const res = await getAllPortals();
    this.portals = res.tenants;
    return res;
  };

  setCreatePortalDialogVisible = (createPortalDialogVisible: boolean) => {
    this.createPortalDialogVisible = createPortalDialogVisible;
  };

  setChangeDomainDialogVisible = (domainDialogVisible: boolean) => {
    this.domainDialogVisible = domainDialogVisible;
  };
}

export default SpacesStore;
