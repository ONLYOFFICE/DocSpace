import { api, store } from "asc-web-common";
import axios from "axios";
const { Filter } = api;
const { setPortalLanguageAndTime, setTimezones, setGreetingSettings } = store.auth.actions;

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
          api.people.changeProductAdmin(userId, productId, isAdmin)
        )
      )
      .then(() => api.people.getListAdmins(filterData))
      .then(admins => {
        filterData.total = admins.total;
        dispatch(setAdmins(admins.items));
        dispatch(setFilter(filterData));
      });
  };
}

export function getPortalOwner(userId) {
  return dispatch => {
    return api.people.getUserById(userId).then(owner => dispatch(setOwner(owner)));
  };
}

export function fetchPeople(filter) {
  let filterData = filter && filter.clone();
  if (!filterData) {
    filterData = Filter.getDefault();
  }

  return dispatch => {
    return api.people.getListAdmins(filterData)
      .then(admins => {
        filterData.total = admins.total;
        dispatch(setAdmins(admins.items));
        dispatch(setFilter(filterData));
      });
  };
}

export function getUpdateListAdmin(filter) {
  let filterData = filter && filter.clone();
  if (!filterData) {
    filterData = Filter.getDefault();
  }
  return dispatch => {
    return api.people.getListAdmins(filterData).then(admins => {
      filterData.total = admins.total;

      dispatch(setAdmins(admins.items));
      dispatch(setFilter(filterData));
    });
  };
}

export function getWhiteLabelLogoText() {
  return dispatch => {
    return api.settings.getLogoText()
    .then(res => {
      dispatch(setLogoText(res));
    });
  };
}

export function getWhiteLabelLogoSizes() {
  return dispatch => {
    return api.settings.getLogoSizes()
    .then(res => {
      dispatch(setLogoSizes(res));
    });
  };
}

export function getWhiteLabelLogoUrls() {
  return dispatch => {
    return api.settings.getLogoUrls()
    .then(res => {
      dispatch(setLogoUrls(Object.values(res)));
    });
  };
}

export function setLanguageAndTime(lng, timeZoneID) {
  return dispatch => {
    return api.settings
      .setLanguageAndTime(lng, timeZoneID)
      .then(() => dispatch(setPortalLanguageAndTime({ lng, timeZoneID })));
  };
};

export function getPortalTimezones() {
  return dispatch => {
    return api.settings.getPortalTimezones().then(timezones => {
      dispatch(setTimezones(timezones));
    });
  };
};

export function setGreetingTitle(greetingTitle) {
  return dispatch => {
    return api.settings.setGreetingSettings(greetingTitle).then(() => {
      dispatch(setGreetingSettings(greetingTitle));
    });
  };
}

export function restoreGreetingTitle() {
  return dispatch => {
    return api.settings.restoreGreetingSettings().then(res => {
      dispatch(setGreetingSettings(res.companyName));
    });
  };
}