//import { api } from "asc-web-common";
// import axios from "axios";

export const INIT_WIZARD = 'INIT_WIZARD'; 
export const SET_OWNER = 'SET_OWNER'; 
export const GET_PARAMS = 'GET_PARAMS';
export const SET_NEW_EMAIL = 'SET_NEW_EMAIL';

export function initWizard(params) { 
  return {
    type: INIT_WIZARD,
    params
  }
}

export function setOwner(owner) {
  return { 
    type: SET_OWNER, 
    owner
  }
}

export function setNewEmail(newEmail) {
  return {
    type: SET_NEW_EMAIL,
    newEmail
  }
}

export function getParams() {
  return dispatch => {
    const params = initParams();
    return dispatch(initWizard(params));
  };
}

export function setOwnerToSrv(owner) {
  return dispatch => {
    return  setTimeout(() => {
      dispatch(setOwner(owner));
    }, 3000);
  }
}

export function saveNewEmail(newEmail) {
  return dispatch => {
    return setTimeout(() => {
      dispatch(setNewEmail(newEmail));
    }, 3000)
    
  }
}

const initParams = () => {
  return {
    isOwner: true,
    ownerEmail: 'portaldomainname@mail.com',
    domain: 'portaldomainname.com',
    language: "ru-RU",
    timezone: "UTC",
    languages: [ "English (United States)", "Русский (РФ)" ],
    timezones: [ "UTC", "Not UTC"]    
  }
}