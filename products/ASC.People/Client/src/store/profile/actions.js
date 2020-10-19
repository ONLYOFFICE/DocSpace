import { updateUserList } from "../people/actions";
import { store, api } from "asc-web-common";
const { setCurrentUser } = store.auth.actions;
const { isMe } = store.auth.selectors;

export const SET_PROFILE = "SET_PROFILE";
export const CLEAN_PROFILE = "CLEAN_PROFILE";
export const SET_AVATAR_MAX = "SET_AVATAR_MAX";
export const SET_CREATED_AVATAR = "SET_CREATED_AVATAR";
export const SET_CROPPED_AVATAR = "SET_CROPPED_AVATAR";

export function setProfile(targetUser) {
  return {
    type: SET_PROFILE,
    targetUser,
  };
}

export function resetProfile() {
  return {
    type: CLEAN_PROFILE,
  };
}

export function employeeWrapperToMemberModel(profile) {
  const comment = profile.notes;
  const department = profile.groups
    ? profile.groups.map((group) => group.id)
    : [];
  const worksFrom = profile.workFrom;

  return { ...profile, comment, department, worksFrom };
}

export function fetchProfile(userName) {
  return (dispatch, getState) => {
    const { auth } = getState();

    if (isMe(auth.user, userName)) {
      dispatch(setProfile(auth.user));
    } else {
      api.people.getUser(userName).then((user) => {
        dispatch(setProfile(user));
      });
    }
  };
}
export function createProfile(profile) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { filter } = people;
    const member = employeeWrapperToMemberModel(profile);
    let result;

    return api.people
      .createUser(member)
      .then((user) => {
        result = user;
        return dispatch(setProfile(user));
      })
      .then(() => {
        return updateUserList(dispatch, filter);
      })
      .then(() => {
        return Promise.resolve(result);
      });
  };
}

export function updateProfile(profile) {
  return (dispatch) => {
    const member = employeeWrapperToMemberModel(profile);
    let result;

    return api.people
      .updateUser(member)
      .then((user) => {
        result = user;
        return Promise.resolve(dispatch(setProfile(user)));
      })
      .then(() => {
        return Promise.resolve(result);
      });
  };
}
export function updateProfileCulture(id, culture) {
  return (dispatch, getState) => {
    return api.people
      .updateUserCulture(id, culture)
      .then((user) => {
        dispatch(setCurrentUser(user));
        return dispatch(setProfile(user));
      })
      .then(() => caches.delete("api-cache"))
      .then(() => {
        const { getCurrentCustomSchema, getModules } = store.auth.actions;
        const state = getState();
        const { nameSchemaId } = state.auth.settings;

        if (!nameSchemaId) return getModules();

        const requests = [
          getModules(dispatch),
          getCurrentCustomSchema(dispatch, nameSchemaId),
        ];

        return Promise.all(requests);
      });
  };
}

export function getUserPhoto(id) {
  return api.people.getUserPhoto(id);
}

export function updateCreatedAvatar(avatar) {
  return (dispatch, getState) => {
    const { big, max, medium, small } = avatar;
    const { profile } = getState();
    const newProfile = {
      ...profile.targetUser,
      avatarMax: max,
      avatarMedium: medium,
      avatar: big,
      avatarSmall: small,
    };
    return dispatch(setProfile(newProfile));
  };
}

export function setAvatarMax(avatarMax) {
  return {
    type: SET_AVATAR_MAX,
    avatarMax,
  };
}

export function setCreatedAvatar(avatar) {
  return {
    type: SET_CREATED_AVATAR,
    avatar,
  };
}

export function setCroppedAvatar(croppedAvatar) {
  return {
    type: SET_CROPPED_AVATAR,
    croppedAvatar,
  };
}
