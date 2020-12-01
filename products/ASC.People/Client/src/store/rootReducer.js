import { combineReducers } from "redux";
import peopleReducer from "./people/reducers";
import profileReducer from "./profile/reducers";
import groupReducer from "./group/reducers";
import portalReducer from "./portal/reducers";
import { store } from "asc-web-common";
const { reducer: authReducer } = store.auth;

const rootReducer = combineReducers({
  auth: authReducer,
  people: peopleReducer,
  profile: profileReducer,
  group: groupReducer,
  portal: portalReducer,
});

export default rootReducer;
