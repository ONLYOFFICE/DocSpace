import { getInvitationLinks } from "@appserver/common/src/api/portal";

export const SET_INVITE_LINKS = "SET_INVITE_LINKS";

export function setInviteLinks(userLink, guestLink) {
  return {
    type: SET_INVITE_LINKS,
    payload: {
      userLink,
      guestLink,
    },
  };
}

export function getPortalInviteLinks() {
  return (dispatch, getState) => {
    const { auth } = getState();
    if (!auth.user.isAdmin) return Promise.resolve();

    return getInvitationLinks().then((data) => {
      dispatch(setInviteLinks(data.userLink, data.guestLink));
    });
  };
}
