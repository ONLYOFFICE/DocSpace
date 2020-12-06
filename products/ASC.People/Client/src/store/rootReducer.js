import { combineReducers } from "redux";
import peopleReducer from "./people/reducers";
import profileReducer from "./profile/reducers";
import groupReducer from "./group/reducers";
import portalReducer from "./portal/reducers";

const rootReducer = combineReducers({
  people: peopleReducer,
  profile: profileReducer,
  group: groupReducer,
  portal: portalReducer,
});

export default rootReducer;
