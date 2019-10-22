import { SET_CURRENT_USER, SET_MODULES, SET_SETTINGS, SET_IS_LOADED, LOGOUT, SET_PASSWORD_SETTINGS, SET_IS_CONFIRM_LOADED, SET_NEW_EMAIL, SET_NEW_SETTING_NODE } from './actions';
import isEmpty from 'lodash/isEmpty';
import config from "../../../package.json";

const settingsTree = [
    {
        title: 'Common',
        key: '0',
        icon: 'SettingsIcon',
        children: [
            {
                title: 'Customization',
                key: '0-0',
                icon: ''
            },
            {
                title: 'Modules & tools',
                key: '0-1',
                icon: ''
            },
            {
                title: 'White label',
                key: '0-2',
                icon: ''
            },
        ]
    },
    {
        title: 'Security',
        key: '1',
        icon: 'SettingsIcon',
        children: [
            {
                title: 'Portal Access',
                key: '1-0',
                icon: ''
            },
            {
                title: 'Access Rights',
                key: '1-1',
                icon: ''
            },
            {
                title: 'Login History',
                key: '1-2',
                icon: ''
            },
            {
                title: 'Audit Trail',
                key: '1-3',
                icon: ''
            },
        ]
    },
    {
        title: 'Data Management',
        key: '2',
        icon: 'SettingsIcon',
        children: [
            {
                title: 'Migration',
                key: '2-0',
                icon: ''
            },
            {
                title: 'Backup',
                key: '2-1',
                icon: ''
            },
            {
                title: 'Portal Deactivation/Deletion',
                key: '2-2',
                icon: ''
            },
        ]
    },
    {
        title: 'Integration',
        key: '3',
        icon: 'SettingsIcon',
        children: [
            {
                title: 'Third-Party Services',
                key: '3-0',
                icon: ''
            },
            {
                title: 'SMTP Settings',
                key: '3-1',
                icon: ''
            }
        ]
    },
    {
        title: 'Statistics',
        key: '4',
        icon: 'SettingsIcon',
    },
];

const initialState = {
    isAuthenticated: false,
    isLoaded: false,
    isConfirmLoaded: false,
    user: {},
    modules: [],
    settings: {
        currentProductId: "home",
        culture: "en-US",
        trustedDomains: [],
        trustedDomainsType: 1,
        timezone: "UTC",
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
            list: settingsTree,
            selected: ['0-0']
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
        case SET_NEW_SETTING_NODE:
            return Object.assign({}, state, {
                settings: { ...state.settings, settingsTree: { ...state.settings.settingsTree, selected: action.selected } }
            });
        case LOGOUT:
            return initialState;
        default:
            return state;
    }
}

export default authReducer;