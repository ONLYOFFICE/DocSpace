import * as api from "../../utils/api";

export const SET_PROFILE = 'SET_PROFILE';
export const CLEAN_PROFILE = 'CLEAN_PROFILE';

export function setProfile(targetUser) {
    return {
        type: SET_PROFILE,
        targetUser
    };
};

export function resetProfile() {
    return {
        type: CLEAN_PROFILE
    };
};

export function fetchProfile(userId) {
    return async (dispatch, getState) => {
        try {
            const res = await api.getUser(userId);
            dispatch(setProfile(res.data.response))
        } catch (error) {
            console.error(error);
        }
    };
};