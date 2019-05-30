import axios from 'axios';
import setAuthorizationToken from './setAuthorizationToken';

const BASE_URL = 'http://localhost';
const PORT = '8080';
const PREFIX = 'api';
const VERSION = '2.0';
const API_URL = `${BASE_URL}:${PORT}/${PREFIX}/${VERSION}`;

export function login(data) {
    return axios.post(`${API_URL}/authentication`, data).then(res => {
        const token = res.data.response.token;
        setAuthorizationToken(token);
    });
};


export function getModulesList() {
    return axios.get(`${API_URL}/modules`, { withCredentials: true }); // , crossdomain: true
};