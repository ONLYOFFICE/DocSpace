import * as api from "../services/api";
import axios from "axios";
import { getSelectorOptions, getUserOptions } from "./selectors";
import Filter from "./filter";

export const SET_USERS = "SET_USERS";
export const SET_ADMINS = "SET_ADMINS";
export const SET_OWNER = "SET_OWNER";
export const SET_FILTER = "SET_FILTER";
export const SET_LOGO_TEXT = "SET_LOGO_TEXT";
export const SET_LOGO_SIZES = "SET_LOGO_SIZES";
export const SET_LOGO_URLS = "SET_LOGO_URLS";

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
        axios.all([
          api.getUserList(filterData),
          api.getListAdmins(filterData),
          api.getAdmins(false)
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
        api.getAdmins(true)
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
