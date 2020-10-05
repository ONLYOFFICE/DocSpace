import { getUserByUserName } from "../people/selectors";
import { fetchPeople, updateProfileInUsers } from "../people/actions";
import { store, api } from "asc-web-common";
const { setCurrentUser } = store.auth.actions;
const { isMe } = store.auth.selectors;

export const SET_PROFILE = "SET_PROFILE";
export const CLEAN_PROFILE = "CLEAN_PROFILE";

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
    const { auth, people } = getState();

    if (isMe(auth.user, userName)) {
      dispatch(setProfile(auth.user));
    } else {
      const user = getUserByUserName(people.users, userName);
      if (!user) {
        api.people.getUser(userName).then((user) => {
          dispatch(setProfile(user));
        });
      } else {
        dispatch(setProfile(user));
      }
    }
  };
}

export function createProfile(profile) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { filter } = people;
    const member = employeeWrapperToMemberModel(profile);
    let result;

    return (
      api.people
        .createUser(member)
        .then((user) => {
          result = user;
          return dispatch(setProfile(user));
        })
        /*
      .then(() => {
        return fetchPeople(filter, dispatch);
      })*/
        .then(() => {
          return Promise.resolve(result);
        })
    );
  };
}

export function updateProfile(profile) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { filter } = people;
    const member = employeeWrapperToMemberModel(profile);
    let result;

    return (
      api.people
        .updateUser(member)
        .then((user) => {
          result = user;
          return Promise.resolve(dispatch(setProfile(user)));
        })
        /*.then(() => {
            return fetchPeople(filter, dispatch);
        })*/
        .then(() => {
          return Promise.resolve(result);
        })
    );
  };
}

export function updateProfileCulture(id, culture) {
  return (dispatch) => {
    return api.people.updateUserCulture(id, culture).then((user) => {
      dispatch(setCurrentUser(user));
      return dispatch(setProfile(user));
    });
  };
}

export function getUserPhoto(id) {
  return api.people.getUserPhoto(id);
}

export function updateCreatedAvatar(avatar) {
  return (dispatch, getState) => {
    const { big, max, medium, small } = avatar;
    const { profile, people } = getState();
    const { filter } = people;
    const newProfile = {
      ...profile.targetUser,
      avatarMax: max,
      avatarMedium: medium,
      avatar: big,
      avatarSmall: small,
    };
    //fetchPeople(filter);
    return dispatch(setProfile(newProfile));
  };
}
