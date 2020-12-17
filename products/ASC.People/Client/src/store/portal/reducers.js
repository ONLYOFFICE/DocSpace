import { SET_INVITE_LINKS } from "./actions";

const initialState = {
  inviteLinks: {},
};

const profileReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_INVITE_LINKS:
      return Object.assign({}, state, {
        inviteLinks: action.payload,
      });
    default:
      return state;
  }
};

export default profileReducer;
