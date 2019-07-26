import {
  SET_GROUPS,
  SET_USERS,
  SET_TARGET_USER,
  SET_FILTER,
  SET_PAGING,
  SET_SELECTION,
  SELECT_USER,
  DESELECT_USER,
  SET_SELECTED
} from "../actions/actionTypes";
import _ from "lodash";
import Filter from '../helpers/filter';

const initialState = {
  users: [],
  groups: [],
  targetUser: null,
  filter: Filter.getDefault(),
  paging: {
    page: 0,
    pageCount: 25,
    prev: false,
    next: true
  },
  selection: [],
  selected: "none"
};

const people = (state = initialState, action) => {
  switch (action.type) {
    case SET_GROUPS:
      return Object.assign({}, state, {
        groups: action.groups
      });
    case SET_USERS:
      return Object.assign({}, state, {
        users: action.users
      });
    case SET_TARGET_USER:
      return Object.assign({}, state, {
        targetUser: action.targetUser
      });
    case SET_FILTER:
      return Object.assign({}, state, {
        filter: action.filter
      });
    case SET_PAGING:
      return Object.assign({}, state, {
        paging: action.paging
      });
    case SET_SELECTION:
      return Object.assign({}, state, {
        selection: action.selection
      });
    case SELECT_USER:
      if (!isSelected(state.selection, action.user.id)) {
        return Object.assign({}, state, {
          selection: [...state.selection, action.user]
        });
      } else return state;
    case DESELECT_USER:
      if (isSelected(state.selection, action.user.id)) {
        const newSelection = _.filter(state.selection, function(obj) {
          return obj.id !== action.user.id;
        });
        return Object.assign({}, state, {
          selection: newSelection
        });
      } else return state;
    case SET_SELECTED:
      return Object.assign({}, state, {
        selected: action.selected
      });
    default:
      return state;
  }
};

export function getSelectedUser(selection, userId) {
  return _.find(selection, function(obj) {
    return obj.id === userId;
  });
}

export function isSelected(selection, userId) {
  return getSelectedUser(selection, userId) !== undefined;
}

export default people;
