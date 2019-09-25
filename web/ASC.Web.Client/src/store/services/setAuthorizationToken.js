import axios from 'axios';
import Cookies from 'universal-cookie';
import { AUTH_KEY } from '../../helpers/constants';

export default function setAuthorizationToken(token) {
    const cookies = new Cookies();

    if (token) {
        axios.defaults.headers.common["Authorization"] = token;

        const current = new Date();
        const nextYear = new Date();

        nextYear.setFullYear(current.getFullYear() + 1);

        cookies.set(AUTH_KEY, token, {
            path: '/',
            expires: nextYear,
        });
    }
    else {
        localStorage.clear();
        delete axios.defaults.headers.common["Authorization"];
        cookies.remove(AUTH_KEY);
    }
}