import { api, store } from "asc-web-common";
const {
  setCurrentUser,
  loadInitInfo,
  login,
  getPortalPasswordSettings,
  setNewEmail,
  logout,
} = store.auth.actions;

export const SET_IS_CONFIRM_LOADED = "SET_IS_CONFIRM_LOADED";

export function setIsConfirmLoaded(isConfirmLoaded) {
  return {
    type: SET_IS_CONFIRM_LOADED,
    isConfirmLoaded,
  };
}

export function getConfirmationInfo(token) {
  return (dispatch) => {
    return getPortalPasswordSettings(dispatch, token).then(() =>
      dispatch(setIsConfirmLoaded(true))
    );
  };
}

export function createConfirmUser(registerData, loginData, key) {
  const data = Object.assign({}, registerData, loginData);
  return (dispatch) => {
    return api.people
      .createUser(data, key)
      .then((user) => dispatch(setCurrentUser(user)))
      .then(() => api.user.login(loginData.userName, loginData.passwordHash))
      .then(() => loadInitInfo(dispatch));
  };
}

export function activateConfirmUser(
  personalData,
  loginData,
  key,
  userId,
  activationStatus
) {
  const changedData = {
    id: userId,
    FirstName: personalData.firstname,
    LastName: personalData.lastname,
  };

  return (dispatch) => {
    return api.people
      .changePassword(userId, loginData.passwordHash, key)
      .then((data) => {
        return api.people.updateActivationStatus(activationStatus, userId, key);
      })
      .then((data) => {
        return dispatch(login(loginData.userName, loginData.passwordHash));
      })
      .then((data) => {
        return api.people.updateUser(changedData);
      })
      .then((user) => dispatch(setCurrentUser(user)));
  };
}

export function changeEmail(userId, email, key) {
  return (dispatch) => {
    return api.people
      .changeEmail(userId, email, key)
      .then((user) => dispatch(setNewEmail(user.email)));
  };
}

export function changePassword(userId, passwordHash, key) {
  return (dispatch) => {
    return api.people
      .changePassword(userId, passwordHash, key)
      .then(() => logout(dispatch));
  };
}
