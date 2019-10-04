import * as api from "../../store/services/api";
import { setGroups, fetchPeople, fetchGroups } from "../people/actions";
import { checkResponseError } from "../../helpers/utils";
import history from "../../history";

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
    const { groups } = people;

    return api
      .createGroup(groupName, groupManager, members)
      .then(res => {
        checkResponseError(res);
        history.goBack();
        const newGroup = res.data.response;
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

    return api
      .updateGroup(id, groupName, groupManager, members)
      .then(res => {
        checkResponseError(res);
        history.goBack();
        const newGroup = res.data.response;
        dispatch(resetGroup());
        const newGroups = groups.map(g =>
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

    return api
      .deleteGroup(id)
      .then(res => {
        checkResponseError(res);
        return dispatch(setGroups(groups.filter(g => g.id !== id)));
      })
      .then(() => {
        const newFilter = filter.clone(true);
        return fetchPeople(newFilter, dispatch);
      });
  };
}
