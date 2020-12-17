import { createStore, applyMiddleware } from "redux";
import { composeWithDevTools } from "redux-devtools-extension/logOnlyInProduction";
import rootReducer from "./rootReducer";
import thunk from "redux-thunk";

/* eslint-disable no-underscore-dangle */
const composeEnhancers = composeWithDevTools({
  // options like actionSanitizer, stateSanitizer
});

const configureStore = (prelodedState) =>
  createStore(
    rootReducer,
    prelodedState,
    composeEnhancers(applyMiddleware(thunk))
  );
/* eslint-enable */

const store = configureStore({});

export default store;
