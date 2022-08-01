import { makeAutoObservable } from "mobx";
import {
  getInvitationLinks,
  getShortenedLink,
} from "@docspace/common/api/portal";
import store from "client/store";

const { auth: authStore } = store;
class InviteLinksStore {
  userLink = null;
  guestLink = null;

  constructor() {
    makeAutoObservable(this);
  }

  setUserLink = (link) => {
    this.userLink = link;
  };
  setGuestLink = (link) => {
    this.guestLink = link;
  };

  getPortalInviteLinks = async () => {
    const isViewerAdmin = authStore.isAdmin;

    if (!isViewerAdmin) return Promise.resolve();

    const links = await getInvitationLinks();
    this.setUserLink(links.userLink);
    this.setGuestLink(links.guestLink);
  };

  getShortenedLink = async (link, forUser = false) => {
    if (forUser) {
      const userLink = await getShortenedLink(link);
      this.setUserLink(userLink);
    } else {
      const guestLink = await getShortenedLink(link);
      this.setGuestLink(guestLink);
    }
  };
}

export default InviteLinksStore;
