import { createStore } from 'redux';
import rootReducer from './rootReducer';

const configureStore = prelodedState => (
    createStore(
        rootReducer,
        prelodedState
    )
);
/* eslint-enable */

const store = configureStore({});

export default store;