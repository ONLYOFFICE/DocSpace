import * as api from "../../store/services/api";
import { isMe } from '../auth/selectors';
import { getUserByUserName } from '../people/selectors';
import { fetchPeopleByFilter } from "../people/actions";

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

export function checkResponseError(res) {
    if (res && res.data && res.data.error) {
        console.error(res.data.error);
        throw new Error(res.data.error.message);
    }
}

export function employeeWrapperToMemberModel(profile) {
    const comment = profile.notes;
    const department = profile.groups ? profile.groups.map(group => group.id) : [];
    const worksFrom = profile.workFrom;

    return {...profile, comment, department, worksFrom};
}

export function fetchProfile(userName) {
    return (dispatch, getState) => {
        const { auth, people } = getState();

        if (isMe(auth.user, userName)) {
            dispatch(setProfile(auth.user));
        } else {
            const user = getUserByUserName(people.users, userName);
            if (!user) {
                api.getUser(userName).then(res => {
                    checkResponseError(res);
                    dispatch(setProfile(res.data.response));
                });
            } else {
                dispatch(setProfile(user));
            }
        }
    };
}

export function createProfile(profile) {
    return (dispatch, getState) => {
        const {people} = getState();
        const {filter} = people;
        const member = employeeWrapperToMemberModel(profile);
        let result;

        return api.createUser(member).then(res => {
            checkResponseError(res);
            result = res.data.response;
            return dispatch(setProfile(result));
        }).then(() => {
            return fetchPeopleByFilter(dispatch, filter);
        }).then(() => {
            return Promise.resolve(result);
        });
    };
};

export function updateProfile(profile) {
    return (dispatch, getState) => {
        const {people} = getState();
        const {filter} = people;
        const member = employeeWrapperToMemberModel(profile);
        let result;

        return api.updateUser(member).then(res => {
            checkResponseError(res);
            result = res.data.response;
            return Promise.resolve(dispatch(setProfile(result)));
        }).then(() => {
            return fetchPeopleByFilter(dispatch, filter);
        }).then(() => {
            return Promise.resolve(result);
        });
    };
};