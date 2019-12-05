import { SET_PASSWORD_SETTINGS, SET_IS_CONFIRM_LOADED } from './actions';

const initialState = {
  isConfirmLoaded: false,
  passwordSettings: {}
};

const confirmReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_PASSWORD_SETTINGS:
      return Object.assign({}, state, {
        passwordSettings: action.passwordSettings
      });
    case SET_IS_CONFIRM_LOADED:
      return Object.assign({}, state, {
        isConfirmLoaded: action.isConfirmLoaded
      });
    default:
      return state;
  }
}

export default confirmReducer;