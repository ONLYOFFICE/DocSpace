import * as api from "../services/api";
import axios from "axios";
import { getSelectorOptions, getUserOptions } from "./selectors";
import Filter from "./filter";

export const SET_USERS = "SET_USERS";
export const SET_ADMINS = "SET_ADMINS";
export const SET_OWNER = "SET_OWNER";

export const SET_FILTER = "SET_FILTER";
export const SET_GREETING_SETTINGS = "SET_GREETING_SETTINGS";

export function setUsers(options) {
  return {
    type: SET_USERS,
    options
  };
}

export function setAdmins(admins) {
  return {
    type: SET_ADMINS,
    admins
  };
}

export function setOwner(owner) {
  return {
    type: SET_OWNER,
    owner
  };
}

export function setFilter(filter) {
  return {
    type: SET_FILTER,
    filter
  };
}

export function setGreetingSettings(title) {
  return {
    type: SET_GREETING_SETTINGS,
    title
  };
}

export function changeAdmins(userIds, productId, isAdmin, filter) {
  let filterData = filter && filter.clone();
  if (!filterData) {
    filterData = Filter.getDefault();
  }
  return dispatch => {
    return axios
      .all(
        userIds.map(userId =>
          api.changeProductAdmin(userId, productId, isAdmin)
        )
      )
      .then(() =>
        axios.all([
          api.getUserList(filterData),
          api.getListAdmins(filterData),
          api.getAdmins()
        ])
      )
      .then(
        axios.spread((users, filterAdmins, admins) => {
          const options = getUserOptions(users, filterAdmins);
          const newOptions = getSelectorOptions(options);
          filterData.total = admins.length;

          dispatch(setUsers(newOptions));
          dispatch(setAdmins(filterAdmins));
          dispatch(setFilter(filterData));
        })
      );
  };
}

export function fetchPeople(filter) {
  let filterData = filter && filter.clone();
  if (!filterData) {
    filterData = Filter.getDefault();
  }

  return dispatch => {
    return axios
      .all([
        api.getUserList(filterData),
        api.getListAdmins(filterData),
        api.getAdmins()
      ])
      .then(
        axios.spread((users, filterAdmins, admins) => {
          const options = getUserOptions(users, admins);
          const newOptions = getSelectorOptions(options);
          filterData.total = admins.length;

          const owner = admins.find(x => x.isOwner);

          dispatch(setAdmins(filterAdmins));
          dispatch(setUsers(newOptions));
          dispatch(setFilter(filterData));
          dispatch(setOwner(owner));
        })
      );
  };
}

export function setGreetingTitle(greetingTitle) {
  return dispatch => {
    return api.setGreetingSettings(greetingTitle).then(res => {
      dispatch(setGreetingSettings(greetingTitle));
    });
  };
}

export function restoreGreetingTitle() {
  return dispatch => {
    return api.restoreGreetingSettings().then(res => {
      dispatch(setGreetingSettings(res.companyName));
    });
  };
}
