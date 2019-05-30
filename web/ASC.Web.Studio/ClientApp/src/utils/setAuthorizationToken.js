import axios from 'axios';
import Cookies from 'universal-cookie';
import { AUTH_KEY } from '../helpers/constants';

export default function setAuthorizationToken(token) {
    const cookies = new Cookies();

    if (token) {
        //localStorage.setItem(AUTH_KEY, token);
        //axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
        const current = new Date();
        const nextYear = new Date();

        nextYear.setFullYear(current.getFullYear() + 1);

        axios.defaults.withCredentials = true;
        cookies.set(AUTH_KEY, token, {
            path: '/',
            expires: nextYear,
        });
    }
    else {
        axios.defaults.withCredentials = false
        cookies.remove(AUTH_KEY);
        //localStorage.removeItem(AUTH_KEY, token);
        //delete axios.defaults.headers.common["Authorization"];
        //setCookie(AUTH_KEY);
    }
}