import * as types from '../actions/actionTypes'

const modulesReducer = (state = [], action) => {
    switch (action.type) {
        case types.MODULES_GET:
            return state.concat([action.data])
        default:
            return state;
    }
}

export default modulesReducer;