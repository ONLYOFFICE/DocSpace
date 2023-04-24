import { makeAutoObservable, runInAction } from "mobx";
import {
  getInvitationLinks,
  getShortenedLink,
} from "@docspace/common/api/portal";

class InviteLinksStore {
  peopleStore = null;
  userLink = null;
  guestLink = null;
  adminLink = null;
  collaboratorLink = null;

  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeAutoObservable(this);
  }

  setUserLink = (link) => {
    this.userLink = link;
  };

  setGuestLink = (link) => {
    this.guestLink = link;
  };

  setAdminLink = (link) => {
    this.adminLink = link;
  };

  setCollaboratorLink = (link) => {
    this.collaboratorLink = link;
  };

  getPortalInviteLinks = async () => {
    const isViewerAdmin = !this.peopleStore.authStore.isVisitor;

    if (!isViewerAdmin) return Promise.resolve();

    const links = await getInvitationLinks();

    runInAction(() => {
      this.setUserLink(links.userLink);
      this.setGuestLink(links.guestLink);
      this.setAdminLink(links.adminLink);
      this.setCollaboratorLink(links.collaboratorLink);
    });
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
