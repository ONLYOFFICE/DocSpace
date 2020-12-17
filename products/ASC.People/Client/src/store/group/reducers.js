import { SET_GROUP, CLEAN_GROUP } from "./actions";

const initialState = {
  targetGroup: null,
};

const groupReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_GROUP:
      return Object.assign({}, state, {
        targetGroup: action.targetGroup,
      });
    case CLEAN_GROUP:
      return initialState;
    default:
      return state;
  }
};

export default groupReducer;
