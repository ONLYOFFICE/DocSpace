import { setGroups, fetchPeople } from "../people/actions";
import { api } from "asc-web-common";

export const SET_GROUP = "SET_GROUP";
export const CLEAN_GROUP = "CLEAN_GROUP";

export function setGroup(targetGroup) {
  return {
    type: SET_GROUP,
    targetGroup,
  };
}

export function resetGroup() {
  return {
    type: CLEAN_GROUP,
  };
}

export function fetchGroup(groupId) {
  return (dispatch) => {
    api.groups
      .getGroup(groupId)
      .then((group) => dispatch(setGroup(group || null)));
  };
}

export function createGroup(groupName, groupManager, members) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { groups } = people;

    return api.groups
      .createGroup(groupName, groupManager, members)
      .then((newGroup) => {
        dispatch(resetGroup());
        dispatch(setGroups([...groups, newGroup]));
        return Promise.resolve(newGroup);
      });
  };
}

export function updateGroup(id, groupName, groupManager, members) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { groups } = people;

    return api.groups
      .updateGroup(id, groupName, groupManager, members)
      .then((newGroup) => {
        dispatch(resetGroup());
        const newGroups = groups.map((g) =>
          g.id === newGroup.id ? newGroup : g
        );
        dispatch(setGroups(newGroups));
        return Promise.resolve(newGroup);
      });
  };
}

export function deleteGroup(id) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { groups, filter } = people;

    return api.groups
      .deleteGroup(id)
      .then((res) => {
        return dispatch(setGroups(groups.filter((g) => g.id !== id)));
      })
      .then(() => {
        const newFilter = filter.clone(true);
        return fetchPeople(newFilter, dispatch);
      });
  };
}
