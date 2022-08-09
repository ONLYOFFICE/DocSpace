import { makeAutoObservable } from "mobx";
import {
  getInvitationLinks,
  getShortenedLink,
} from "@docspace/common/api/portal";

class InviteLinksStore {
  userLink = null;
  guestLink = null;
  authStore = null;

  constructor(authStore) {
    this.authStore = authStore;
    makeAutoObservable(this);
  }

  setUserLink = (link) => {
    this.userLink = link;
  };
  setGuestLink = (link) => {
    this.guestLink = link;
  };

  getPortalInviteLinks = async () => {
    const isViewerAdmin = this.authStore.isAdmin;

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
