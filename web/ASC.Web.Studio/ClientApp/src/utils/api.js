import axios from 'axios';
import * as fakeApi from './fakeApi';

const BASE_URL = `${window.location.protocol}//${window.location.hostname}`;
const PORT = '5000';
const PREFIX = 'api';
const VERSION = '2.0';
const API_URL = `${BASE_URL}:${PORT}/${PREFIX}/${VERSION}`;

const IS_FAKE = true;

export function login(data) {
    return axios.post(`${API_URL}/authentication`, data);
};

export function getModulesList() {
    return IS_FAKE ? fakeApi.getModulesList() : axios.get(`${API_URL}/modules`);
};

export function getUser() {
    return IS_FAKE ? fakeApi.getUser() :  axios.get(`${API_URL}/people/@self.json`);
};