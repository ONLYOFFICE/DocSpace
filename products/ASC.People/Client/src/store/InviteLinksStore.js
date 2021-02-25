import { action, makeObservable, observable } from "mobx";
import { getInvitationLinks } from "@appserver/common/api/portal";
import store from "@appserver/common/store";

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

    const res = await getInvitationLinks();
    this.inviteLinks.userLink = res.userLink;
    this.inviteLinks.guestLink = res.guestLink;
  };
}

export default InviteLinksStore;
