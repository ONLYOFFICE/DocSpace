
import axios from 'axios';
import * as fakeApi from './fakeApi';

const PREFIX = 'api';
const VERSION = '2.0';
const API_URL = `${window.location.origin}/${PREFIX}/${VERSION}`;

const IS_FAKE = false;

export function login(data) {
    return axios.post(`${API_URL}/authentication`, data);
};

export function getModulesList() {
    return IS_FAKE ? fakeApi.getModulesList() :
        axios.get(`${API_URL}/modules`)
        .then((res) => {
            const modules = res.data.response;
            return axios.all(modules.map((m) => axios.get(`${window.location.origin}/${m}`)))
        })
        .then((res) => {
            const response = res.map((d) => d.data.response);
            return Promise.resolve({ data: { response } });
        });
};

export function getUser() {
    return IS_FAKE ? fakeApi.getUser() :  axios.get(`${API_URL}/people/@self.json`);
};

export function getUserList() {
    return IS_FAKE ? fakeApi.getUsers() :  axios.get(`${API_URL}/people`);
};

export function getGroupList() {
    return IS_FAKE ? fakeApi.getGroups() :  axios.get(`${API_URL}/group`);
};