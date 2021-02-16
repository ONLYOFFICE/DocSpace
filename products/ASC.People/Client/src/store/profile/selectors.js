import { createSelector } from "reselect";
import { store } from "asc-web-common";
const { isAdmin, getCurrentUserId } = store.auth.selectors;

const getProfileId = (state) =>
  state.profile.targetUser && state.profile.targetUser.id;

const getIsAdminProfile = (state) => true;
// state.profile.targetUser.listAdminModules &&
//state.profile.targetUser.listAdminModules.includes("people");

// export const getDisableProfileType = createSelector(
//   [isAdmin, getCurrentUserId, getProfileId, getIsAdminProfile],
//   (isAdmin, currentUserId, profileId, isAdminProfile) => {
//     return currentUserId === profileId || !isAdmin || isAdminProfile
//       ? true
//       : false;
//   }
// );
