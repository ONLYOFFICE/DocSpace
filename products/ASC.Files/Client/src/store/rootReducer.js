import { combineReducers } from "redux";
import filesReducer from "./files/reducers";
import { store } from "asc-web-common";
const { reducer: authReducer } = store.auth;

const rootReducer = combineReducers({
  auth: authReducer,
  files: filesReducer,
});

export default rootReducer;
