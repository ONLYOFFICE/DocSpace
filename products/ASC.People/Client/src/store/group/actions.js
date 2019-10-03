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

    return api
      .createGroup(groupName, groupManager, members)
      .then(res => {
        checkResponseError(res);
        const newGroup = res.data.response;

        dispatch(setGroups([...groups, newGroup]))
        dispatch(resetGroup());

        return Promise.resolve(newGroup);
      }).then((group) => {
        const newFilter = filter.clone();
        newFilter.group = group.id;
        return fetchPeopleByFilter(dispatch, newFilter);
      });;
  };
}

export function updateGroup(id, groupName, groupManager, members) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { groups, filter } = people;

    return api
      .updateGroup(id, groupName, groupManager, members)
      .then(res => {
        checkResponseError(res);
        const newGroup = res.data.response;

        const newGroups = groups.map(g =>
          g.id === newGroup.id ? newGroup : g
        );

        dispatch(setGroups(newGroups));
        dispatch(resetGroup());

        return Promise.resolve(newGroup);
      }).then((group) => {
        const newFilter = filter.clone();
        newFilter.group = group.id;
        return fetchPeopleByFilter(dispatch, newFilter);
      });
  };
}

export function deleteGroup(id) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { groups, filter } = people;

    return api
      .deleteGroup(id)
      .then(res => {
        checkResponseError(res);
        return dispatch(setGroups(groups.filter(g => g.id !== id)));
      })
      .then(() => {
        const newFilter = filter.clone(true);
        return fetchPeopleByFilter(dispatch, newFilter);
      });
  };
}
