import * as api from "../../store/services/api";
import { setGroups, fetchPeopleByFilter } from "../people/actions";

export const SET_GROUP = "SET_GROUP";
export const CLEAN_GROUP = "CLEAN_GROUP";

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
    api.getGroup(groupId).then(res => {
      checkResponseError(res);
      dispatch(setGroup(res.data.response || null));
    });
  };
}

export function createGroup(groupName, groupManager, members) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { groups, filter } = people;

    let newGroup;

    return api
      .createGroup(groupName, groupManager, members)
      .then(res => {
        checkResponseError(res);
        newGroup = res.data.response;

        //dispatch(setGroup(newGroup));
        return dispatch(setGroups([...groups, newGroup]));
      })
      .then(() => {
        return fetchPeopleByFilter(dispatch, filter);
      })
      .then(() => {
        return Promise.resolve(newGroup);
      });
  };
}

export function updateGroup(id, groupName, groupManager, members) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { groups, filter } = people;

    let newGroup;

    return api
      .updateGroup(id, groupName, groupManager, members)
      .then(res => {
        checkResponseError(res);
        newGroup = res.data.response;

        //dispatch(setGroup(newGroup));

        const newGroups = groups.map(g =>
          g.id === newGroup.id ? newGroup : g
        );

        return dispatch(setGroups(newGroups));
      })
      .then(() => {
        return fetchPeopleByFilter(dispatch, filter);
      })
      .then(() => {
        return Promise.resolve(newGroup);
      });
  };
}
