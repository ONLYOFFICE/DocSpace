import * as api from "../../store/services/api";

export const SET_GROUP = "SET_PROFILE";
export const CLEAN_GROUP = "CLEAN_PROFILE";

export function setGroup(targetGroup) {
  return {
    type: SET_GROUP,
    targetGroup
  };
}

export function resetGroup() {
  return {
    type: CLEAN_GROUP
  };
}

export function checkResponseError(res) {
  if (res && res.data && res.data.error) {
    console.error(res.data.error);
    throw new Error(res.data.error.message);
  }
}

export function fetchGroup(groupId) {
  return dispatch => {
    api.getGroup(groupId)
    .then(res => {
      checkResponseError(res);
      dispatch(setGroup(res.data.response || null));
    });
  };
}
