import { createStore, applyMiddleware } from "redux";
import thunk from "redux-thunk";
import { composeWithDevTools } from "redux-devtools-extension/logOnlyInProduction";
import dynostore, {
  combineReducers,
  dynamicReducers,
} from "@redux-dynostore/core";
//import { createReducerManager } from "./reducerManager";

import authReducer from "@appserver/common/src/store/auth/reducer";
import settingsReducer from "./settings/reducer";
import confirmReducer from "./confirm/reducer";
import wizardReducer from "./wizard/reducer";
import paymentsReducer from "./payments/reducer";

const rootReducer = combineReducers({
  auth: authReducer,
  settings: settingsReducer,
  confirm: confirmReducer,
  wizard: wizardReducer,
  payments: paymentsReducer,
});

/* eslint-disable no-underscore-dangle */
// const composeEnhancers = composeWithDevTools({
//   // options like actionSanitizer, stateSanitizer
// });

//const reducerManager = createReducerManager(staticReducers);

// Create a store with the root reducer function being the one exposed by the manager.
const store = createStore(
  //reducerManager.reduce,
  rootReducer,
  composeWithDevTools(applyMiddleware(thunk), dynostore(dynamicReducers()))
);

// Optional: Put the reducer manager on the store so it is easily accessible
//store.reducerManager = reducerManager;

export default store;
