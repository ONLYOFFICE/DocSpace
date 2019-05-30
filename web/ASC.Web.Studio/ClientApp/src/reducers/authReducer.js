import * as types from '../actions/actionTypes'

const authReducer = (state = [], action) => {
    switch (action.type) {
        case types.LOGIN_POST:
            return state.concat([action.data])
        default:
            return state;
    }
}

export default authReducer;