import { SET_IS_CONFIRM_LOADED } from "./actions";

const initialState = {
  isConfirmLoaded: false,
};

const confirmReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_IS_CONFIRM_LOADED:
      return Object.assign({}, state, {
        isConfirmLoaded: action.isConfirmLoaded,
      });
    default:
      return state;
  }
};

export default confirmReducer;
