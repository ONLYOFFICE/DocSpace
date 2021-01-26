import { createStore, applyMiddleware } from "redux";
import thunk from "redux-thunk";
import { composeWithDevTools } from "redux-devtools-extension/logOnlyInProduction";
import { combineReducers } from "redux";
//import { createReducerManager } from "./reducerManager";

import authReducer from "@appserver/common/src/store/auth/reducer";
import settingsReducer from "./settings/reducer";
import confirmReducer from "./confirm/reducer";
import wizardReducer from "./wizard/reducer";
import paymentsReducer from "./payments/reducer";

const staticReducers = {
  auth: authReducer,
  settings: settingsReducer,
  confirm: confirmReducer,
  wizard: wizardReducer,
  payments: paymentsReducer,
};

function createReducer(asyncReducers) {
  return combineReducers({
    ...staticReducers,
    ...asyncReducers,
  });
}

/**
 * Cf. redux docs:
 * https://redux.js.org/recipes/code-splitting/#defining-an-injectreducer-function
 */
const configureStore = (initialState) => {
  const store = createStore(createReducer(), initialState);

  store.asyncReducers = {};

  store.attachReducer = (key, asyncReducer) => {
    store.asyncReducers[key] = asyncReducer;
    store.replaceReducer(createReducer(store.asyncReducers));
  };

  store.detachReducer = (key) => {
    if (key in store.asyncReducers) {
      delete store.asyncReducers[key];
      store.replaceReducer(createReducer(store.asyncReducers));
    }
  };

  return store;
};

const store = configureStore(composeWithDevTools(applyMiddleware(thunk)));

export default store;
