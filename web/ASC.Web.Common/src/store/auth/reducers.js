import {
  SET_CURRENT_USER,
  SET_MODULE,
  SET_MODULES,
  SET_IS_LOADED,
  LOGOUT,
  SET_SETTINGS
} from "./actions";
import isEmpty from "lodash/isEmpty";

const initialState = {
  isAuthenticated: false,
  isLoaded: false,
  user: {},
  modules: [],
  settings: {
    currentProductId: null,
    culture: "en-US",
    trustedDomains: [],
    trustedDomainsType: 1,
    timezone: "UTC",
    utcOffset: "00:00:00",
    utcHoursOffset: 0,
    homepage: "",
    datePattern: "M/d/yyyy",
    datePatternJQ: "00/00/0000",
    dateTimePattern: "dddd, MMMM d, yyyy h:mm:ss tt",
    datepicker: {
      datePattern: "mm/dd/yy",
      dateTimePattern: "DD, mm dd, yy h:mm:ss tt",
      timePattern: "h:mm tt"
    },
    hasShortenService: false
  }
};

const authReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_CURRENT_USER:
      return Object.assign({}, state, {
        isAuthenticated: !isEmpty(action.user),
        user: action.user
      });
    case SET_MODULE:
      return Object.assign({}, state, {
        settings: {
          ...state.settings,
          currentProductId: action.module.id,
          homepage: action.module.homepage
        }
      });
    case SET_MODULES:
      return Object.assign({}, state, {
        modules: action.modules
      });
    case SET_SETTINGS:
      return Object.assign({}, state, {
        settings: { ...state.settings, ...action.settings }
      });
    case SET_IS_LOADED:
      return Object.assign({}, state, {
        isLoaded: action.isLoaded
      });
    case LOGOUT:
      return Object.assign({}, initialState, {
        isLoaded: true
      });
    default:
      return state;
  }
};

export default authReducer;
