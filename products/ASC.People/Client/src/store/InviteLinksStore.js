import { action, makeObservable, observable } from "mobx";
import { api, store } from "asc-web-common";

const { authStore } = store;

class InviteLinksStore {
  inviteLinks = {};

  constructor() {
    makeObservable(this, {
      inviteLinks: observable,
      getPortalInviteLinks: action,
    });
  }

  getPortalInviteLinks = async () => {
    const isViewerAdmin = authStore.isAdmin;

    if (!isViewerAdmin) return Promise.resolve();

    const res = await api.portal.getInvitationLinks();
    this.inviteLinks.userLink = res.userLink;
    this.inviteLinks.guestLink = res.guestLink;
  };
}

export default InviteLinksStore;
