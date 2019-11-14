import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { AUTH_KEY } from './helpers/constants';
import store from './store/store';
import './custom.scss';
import App from './App';
import * as serviceWorker from './serviceWorker';
import { store as commonStore } from 'asc-web-common';
const { getUserInfo, getPortalSettings } = commonStore.auth.actions;

const token = localStorage.getItem(AUTH_KEY);

if(!token) {
    store.dispatch(getPortalSettings);
}
else if (!window.location.pathname.includes("confirm/EmailActivation")) {
    store.dispatch(getUserInfo);
}

ReactDOM.render(
    <Provider store={store}>
        <App />
    </Provider>,
    document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
