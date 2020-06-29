//import { api } from "asc-web-common";
// import axios from "axios";

export const INIT_WIZARD = 'INIT_WIZARD'; 
export const SET_OWNER = 'SET_OWNER'; 
export const GET_PARAMS = 'GET_PARAMS';

export function initWizard(params) { 
  return {
    type: INIT_WIZARD,
    params
  }
}

export function setOwner(owner) {
  console.log(owner)
  return { 
    type: SET_OWNER, 
    owner
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
    setDummy(owner);
    return dispatch(setOwner(owner));
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

const setDummy = (owner) => {
  console.log(owner);
}