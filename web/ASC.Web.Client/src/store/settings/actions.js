import * as api from "../services/api";
import axios from "axios";
import { getSelectorOptions, getUserOptions } from "./selectors";
import Filter from "./filter";

export const SET_USERS = "SET_USERS";
export const SET_ADMINS = "SET_ADMINS";
export const SET_OWNER = "SET_OWNER";
export const SET_OPTIONS = "SET_OPTIONS";
export const SET_FILTER = "SET_FILTER";
export const SET_LOGO_TEXT = "SET_LOGO_TEXT";
export const SET_LOGO_SIZES = "SET_LOGO_SIZES";
export const SET_LOGO_URLS = "SET_LOGO_URLS";

export function setOptions(options) {
  return {
    type: SET_OPTIONS,
    options
  };
}

export function setUsers(users) {
  return {
    type: SET_USERS,
    users
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

export function setLogoText(text) {
  return {
    type: SET_LOGO_TEXT,
    text
  };
}

export function setLogoSizes(sizes) {
  return {
    type: SET_LOGO_SIZES,
    sizes
  };
}

export function setLogoUrls(urls) {
  return {
    type: SET_LOGO_URLS,
    urls
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
        axios.all([api.getUserList(filterData), api.getListAdmins(filterData)])
      )
      .then(
        axios.spread((users, admins) => {
          const options = getUserOptions(users.items, admins.items);
          const newOptions = getSelectorOptions(options);
          filterData.total = admins.total;

          dispatch(setOptions(newOptions));
          dispatch(setAdmins(admins.items));
          dispatch(setFilter(filterData));
        })
      );
  };
}

export function getPortalOwner(userId) {
  return dispatch => {
    return api.getUserById(userId).then(owner => dispatch(setOwner(owner)));
  };
}

export function fetchPeople(filter) {
  let filterData = filter && filter.clone();
  if (!filterData) {
    filterData = Filter.getDefault();
  }

  return dispatch => {
    return axios
      .all([api.getUserList(filterData), api.getListAdmins(filterData)])
      .then(
        axios.spread((users, admins) => {
          const options = getUserOptions(users.items, admins.items);
          const newOptions = getSelectorOptions(options);
          const usersOptions = getSelectorOptions(users.items);
          filterData.total = admins.total;

          dispatch(setUsers(usersOptions));
          dispatch(setAdmins(admins.items));
          dispatch(setOptions(newOptions));
          dispatch(setFilter(filterData));
        })
      );
  };
}

export function getWhiteLabelLogoText() {
  return dispatch => {
    return api.getLogoText()
    .then(res => {
      dispatch(setLogoText(res));
    });
  };
};

export function getWhiteLabelLogoSizes() {
  return dispatch => {
    return api.getLogoSizes()
    .then(res => {
      dispatch(setLogoSizes(res));
    });
  };
};

export function getWhiteLabelLogoUrls() {
  return dispatch => {
    return api.getLogoUrls()
    .then(res => {
      dispatch(setLogoUrls(Object.values(res)));
    });
  };
};
