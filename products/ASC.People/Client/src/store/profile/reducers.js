import { SET_PROFILE, CLEAN_PROFILE } from "./actions";

const initialState = {
  targetUser: null
};

const profileReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_PROFILE:
      return Object.assign({}, state, {
        targetUser: action.targetUser
      });
    case CLEAN_PROFILE:
      return initialState;
    default:
      return state;
  }
};

export default profileReducer;
