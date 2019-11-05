import {
    SET_CURRENT_USER, SET_MODULES, SET_SETTINGS, SET_IS_LOADED, LOGOUT, SET_PASSWORD_SETTINGS, SET_IS_CONFIRM_LOADED, SET_NEW_EMAIL,
    GET_PORTAL_CULTURES, SET_PORTAL_LANGUAGE_AND_TIME, GET_TIMEZONES, SET_CURRENT_PRODUCT_ID
} from './actions';
import isEmpty from 'lodash/isEmpty';
import config from "../../../package.json";


const initialState = {
    isAuthenticated: false,
    isLoaded: false,
    isConfirmLoaded: false,
    user: {},
    modules: [],
    settings: {
        currentProductId: "home",
        culture: "en-US",
        cultures: [],
        trustedDomains: [],
        trustedDomainsType: 1,
        timezone: "UTC",
        timezones: [],
        utcOffset: "00:00:00",
        utcHoursOffset: 0,
        homepage: config.homepage,
        datePattern: "M/d/yyyy",
        datePatternJQ: "00/00/0000",
        dateTimePattern: "dddd, MMMM d, yyyy h:mm:ss tt",
        datepicker: {
            datePattern: "mm/dd/yy",
            dateTimePattern: "DD, mm dd, yy h:mm:ss tt",
            timePattern: "h:mm tt"
        },
        settingsTree: {
            selectedKey: ['0-0']
        }
    }/*,
    password: null*/
}

const authReducer = (state = initialState, action) => {
    switch (action.type) {
        case SET_CURRENT_USER:
            return Object.assign({}, state, {
                isAuthenticated: !isEmpty(action.user),
                user: action.user
            });
        case SET_MODULES:
            return Object.assign({}, state, {
                modules: action.modules
            });
        case SET_SETTINGS:
            return Object.assign({}, state, {
                settings: { ...state.settings, ...action.settings }
            });
        case GET_PORTAL_CULTURES:
            return Object.assign({}, state, {
                settings: { ...state.settings, cultures: action.cultures }
            });
        case SET_PASSWORD_SETTINGS:
            return Object.assign({}, state, {
                settings: { ...state.settings, passwordSettings: action.passwordSettings }
            });
        case SET_IS_LOADED:
            return Object.assign({}, state, {
                isLoaded: action.isLoaded
            });
        case SET_IS_CONFIRM_LOADED:
            return Object.assign({}, state, {
                isConfirmLoaded: action.isConfirmLoaded
            });
        case SET_NEW_EMAIL:
            return Object.assign({}, state, {
                user: { ...state.user, email: action.email }
            });
        case SET_PORTAL_LANGUAGE_AND_TIME:
            return Object.assign({}, state, {
                settings: { ...state.settings, culture: action.newSettings.lng, timezone: action.newSettings.timeZoneID }
            });
        case GET_TIMEZONES:
            return Object.assign({}, state, {
                settings: { ...state.settings, timezones: action.timezones }
            });
        case SET_CURRENT_PRODUCT_ID:
            return Object.assign({}, state, {
                settings: { ...state.settings, currentProductId: action.currentProductId }
            });
        case LOGOUT:
            return initialState;
        default:
            return state;
    }
}

export default authReducer;