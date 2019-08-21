import * as api from "../../store/services/api";
import { isMe } from '../auth/selectors';
import { getUserByUserName } from '../people/selectors';
import { fetchPeopleAsync } from "../people/actions";

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

function checkResponseError(res) {
    if(res && res.data && res.data.error){
        console.error(res.data.error);
        throw res.data.error.message;
    }
}

export function fetchProfile(userName) {
    return async (dispatch, getState) => {
        try {
            const { auth, people } = getState();

            if (isMe(auth.user, userName)) {
                dispatch(setProfile(auth.user));
            } else {
                const user = getUserByUserName(people.users, userName);
                if (!user) {
                    const res = await api.getUser(userName);
                    dispatch(setProfile(res.data.response))
                }
                else
                    dispatch(setProfile(user));
            }
        } catch (error) {
            console.error(error);
        }
    };
};

export function createProfile(profile) {
    return async (dispatch, getState) => {
        const {people} = getState();
        const {filter} = people;

        const res = await api.createUser(profile);

        checkResponseError(res);

        dispatch(setProfile(res.data.response))
        await fetchPeopleAsync(dispatch, filter);
    };
};

export function updateProfile(profile) {
    return async (dispatch, getState) => {
        const {people} = getState();
        const {filter} = people;

        const res = await api.updateUser(profile);

        checkResponseError(res);

        dispatch(setProfile(res.data.response))
        await fetchPeopleAsync(dispatch, filter);
    };
};