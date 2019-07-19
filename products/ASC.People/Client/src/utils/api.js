
import axios from 'axios';
import * as fakeApi from './fakeApi';
import isEmpty from 'lodash/isEmpty';
import { objectToUrlQuery } from './converter'

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

export function getUserList(filterOptions = {}) {
    const filter = !isEmpty(filterOptions) ? `/filter.json?${objectToUrlQuery(filterOptions)}` : ""; // /filter.json?StartIndex=0&Count=25&sortby=lastname&sortorder=ascending
    return IS_FAKE ? fakeApi.getUsers() :  axios.get(`${API_URL}/people${filter}`);
};

export function getGroupList() {
    return IS_FAKE ? fakeApi.getGroups() :  axios.get(`${API_URL}/group`);
};