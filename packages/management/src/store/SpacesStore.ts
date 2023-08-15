import { settingsTree } from "SRC_DIR/utils/settingsTree";
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
  authStore = null;

  portals: TPortals[] = [];
  domain: string | null = null;

  createPortalDialogVisible = false;
  domainDialogVisible = false;

  constructor(authStore) {
    this.authStore = authStore;
    makeAutoObservable(this);
  }

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

    this.authStore.settingsStore.setPortalDomain(settings);

    if (settings) {
      const status = await getPortalStatus(settings);
    }
  };

  get isConnected() {
    return !!this.authStore.settingsStore.domain;
  }

  get faviconLogo() {
    const logos = this.authStore.settingsStore.whiteLabelLogoUrls;
    if (!logos) return;
    return getLogoFromPath(logos[2]?.path?.light);
  }

  setPortalSettings = async (domain: string, portalName: string) => {
    const dmn = await setDomainName(domain);
    const { settings } = dmn;
    this.authStore.settingsStore.setPortalDomain(settings);
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
    this.authStore.settingsStore.setPortals(res.tenants);
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
