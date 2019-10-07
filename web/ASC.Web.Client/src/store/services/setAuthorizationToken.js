import axios from 'axios';
import Cookies from 'universal-cookie';
import { AUTH_KEY } from '../../helpers/constants';
import history from '../../history';
import { toastr } from 'asc-web-components';

export default function setAuthorizationToken(token) {
    axios.interceptors.response.use(response => {
        return response;
     }, error => {
       if (error.response.status === 401) {
        //place your reentry code
        history.push("/login/error=unauthorized");
       }

       toastr.error(error.response);

       return error;
     });
    
    const cookies = new Cookies();

    if (token) {
        axios.defaults.headers.common["Authorization"] = token;
        localStorage.setItem(AUTH_KEY, token);

        const current = new Date();
        const nextYear = new Date();

        nextYear.setFullYear(current.getFullYear() + 1);

        cookies.set(AUTH_KEY, token, { // Cookie is necessary for private images (avatars etc.)
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