import { SET_PAGING } from "./actions";

const initialState = {
    paging: {
        page: 0,
        pageCount: 25,
        prev: false,
        next: true
    }
};

const pagingReducer = (state = initialState, action) => {
    switch (action.type) {
        case SET_PAGING:
            return Object.assign({}, state, {
                paging: action.paging
            });
        default:
            return state;
    }
};

export default pagingReducer;
