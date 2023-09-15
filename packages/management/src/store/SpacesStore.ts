import { makeAutoObservable } from "mobx";
import { getLogoFromPath } from "@docspace/common/utils";
import {
  deletePortal,
  getDomainName,
  setDomainName,
  setPortalName,
  createNewPortal,
  checkDomain,
} from "@docspace/common/api/management";
import { TNewPortalData } from "SRC_DIR/types/spaces";

class SpacesStore {
  authStore = null;

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

    const res = await deletePortal(data);
    return res;
  };

  getPortalDomain = async () => {
    const res = await getDomainName();
    const { settings } = res;

    this.authStore.settingsStore.setPortalDomain(settings);
  };

  get isConnected() {
    return (
      this.authStore.settingsStore.baseDomain &&
      this.authStore.settingsStore.baseDomain !== "localhost"
    );
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

    await setPortalName(portalName);
  };

  checkDomain = async (domain) => {
    const res = await checkDomain(domain);
    return res;
  };

  createNewPortal = async (data: TNewPortalData) => {
    const register = await createNewPortal(data);
    return register;
  };

  setCreatePortalDialogVisible = (createPortalDialogVisible: boolean) => {
    this.createPortalDialogVisible = createPortalDialogVisible;
  };

  setChangeDomainDialogVisible = (domainDialogVisible: boolean) => {
    this.domainDialogVisible = domainDialogVisible;
  };
}

export default SpacesStore;
