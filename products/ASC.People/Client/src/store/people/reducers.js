import {
    SET_GROUPS,
    SET_USERS,
    SET_SELECTION,
    SELECT_USER,
    DESELECT_USER,
    SET_SELECTED
} from "./actions";
import { isSelected, skipUser } from './selectors';

const initialState = {
    users: [],
    groups: [],
    selection: [],
    selected: "none"
};

const peopleReducer = (state = initialState, action) => {
    switch (action.type) {
        case SET_GROUPS:
            return Object.assign({}, state, {
                groups: action.groups
            });
        case SET_USERS:
            return Object.assign({}, state, {
                users: action.users
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
                return Object.assign({}, state, {
                    selection: skipUser(state.selection, action.user.id)
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

export default peopleReducer;
