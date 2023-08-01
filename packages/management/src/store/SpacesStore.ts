import { makeAutoObservable } from "mobx";
import { getLogoFromPath } from "@docspace/common/utils";
import {
  deletePortal,
  getDomainName,
  setDomainName,
  setPortalName,
  createNewPortal,
  getAllPortals,
  getPortalStatus,
} from "@docspace/common/api/management";
import { TNewPortalData, TPortals } from "SRC_DIR/types/spaces";

class SpacesStore {
  isInit = false;
  brandingStore = null;

  portals: TPortals[] = [];
  domain: string | null = null;

  createPortalDialogVisible = false;
  domainDialogVisible = false;

  constructor(brandingStore) {
    this.brandingStore = brandingStore;
    makeAutoObservable(this);
  }

  initStore = async () => {
    if (this.isInit) return;
    this.isInit = true;
    this.brandingStore.initStore();

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

  get faviconLogo() {
    const logos = this.brandingStore.whiteLabelLogos;
    if (!logos) return;
    return getLogoFromPath(logos[2]?.path?.light);
  }

  setPortalSettings = async (domain: string, portalName: string) => {
    const dmn = await setDomainName(domain);
    const { settings } = dmn;
    this.domain = settings;
    if (!portalName) return;
    const name = await setPortalName(portalName);
  };

  setPortalName = async (portalName: string) => {
    const name = await setPortalName(portalName);
  };

  createNewPortal = async (data: TNewPortalData) => {
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
