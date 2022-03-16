import { request } from "../client";

export function getShortenedLink(link) {
  return request({
    method: "put",
    url: "/portal/getshortenlink.json",
    data: { link },
  });
}

const GUEST_INVITE_LINK = "guestInvitationLink";
const USER_INVITE_LINK = "userInvitationLink";
const INVITE_LINK_TTL = "localStorageLinkTtl";
const LINKS_TTL = 6 * 3600 * 1000;

export function getInvitationLink(isGuest) {
  const curLinksTtl = localStorage.getItem(INVITE_LINK_TTL);
  const now = +new Date();

  if (!curLinksTtl) {
    localStorage.setItem(INVITE_LINK_TTL, now);
  } else if (now - curLinksTtl > LINKS_TTL) {
    localStorage.removeItem(GUEST_INVITE_LINK);
    localStorage.removeItem(USER_INVITE_LINK);
    localStorage.setItem(INVITE_LINK_TTL, now);
  }

  const link = localStorage.getItem(
    isGuest ? GUEST_INVITE_LINK : USER_INVITE_LINK
  );

  return link
    ? Promise.resolve(link)
    : request({
        method: "get",
        url: `/portal/users/invite/${isGuest ? 2 : 1}.json`,
      }).then((link) => {
        localStorage.setItem(
          isGuest ? GUEST_INVITE_LINK : USER_INVITE_LINK,
          link
        );
        return Promise.resolve(link);
      });
}

export function getInvitationLinks() {
  const isGuest = true;
  return Promise.all([getInvitationLink(), getInvitationLink(isGuest)]).then(
    ([userInvitationLinkResp, guestInvitationLinkResp]) => {
      return Promise.resolve({
        userLink: userInvitationLinkResp,
        guestLink: guestInvitationLinkResp,
      });
    }
  );
}

export function setPortalRename(alias) {
  return request({
    method: "put",
    url: "/portal/portalrename.json",
    data: { alias },
  });
}
