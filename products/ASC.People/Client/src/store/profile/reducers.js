import { SET_PROFILE, CLEAN_PROFILE, SET_AVATAR_MAX } from "./actions";

const initialState = {
  targetUser: null,
  avatarMax: null,
};

const profileReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_PROFILE:
      return Object.assign({}, state, {
        targetUser: action.targetUser
      });
    case CLEAN_PROFILE:
      return initialState;
    case SET_AVATAR_MAX:
      return Object.assign({}, state, {
        avatarMax: action.avatarMax,
      });
    default:
      return state;
  }
};

export default profileReducer;
