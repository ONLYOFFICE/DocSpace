import {
  SET_USERS,
  SET_ADMINS,
  SET_OWNER,
  SET_OPTIONS,
  SET_FILTER,
  SET_LOGO_TEXT,
  SET_LOGO_SIZES,
  SET_LOGO_URLS,
  SET_CONSUMERS,
  SET_SELECTED_CONSUMER,
} from "./actions";
import { api } from "asc-web-common";
const { Filter } = api;

const initialState = {
  common: {
    whiteLabel: {
      logoSizes: [],
      logoText: null,
      logoUrls: [],
    },
  },
  security: {
    accessRight: {
      options: [],
      users: [],
      admins: [],
      owner: {},
      filter: Filter.getDefault(),
    },
  },
  integration: {
    consumers: [],
    selectedConsumer: {},
  },
};

const peopleReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_OPTIONS:
      return Object.assign({}, state, {
        security: Object.assign({}, state.security, {
          accessRight: Object.assign({}, state.security.accessRight, {
            options: action.options,
          }),
        }),
      });
    case SET_USERS:
      return Object.assign({}, state, {
        security: Object.assign({}, state.security, {
          accessRight: Object.assign({}, state.security.accessRight, {
            users: action.users,
          }),
        }),
      });
    case SET_ADMINS:
      return Object.assign({}, state, {
        security: Object.assign({}, state.security, {
          accessRight: Object.assign({}, state.security.accessRight, {
            admins: action.admins,
          }),
        }),
      });
    case SET_OWNER:
      return Object.assign({}, state, {
        security: Object.assign({}, state.security, {
          accessRight: Object.assign({}, state.security.accessRight, {
            owner: action.owner,
          }),
        }),
      });
    case SET_FILTER:
      return Object.assign({}, state, {
        security: Object.assign({}, state.security, {
          accessRight: Object.assign({}, state.security.accessRight, {
            filter: action.filter,
          }),
        }),
      });

    case SET_LOGO_TEXT:
      return Object.assign({}, state, {
        common: {
          ...state.common,
          whiteLabel: { ...state.common.whiteLabel, logoText: action.text },
        },
      });

    case SET_LOGO_SIZES:
      return Object.assign({}, state, {
        common: {
          ...state.common,
          whiteLabel: { ...state.common.whiteLabel, logoSizes: action.sizes },
        },
      });

    case SET_LOGO_URLS:
      return Object.assign({}, state, {
        common: {
          ...state.common,
          whiteLabel: { ...state.common.whiteLabel, logoUrls: action.urls },
        },
      });

    case SET_CONSUMERS:
      return {
        ...state,
        integration: {
          ...state.integration,
          consumers: action.consumers,
        },
      };

    case SET_SELECTED_CONSUMER:
      return {
        ...state,
        integration: {
          ...state.integration,
          selectedConsumer:
            state.integration.consumers.find(
              (c) => c.name === action.selectedConsumer
            ) || {},
        },
      };

    default:
      return state;
  }
};

export default peopleReducer;
