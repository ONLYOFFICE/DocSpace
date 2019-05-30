import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import Cookies from 'universal-cookie';
import setAuthorizationToken from './utils/setAuthorizationToken';
import { AUTH_KEY } from './helpers/constants';
import store from './store/store';
import 'bootstrap/dist/css/bootstrap.css';
import App from './App';
import './i18n'

import * as serviceWorker from './serviceWorker';
import { getUserInfo } from './actions/authActions';

var token = (new Cookies()).get(AUTH_KEY);

if (token) {
    setAuthorizationToken(token);
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
