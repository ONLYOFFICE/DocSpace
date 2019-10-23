import { SET_CURRENT_USER, SET_MODULES, SET_SETTINGS, SET_IS_LOADED, LOGOUT, SET_PASSWORD_SETTINGS, SET_IS_CONFIRM_LOADED, SET_NEW_EMAIL, SET_NEW_SETTING_NODE } from './actions';
import isEmpty from 'lodash/isEmpty';
import config from "../../../package.json";

const settingsTree = [
    {
        title: 'Common',
        key: '0',
        icon: 'SettingsIcon',
        link: 'common',
        children: [
            {
                title: 'Customization',
                key: '0-0',
                icon: '',
                link: 'customization',
            },
            {
                title: 'Modules & tools',
                key: '0-1',
                icon: '',
                link: 'modules-and-tools',
            },
            {
                title: 'White label',
                key: '0-2',
                icon: '',
                link: 'white-label',
            },
        ]
    },
    {
        title: 'Security',
        key: '1',
        icon: 'SettingsIcon',
        link: 'security',
        children: [
            {
                title: 'Portal Access',
                key: '1-0',
                icon: '',
                link: 'portal-access',
            },
            {
                title: 'Access Rights',
                key: '1-1',
                icon: '',
                link: 'access-rights',
            },
            {
                title: 'Login History',
                key: '1-2',
                icon: '',
                link: 'login-history',
            },
            {
                title: 'Audit Trail',
                key: '1-3',
                icon: '',
                link: 'audit-trail',
            },
        ]
    },
    {
        title: 'Data Management',
        key: '2',
        icon: 'SettingsIcon',
        link: 'data-management',
        children: [
            {
                title: 'Migration',
                key: '2-0',
                icon: '',
                link: 'migration',
            },
            {
                title: 'Backup',
                key: '2-1',
                icon: '',
                link: 'backup',
            },
            {
                title: 'Portal Deactivation/Deletion',
                key: '2-2',
                icon: '',
                link: 'portal-deactivation-deletion',
            },
        ]
    },
    {
        title: 'Integration',
        key: '3',
        icon: 'SettingsIcon',
        link: 'integration',
        children: [
            {
                title: 'Third-Party Services',
                key: '3-0',
                icon: '',
                link: 'third-party-services',
            },
            {
                title: 'SMTP Settings',
                key: '3-1',
                icon: '',
                link: 'smtp-settings',
            }
        ]
    },
    {
        title: 'Statistics',
        key: '4',
        icon: 'SettingsIcon',
        link: 'statistics',
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
            selectedKey: ['0-0'],
            selectedTitle: 'Common',
            selectedSubtitle: 'Customization',
            selectedLink: '/common/customization',
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
                settings: { ...state.settings, settingsTree: { ...state.settings.settingsTree, ...action.selectedNodeData } }
            });
        case LOGOUT:
            return initialState;
        default:
            return state;
    }
}

export default authReducer;