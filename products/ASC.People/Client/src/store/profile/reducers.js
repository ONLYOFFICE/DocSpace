import {
  SET_PROFILE,
  CLEAN_PROFILE,
  SET_AVATAR_MAX,
  SET_CREATED_AVATAR,
  SET_CROPPED_AVATAR,
} from "./actions";

const initialState = {
  targetUser: null,
  avatarMax: null,
  createdAvatar: {
    tmpFile: "",
    image: null,
    defaultWidth: 0,
    defaultHeight: 0,
    x: 0,
    y: 0,
    width: 0,
    height: 0,
  },
  croppedAvatar: null,
};

const profileReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_PROFILE:
      return Object.assign({}, state, {
        targetUser: action.targetUser,
      });
    case CLEAN_PROFILE:
      return initialState;
    case SET_AVATAR_MAX:
      return Object.assign({}, state, {
        avatarMax: action.avatarMax,
      });
    case SET_CREATED_AVATAR:
      return Object.assign({}, state, {
        createdAvatar: action.avatar,
      });
    case SET_CROPPED_AVATAR:
      return Object.assign({}, state, {
        croppedAvatar: action.croppedAvatar,
      });
    default:
      return state;
  }
};

export default profileReducer;
