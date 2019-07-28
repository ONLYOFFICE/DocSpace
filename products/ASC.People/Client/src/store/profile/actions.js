import * as api from "../../utils/api";
import { isMe } from '../auth/selectors';
import { getUserByUserName } from '../people/selectors';

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
            const { auth, people } = getState();

            if (isMe(auth, userId)) {
                dispatch(setProfile(auth.user));
            } else {
                const user = getUserByUserName(people.users, userId);
                if (!user) {
                    const res = await api.getUser(userId);
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